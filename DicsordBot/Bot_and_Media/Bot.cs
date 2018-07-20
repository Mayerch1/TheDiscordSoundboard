using System;
using System.Threading.Tasks;

using NAudio.Wave;

using Discord;
using Discord.WebSocket;
using Discord.Audio;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Collections.Generic;

namespace DicsordBot
{
    public class Bot
    {
        #region config

        private const int channelCount = 2;
        private const int sampleRate = 48000;
        private const int sampleQuality = 60;

        //if changed, also change applyVolume();
        private const int bitDepth = 16;

        #endregion config

        #region event Handlers

        public delegate void StreamStateHandler(bool newState);

        public StreamStateHandler StreamStateChanged;

        #endregion event Handlers

        #region status fields

        private bool isStreaming;

        #endregion status fields

        #region status propertys

        //1.0 is 100%
        //5.0 is earrape
        //10.0 is just static noise
        public float Volume { get; set; } = 0.5f;

        public bool IsServerConnected { get; private set; } = false;
        public bool IsChannelConnected { get; private set; } = false;

        public bool IsStreaming
        {
            get { return isStreaming; }
            private set { if (value != isStreaming) { isStreaming = value; StreamStateChanged(isStreaming); } }
        }

        public bool IsLoop { get; set; } = false;

        private bool IsToAbort { get; set; } = false;

        private uint SkipSeconds { get; set; }
        private uint SkipTracks { get; set; }

        #endregion status propertys

        #region other vars

        private DiscordSocketClient Client { get; set; }
        private IAudioClient AudioCl { get; set; }
        public Queue<ButtonData> Queue { get; set; }

        #endregion other vars

        public Bot()
        {
            //TOOD: maybe other format, to format more information
            Queue = new Queue<ButtonData>();
            IsStreaming = false;
        }

        #region controll stuff

        public async Task enqueueAsync(ButtonData btn)
        {
            Queue.Enqueue(btn);
            if (!IsStreaming)
                await startStreamAsync(Queue.Dequeue());
        }

        public void skipTrack()
        {
            SkipTracks += 1;
        }

        public void skipSeconds(uint skip)
        {
            SkipSeconds += skip * 50;
        }

        public async Task<bool> setGameState(string msg, string streamUrl = "", bool isStreaming = false)
        {
            if (!IsServerConnected)
                return false;

            StreamType type = StreamType.NotStreaming;
            if (isStreaming)
                type = StreamType.Twitch;

            try
            {
                if (streamUrl == "")
                    await Client.SetGameAsync(msg, streamUrl, type);
                else
                    await Client.SetGameAsync(msg);
            }
            catch
            {
                throw new BotException(BotException.type.others, "Could not set Game-state", BotException.connectionError.Unspecified);
            }
            return true;
        }

        #endregion controll stuff

        #region play stuff

        private async Task startStreamAsync(ButtonData btn, AudioOutStream stream = null)
        {
            //IsChannelConnected gaurantees, to have IsServerConnected
            if (!IsChannelConnected)
            {
                if (!IsServerConnected)
                    throw new BotException(BotException.type.connection, "Not connected", BotException.connectionError.NoServer);
                else
                    throw new BotException(BotException.type.connection, "Not connected", BotException.connectionError.NoChannel);
            }

            if (!IsStreaming && IsServerConnected && AudioCl != null)
            {
                if (stream == null)
                    stream = AudioCl.CreatePCMStream(AudioApplication.Music);

                MediaFoundationResampler resampler = null;
                var OutFormat = new WaveFormat(sampleRate, bitDepth, channelCount);

                resampler = getFileStream(btn.File, OutFormat);

                loadOverrideSettings(btn);

                if (resampler == null)
                {
                    stream.Close();
                    return;
                }

                //send stream in small packages
                int blockSize = OutFormat.AverageBytesPerSecond / 50;
                byte[] buffer = new byte[blockSize];
                int byteCount;

                IsStreaming = true;

                //repeat, read new block into buffer -> stream buffer
                while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0)
                {
                    applyVolume(ref buffer);

                    if (IsToAbort || SkipTracks > 0)
                        break;

                    if (byteCount < blockSize)
                    {
                        //fill rest of stream with '0'
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }

                    if (SkipSeconds > 0)
                    {
                        --SkipSeconds;
                        continue;
                    }

                    await stream.WriteAsync(buffer, 0, blockSize);
                }

                IsStreaming = false;

                //reopen the same file
                if (IsLoop && !IsToAbort && SkipTracks == 0)
                {
                    await startStreamAsync(btn, stream);
                    return;
                }
                //next file in queue
                else if (Queue.Count > 0 && !IsToAbort)
                {
                    //skip one less
                    if (SkipTracks > 0)
                        //reset for next song
                        --SkipTracks;

                    IsLoop = false;
                    //IDEA: disable earrape

                    await startStreamAsync(Queue.Dequeue(), stream);
                    return;
                }
                //exit stream
                else
                {
                    //wait until last packages are played
                    await Task.Delay(1250);

                    stream.Close();
                    IsToAbort = false;
                    return;
                }
            }
        }

