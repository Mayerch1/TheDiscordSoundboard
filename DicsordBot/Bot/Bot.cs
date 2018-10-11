using Discord;
using Discord.Audio;
using Discord.WebSocket;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DicsordBot.Bot
{
    /// <summary>
    /// Basic Bot class, directly communicates with the api, throws for every little sh
    /// </summary>
    /// <remarks>
    /// can used as standalone but requires to change all 'protected' keywords to 'public' in order to be accessed, also it is not recommended
    /// if bot is not connected to server or channel, it throws a BotException(...)
    /// </remarks>
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

        /// <summary>
        /// StreamStateHandler delegate
        /// </summary>
        /// <param name="newState">new Streaming state</param>
        public delegate void StreamStateHandler(bool newState);

        /// <summary>
        /// StreamStateChanged filed
        /// </summary>
        public StreamStateHandler StreamStateChanged;

        /// <summary>
        /// EarrapeStateHandler delegate
        /// </summary>
        /// <param name="isEarrape">new isEarrape value</param>
        public delegate void EarrapeStateHandler(bool isEarrape);

        /// <summary>
        /// EarrapeStateChanged field
        /// </summary>
        public EarrapeStateHandler EarrapeStateChanged;

        /// <summary>
        /// LoopStateHandler delegate
        /// </summary>
        /// <param name="isLoop">new isLoop value</param>
        public delegate void LoopStateHandler(bool isLoop);

        /// <summary>
        /// LoopStateChanged field
        /// </summary>
        public LoopStateHandler LoopStateChanged;

        #endregion event Handlers

        #region status fields

        private bool isStreaming;

        #endregion status fields

        #region status propertys

        /// <summary>
        /// Volume property
        /// </summary>
        /// <remarks>
        /// 1.0 is 100%, 10.0 is static noise
        /// </remarks>
        public float Volume { get; set; }

        /// <summary>
        /// IsServerConnected property
        /// </summary>
        public bool IsServerConnected { get; set; }

        /// <summary>
        /// IsChannelConnected property
        /// </summary>
        public bool IsChannelConnected { get; set; }

        /// <summary>
        /// IsStreaming property, calls StreamStateChanged delegate
        /// </summary>
        public bool IsStreaming
        {
            get { return isStreaming; }
            private set { if (value != isStreaming) { isStreaming = value; StreamStateChanged(isStreaming); } }
        }

        /// <summary>
        /// CurrentTime property
        /// </summary>
        public TimeSpan CurrentTime { get { if (Reader != null) return Reader.CurrentTime; else return TimeSpan.Zero; } }

        /// <summary>
        /// TitleLength property
        /// </summary>
        public TimeSpan TitleLenght { get { if (Reader != null) return Reader.TotalTime; else return TimeSpan.Zero; } }

        /// <summary>
        /// IsBufferEmpty property
        /// </summary>
        public bool IsBufferEmpty { get; set; }

        /// <summary>
        /// IsLoop property
        /// </summary>
        public bool IsLoop { get; set; } = false;

        /// <summary>
        /// IsToAbort property
        /// </summary>
        private bool IsToAbort { get; set; } = false;

        private uint SkipTracks { get; set; }

        /// <summary>
        /// IsEarrape property
        /// </summary>
        public bool IsEarrape { get; set; } = false;

        #endregion status propertys

        #region other vars

        private DiscordSocketClient Client { get; set; }
        private IAudioClient AudioCl { get; set; }

        /// <summary>
        /// queue of files which are going to be played
        /// </summary>
        /// <remarks>
        /// Contains data representation of Buttons, to also store settings like a custom loop-state
        /// </remarks>
        private List<BotData> Queue { get; set; }

        private MediaFoundationReader Reader { get; set; }
        private MediaFoundationResampler ActiveResampler { get; set; }

        private MediaFoundationResampler NormalResampler { get; set; }
        private MediaFoundationResampler BoostResampler { get; set; }
        private WaveFormat OutFormat { get; set; }

        #endregion other vars

        /// <summary>
        /// constructor inits important properties
        /// </summary>
        public Bot()
        {
            Queue = new List<BotData>();
            IsStreaming = false;
            IsChannelConnected = false;
            IsServerConnected = false;
            IsBufferEmpty = true;
        }

        #region controll stuff

        /// <summary>
        /// enqueues a btn into the queue, if queue is empy directly gather stream
        /// </summary>
        /// <param name="data"></param>
        protected void enqueueAsync(BotData data)
        {
            if (!IsStreaming && IsBufferEmpty)
            {
                getStream(data);
            }
            else
            {
                Queue.Add(data);
            }
        }

        /// <summary>
        /// enqueues a btn at the first position of the queue
        /// </summary>
        /// <param name="data"></param>
        protected void enqueuePriorityAsync(BotData data)
        {
            if (!IsStreaming)
            {
                getStream(data);
            }
            else
            {
                //insert on first position
                Queue.Insert(0, data);
            }
        }

        /// <summary>
        /// start the stream
        /// </summary>
        /// <returns>Task</returns>
        protected async Task resumeStream()
        {
            await startStreamAsync();
        }

        /// <summary>
        /// skips the track, can be cummulated to skip multiple tracks
        /// </summary>
        public void skipTrack()
        {
            if (IsStreaming)
                SkipTracks += 1;
        }

        /// <summary>
        /// skips ahead to a timespan
        /// </summary>
        /// <param name="newTime">new Time</param>
        /// <param name="enforce">enforce the skip, even if nothing is playing</param>
        public void skipToTime(TimeSpan newTime, bool enforce = false)
        {
            if (IsStreaming || enforce)
            {
                Reader.CurrentTime = newTime;
            }
        }

        /// <summary>
        /// skip over a time period
        /// </summary>
        /// <param name="skipTime">timeSpan to skip over</param>
        public void skipOverTime(TimeSpan skipTime)
        {
            if (IsStreaming)
            {
                Reader.CurrentTime = Reader.CurrentTime.Add(skipTime);
            }
        }

        /// <summary>
        /// sets the GameState of the bot
        /// </summary>
        /// <param name="msg">Message to be displayed</param>
        /// <param name="streamUrl">Url to twitch-stream, only relevant when isStreamin is true</param>
        /// <param name="isStreaming">bool, if bot is streaming on twitch or not</param>
        /// <returns>Task</returns>
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

        /// <summary>
        /// gets the stream saved in btn.File
        /// </summary>
        /// <param name="data">BotData object</param>
        private void getStream(BotData data)
        {
            //return if nothing to stream
            if (!File.Exists(data.filePath) && data.stream == null)
                return;

            OutFormat = new WaveFormat(sampleRate, bitDepth, channelCount);

            if (data.filePath != null)
            {
                Reader = new MediaFoundationReader(data.filePath);
                NormalResampler = new MediaFoundationResampler(Reader, OutFormat);
            }
            else if (data.stream != null)
            {
                IWaveProvider provider = new RawSourceWaveStream(new MemoryStream(data.stream), OutFormat);
                NormalResampler = new MediaFoundationResampler(provider, OutFormat);
                //TODO: test seeking behaviour with memoryStream
                //maybe global var, flipping between memorystream and MediaF.Reader
            }

            /*
             * Generate one normal resampler,
             * Generate one boosted resampler,
             * in applyVolume() the matching resampler is assigned to activeResampler
             */

            var volumeSampler = new VolumeWaveProvider16(NormalResampler);
            //this means 10,000%
            volumeSampler.Volume = 100;
            BoostResampler = new MediaFoundationResampler(volumeSampler, OutFormat);

            ActiveResampler = NormalResampler;

            IsBufferEmpty = false;

            loadOverrideSettings(data);
        }

        /// <summary>
        /// starts the stream
        /// </summary>
        /// <param name="stream">if one stream is already opened it goes here to prevent a gap in the audio line</param>
        /// <returns>Task</returns>
        /// <remarks>
        /// calls itself again as long as isLoop is true
        /// </remarks>
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

                if (ActiveResampler == null)
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
                while ((byteCount = ActiveResampler.Read(buffer, 0, blockSize)) > 0)
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

                    //queue must contain at least one item
                    var nextTitle = Queue[0];
                    Queue.RemoveAt(0);
                    getStream(nextTitle);

                    await startStreamAsync(stream);
                }
                //exit stream
                else
                {
                    //can't skip a track if nothing is running
                    SkipTracks = 0;

                    //wait until last packages are played
                    await stream.FlushAsync();

                    stream.Close();
                    IsToAbort = false;
                }
            }
        }

        /// <summary>
        /// load all specific button settings, raise events to call back to ui for visual indication
        /// </summary>
        /// <param name="data">BotData object</param>
        private void loadOverrideSettings(BotData data)
        {
            //if earrape changes
            if (IsEarrape != data.isEarrape)
            {
                if (data.isEarrape)
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

            LoopStateChanged(data.isLoop);
        }

        /// <summary>
        /// split buffer and apply volume to each byte pair
        /// </summary>
        /// <param name="buffer">ref to byte array package of the current stream</param>
        private void applyVolume(ref byte[] buffer)
        {
            if (IsEarrape)
            {
                ActiveResampler = BoostResampler;
            }
            else
            {
                ActiveResampler = NormalResampler;
                for (int i = 0; i < buffer.Length; i += 2)
                {
                    //convert a byte-Pair into one char (with 2 bytes)

                    short bytePair = (short)((buffer[i + 1] & 0xFF) << 8 | (buffer[i] & 0xFF));

                    //float floatPair = bytePair * Volume;

                    var customVol = Volume;
                    var overOne = Volume - 1;

                    bytePair = (short)(bytePair * customVol);

                    //convert char back to 2 bytes
                    buffer[i] = (byte)bytePair;
                    buffer[i + 1] = (byte)(bytePair >> 8);
                }
            }
        }

        #endregion play stuff

        #region start stuff

        /// <summary>
        /// connect to server
        /// </summary>
        /// <param name="token">bot token used to login</param>
        /// <returns>Task</returns>
        protected async Task connectToServerAsync(string token)
        {
            if (IsServerConnected)
                await disconnectFromServerAsync();

            Client = new DiscordSocketClient();

            await Client.LoginAsync(TokenType.Bot, token);

            await Client.StartAsync();

            IsServerConnected = true;
        }

        /// <summary>
        /// connect to specific channel
        /// </summary>
        /// <param name="channelId">id of channel to join</param>
        /// <returns>Task</returns>
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

        /// <summary>
        /// stop running streams and disconnect from channel
        /// </summary>
        /// <returns>Task</returns>
        /// <see cref="stopStreamAsync()"/>
        public async Task disconnectFromChannelAsync()
        {
            if (!IsChannelConnected)
                return;

            await stopStreamAsync();

            await AudioCl.StopAsync();

            IsChannelConnected = false;
        }

        /// <summary>
        /// disconnect from channel and close connection to sevrer
        /// </summary>
        /// <returns>Task</returns>
        /// <see cref="disconnectFromChannelAsync()"/>

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

        /// <summary>
        /// stop a running stream
        /// </summary>
        /// <returns>Task</returns>
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

        /// <summary>
        /// get a list of all channels of all servers
        /// </summary>
        /// <returns>list of all servers, each contains a list of all channels</returns>
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

        /// <summary>
        /// get a list of all clients of all servers
        /// </summary>
        /// <param name="acceptOffline">incude users which are offline</param>
        /// <returns>list of all servers, each contains a list of all clients, regarding acceptOffline</returns>
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