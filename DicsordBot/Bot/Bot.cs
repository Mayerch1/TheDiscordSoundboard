using System.Threading.Tasks;

using NAudio.Wave;

using Discord;
using Discord.WebSocket;
using Discord.Audio;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace DicsordBot.Bot
{
    /*
     * Thin Discord-Bot client
     * Provides all functions for connecting/disconnecting to/from server
     * Provides all functions to queue files and skip, pause the stream
     *
     * No error Handling, just crashes, when sth is wrong
     *
     * To use as standalone simply change all protected keywords to public
     * But prepare for exceptions (and crashes, if not handled)
    */

    public class Bot
    {
        #region config

        private const int channelCount = 2;
        private const int sampleRate = 48000;
        private const int sampleQuality = 60;
        private const int packagesPerSecond = 50;

        //if changed, also change applyVolume();
        private const int bitDepth = 16;

        #endregion config

        #region event Handlers

        public delegate void StreamStateHandler(bool newState);

        public StreamStateHandler StreamStateChanged;

        public delegate void EarrapeStateHandler(bool isEarrape);

        public EarrapeStateHandler EarrapeStateChanged;

        public delegate void LoopStateHandler(bool isLoop);

        public LoopStateHandler LoopStateChanged;

        #endregion event Handlers

        #region status fields

        private bool isStreaming;

        #endregion status fields

        #region status propertys

        //1.0 is 100%
        //5.0 is earrape
        //10.0 is just static noise
        public float Volume { get; set; }

        public bool IsServerConnected { get; set; }
        public bool IsChannelConnected { get; set; }

        public bool IsStreaming
        {
            get { return isStreaming; }
            private set { if (value != isStreaming) { isStreaming = value; StreamStateChanged(isStreaming); } }
        }

        public TimeSpan CurrentTime { get { if (Reader != null) return Reader.CurrentTime; else return TimeSpan.Zero; } }
        public TimeSpan TitleLenght { get { if (Reader != null) return Reader.TotalTime; else return TimeSpan.Zero; } }

        public bool IsBufferEmpty { get; set; }

        public bool IsLoop { get; set; } = false;

        private bool IsToAbort { get; set; } = false;

        private uint SkipTracks { get; set; }
        public bool IsEarrape { get; set; } = false;

        #endregion status propertys

        #region other vars

        private DiscordSocketClient Client { get; set; }
        private IAudioClient AudioCl { get; set; }
        protected Queue<Data.ButtonData> Queue { get; set; }

        private MediaFoundationReader Reader { get; set; }
        private MediaFoundationResampler Resampler { get; set; }
        private WaveFormat OutFormat { get; set; }

        #endregion other vars

        public Bot()
        {
            //TOOD: maybe other format, to format more information
            Queue = new Queue<Data.ButtonData>();
            IsStreaming = false;
            IsChannelConnected = false;
            IsServerConnected = false;
            IsBufferEmpty = true;
        }

        #region controll stuff

        protected void enqueueAsync(Data.ButtonData btn)
        {
            if (!IsStreaming)
            {
                getStream(btn);
            }
            else
            {
                Queue.Enqueue(btn);
            }
        }

        protected async Task resumeStream()
        {
            await startStreamAsync();
        }

        public void skipTrack()
        {
            if (IsStreaming)
                SkipTracks += 1;
        }

        public void skipToTime(TimeSpan newTime, bool enforce = false)
        {
            if (IsStreaming || enforce)
            {
                Reader.CurrentTime = newTime;
            }
        }

        public void skipOverTime(TimeSpan skipTime)
        {
            if (IsStreaming)
            {
                Reader.CurrentTime = Reader.CurrentTime.Add(skipTime);
            }
        }

        protected async Task setGameState(string msg, string streamUrl = "", bool isStreaming = false)
        {
            if (!IsServerConnected)
                throw new BotException(BotException.type.connection, "Not connected to the Servers, while Setting GameState", BotException.connectionError.NoServer);

            StreamType type = StreamType.NotStreaming;
            if (isStreaming)
                type = StreamType.Twitch;

            if (streamUrl == "")
                await Client.SetGameAsync(msg, streamUrl, type);
            else
                await Client.SetGameAsync(msg);
        }

        #endregion controll stuff

        #region play stuff

        private void getStream(Data.ButtonData btn)
        {
            if (btn.File == null || btn.File == "")
                return;

            OutFormat = new WaveFormat(sampleRate, bitDepth, channelCount);

            Reader = new MediaFoundationReader(btn.File);

            Resampler = new MediaFoundationResampler(Reader, OutFormat);

            IsBufferEmpty = false;

            loadOverrideSettings(btn);
        }

        private async Task startStreamAsync(AudioOutStream stream = null)
        {
            //IsChannelConnected gaurantees, to have IsServerConnected
            if (!IsChannelConnected)
            {
                if (!IsServerConnected)
                    throw new BotException(BotException.type.connection, "Not connected to Server, while trying to start stream", BotException.connectionError.NoServer);
                else
                    throw new BotException(BotException.type.connection, "Not connected to a channel, while trying to start stream", BotException.connectionError.NoChannel);
            }

            if (!IsStreaming && IsServerConnected && AudioCl != null)
            {
                if (stream == null)
                    stream = AudioCl.CreatePCMStream(AudioApplication.Music);

                if (Resampler == null)
                {
                    stream.Close();
                    return;
                }

                //send stream in small packages
                int blockSize = OutFormat.AverageBytesPerSecond / packagesPerSecond;
                byte[] buffer = new byte[blockSize];
                int byteCount;

                IsStreaming = true;

                //repeat, read new block into buffer -> stream buffer
                while ((byteCount = Resampler.Read(buffer, 0, blockSize)) > 0)
                {
                    applyVolume(ref buffer);

                    if (IsToAbort || SkipTracks > 0)
                        break;

                    if (byteCount < blockSize)
                    {
                        //fill rest of stream with '0'
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                        IsBufferEmpty = true;
                    }

                    await stream.WriteAsync(buffer, 0, blockSize);
                }

                IsStreaming = false;

                //reopen the same file
                if (IsLoop && !IsToAbort && SkipTracks == 0)
                {
                    //move head to begin of file
                    skipToTime(TimeSpan.Zero, true);
                    await startStreamAsync(stream);
                }
                //next file in queue
                else if (Queue.Count > 0 && !IsToAbort)
                {
                    //skip one less
                    if (SkipTracks > 0)
                        --SkipTracks;

                    //reset for next song
                    if (IsLoop)
                        LoopStateChanged(false);

                    getStream(Queue.Dequeue());
                    await startStreamAsync(stream);
                }
                //exit stream
                else
                {
                    //can't skip a track if nothing is running
                    SkipTracks = 0;
                    //wait until last packages are played
                    await Task.Delay(1250);

                    stream.Close();
                    IsToAbort = false;
                }
            }
        }

        //if button has any override settings, raise their events
        private void loadOverrideSettings(Data.ButtonData btn)
        {
            //if earrape changes
            if (IsEarrape != btn.IsEarrape)
            {
                if (btn.IsEarrape)
                {
                    EarrapeStateChanged(true);
                }
                else
                {
                    EarrapeStateChanged(false);
                }
            }
            //trigger this every time

            //IsLoop will be set from outside

            LoopStateChanged(btn.IsLoop);
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

        #endregion play stuff

        #region start stuff

        protected async Task connectToServerAsync(string token)
        {
            if (IsServerConnected)
                await disconnectFromServerAsync();

            Client = new DiscordSocketClient();

            await Client.LoginAsync(TokenType.Bot, token);

            await Client.StartAsync();

            IsServerConnected = true;
        }

        protected async Task connectToChannelAsync(ulong channelId)
        {
            if (IsChannelConnected)
                await disconnectFromChannelAsync();

            if (!IsServerConnected)
            {
                throw new BotException(BotException.type.connection, "Not connected to the servers, while trying to connect to a channel", BotException.connectionError.NoServer);
            }

            AudioCl = await ((ISocketAudioChannel)Client.GetChannel(channelId)).ConnectAsync();

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
        protected List<List<SocketVoiceChannel>> getAllChannels()
        {
            if (!IsServerConnected)
                throw new BotException(BotException.type.connection, "Not connectet to the servers, while trying to get channel list", BotException.connectionError.NoServer);

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
        protected List<List<SocketGuildUser>> getAllClients(bool acceptOffline)
        {
            if (!IsServerConnected)
                throw new BotException(BotException.type.connection, "Not connectet to the servers, while trying to get clint list", BotException.connectionError.NoServer);

            List<List<SocketGuildUser>> guildList = new List<List<SocketGuildUser>>();

            var guilds = Client.Guilds;
            foreach (var gElement in guilds)
            {
                List<SocketGuildUser> subList = new List<SocketGuildUser>();
                var users = gElement.Users;

                foreach (var singleUser in users)
                {
                    if (singleUser.VoiceChannel != null || acceptOffline)
                        subList.Add(singleUser);
                }
                guildList.Add(subList);
            }

            return guildList;
        }

        #endregion get data
    }
}