        //if button has any override settings, load them
        private void loadOverrideSettings(ButtonData btn)
        {
            if (btn.Volume > 0)
                Volume = btn.Volume;

            if (btn.IsLoop)
                IsLoop = btn.IsLoop;
        }

        private void applyVolume(ref byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i += 2)
            {
                //convert a byte-Pair into one char (with 2 bytes)

                short bytePair = (short)((buffer[i + 1] & 0xFF) << 8 | (buffer[i] & 0xFF));

                //float floatPair = bytePair * Volume;

                bytePair = (short)(bytePair * Volume);

                //convert char back to 2 bytes
                buffer[i] = (byte)bytePair;
                buffer[i + 1] = (byte)(bytePair >> 8);
            }
        }

        //get resampler for giver format
        private MediaFoundationResampler getFileStream(string file, WaveFormat OutFormat)
        {
            Mp3FileReader mp3File;
            WaveFileReader waveFile;

            MediaFoundationResampler resampler = null;

            //*.wav
            if (file[file.Length - 1] == 'v')
            {
                try
                {
                    waveFile = new WaveFileReader(file);
                }
                catch
                {
                    throw new BotException(BotException.type.file, "Could not load wav file", BotException.connectionError.Unspecified);
                }
                resampler = new MediaFoundationResampler(waveFile, OutFormat);
            }
            //*.mp3
            else
            {
                try
                {
                    mp3File = new Mp3FileReader(file);
                }
                catch
                {
                    throw new BotException(BotException.type.file, "Could not load mp3 file", BotException.connectionError.Unspecified);
                }
                resampler = new MediaFoundationResampler(mp3File, OutFormat);
            }
            resampler.ResamplerQuality = sampleQuality;

            return resampler;
        }

        #endregion play stuff

        #region start stuff

        public async Task connectToServerAsync(string token)
        {
            if (IsServerConnected)
                await disconnectFromServerAsync();

            Client = new DiscordSocketClient();

            try
            {
                await Client.LoginAsync(TokenType.Bot, token);
            }
            catch
            {
                throw new BotException(BotException.type.connection, "Could not connect to Server", BotException.connectionError.NoServer);
            }
            await Client.StartAsync();

            //IDEA: maybe set gamestate here

            IsServerConnected = true;
        }

        public async Task connectToChannelAsync(ulong channelId)
        {
            if (IsChannelConnected)
                await disconnectFromChannelAsync();

            if (!IsServerConnected)
            {
                throw new BotException(BotException.type.connection, "No server connection", BotException.connectionError.NoServer);
            }

            try
            {
                AudioCl = await ((ISocketAudioChannel)Client.GetChannel(channelId)).ConnectAsync();
            }
            catch
            {
                throw new BotException(BotException.type.file, "Could not connect to Channel", BotException.connectionError.NoChannel);
            }
            IsChannelConnected = true;
        }

        #endregion start stuff

        #region stop stuff

        public async Task disconnectFromChannelAsync()
        {
            if (!IsChannelConnected)
                return;

            await stopStreamAsync();

            await AudioCl.StopAsync();

            IsChannelConnected = false;
        }

        public async Task disconnectFromServerAsync()
        {
            if (!IsServerConnected)
                return;

            await disconnectFromChannelAsync();

            //IDEA: maybe set game state

            //wait until last packet is played
            while (IsChannelConnected)
                await Task.Delay(10);

            await Client.StopAsync();
            await Client.LogoutAsync();

            IsServerConnected = false;
        }

        public async Task stopStreamAsync()
        {
            if (!IsStreaming)
                return;

            IsToAbort = true;

            //wait until last package is played
            while (IsStreaming)
                await Task.Delay(10);

            //make shure to not block future streams
            IsToAbort = false;
        }

        #endregion stop stuff

        #region get data

        //returns a List<List>, all channels of all servers are contained
        public List<List<SocketVoiceChannel>> getAllChannels()
        {
            if (!IsServerConnected)
                throw new BotException(BotException.type.connection, "No Server-connection", BotException.connectionError.NoServer);

            List<List<SocketVoiceChannel>> guildList = new List<List<SocketVoiceChannel>>();

            //get all Servers the bot is connectet to
            var guilds = Client.Guilds;

            foreach (var gElement in guilds)
            {
                //get all channels of a server, add them to list

                List<SocketVoiceChannel> subList = new List<SocketVoiceChannel>();

                var vChannels = gElement.VoiceChannels;
                foreach (var vElement in vChannels)
                {
                    subList.Add(vElement);
                }
                guildList.Add(subList);
            }

            return guildList;
        }

        //returns a List<List>, all online clients of all servers are contained
        public List<List<SocketGuildUser>> getAllClients()
        {
            if (!IsServerConnected)
                throw new BotException(BotException.type.connection, "No Server-connection", BotException.connectionError.NoServer);

            List<List<SocketGuildUser>> guildList = new List<List<SocketGuildUser>>();

            var guilds = Client.Guilds;
            foreach (var gElement in guilds)
            {
                List<SocketGuildUser> subList = new List<SocketGuildUser>();
                var users = gElement.Users;

                foreach (var singleUser in users)
                {
                    if (singleUser.VoiceChannel != null)
                        subList.Add(singleUser);
                }
                guildList.Add(subList);
            }

            return guildList;
        }

        #endregion get data
    }
}