using DataManagement;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BotModule
{
    /// <summary>
    /// Basic Bot class, directly communicates with the api, no fail safe, throws for everything
    /// </summary>
    /// <seealso cref="BotHandle"/>
    /// <remarks>
    /// can used as standalone but requires to change all 'protected' keywords to 'public' in order to be accessed, also it is not recommended
    /// if bot is not connected to server or channel, it throws a <see cref="BotException"/>
    /// </remarks>
    public class Bot
    {
        #region config

        //===========================
        private const int packagesPerSecond = 50;
        //===========================

        #endregion config

        #region event Handlers

        /// <summary>
        /// SpeedAvailableChangedHandler delegate
        /// </summary>
        /// <param name="isAvailable">modify speed is available</param>
        public delegate void SpeedAvailableChangedHandler(bool isAvailable);

        /// <summary>
        /// Indicates a change in the availability for the speed slider
        /// Live sources like Microphone are not able to provide this function.
        /// </summary>
        public SpeedAvailableChangedHandler SpeedAvailableChanged;


        /// <summary>
        /// IncompatibleWaveHandler delegate
        /// </summary>
        protected delegate void IncompatibleWaveHandler();

        /// <summary>
        /// Gets triggered when the waveformat of a infinite stream is not equals to the desired format
        /// Replay is possible, but the audio is distorted
        /// </summary>
        protected IncompatibleWaveHandler IncompatibleWave;


        /// <summary>
        /// EndOfFileHandler delegate
        /// </summary>
        public delegate void EndOfFileHandler();

        /// <summary>
        /// EndOfFile field
        /// </summary>
        public EndOfFileHandler EndOfFile;


        /// <summary>
        /// StreamStateHandler delegate
        /// </summary>
        /// <param name="newState">new Streaming state</param>
        /// <param name="songName">Name of the current Song</param>
        public delegate void StreamStateHandler(bool newState, string songName);

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

        private bool isStreaming = false;
        private bool isChannelConnected = false;
        private string currentSong = "";
        private float pitch = 1.0f;
        private float speed = 1.0f;
        private bool isEarrape = false;
        private float volume = 1;

        private float appliedPitch, appliedSpeed, appliedVolume;

        #endregion status fields

        #region status propertys

        /// <summary>
        /// Volume property
        /// </summary>
        /// <remarks>
        /// 1.0 is 100%, 10.0 is static noise
        /// </remarks>
        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                ConfigChanged(Pitch, value, Speed);
            }
        }

        /// <summary>
        /// Pitch property
        /// </summary>
        /// <remarks>
        /// 0.0 is default and will not change pitch
        /// </remarks>
        public float Pitch
        {
            get => pitch;
            set
            {
                pitch = value;
                ConfigChanged(value, Volume, Speed);
            }
        }


        /// <summary>
        /// Sets the replay-Speed of the bot
        /// </summary>
        /// <remarks>
        /// No effect on Throughput-devices like Microphone or VCable
        /// </remarks>
        public float Speed
        {
            get => speed;
            set
            {
                speed = value;
                ConfigChanged(Pitch, Volume, value);
            }
        }

        /// <summary>
        /// IsServerConnected property
        /// </summary>
        public bool IsServerConnected { get; private set; }

        /// <summary>
        /// IsChannelConnected property
        /// </summary>
        public bool IsChannelConnected
        {
            get
            {
                //check if client is timed out
                if (Client.ConnectionState != ConnectionState.Connected && isChannelConnected)
                {
                    isChannelConnected = false;
                }

                return isChannelConnected;
            }
            private set => isChannelConnected = value;
        }

        /// <summary>
        /// IsStreaming property, calls StreamStateChanged delegate
        /// </summary>
        public bool IsStreaming
        {
            get => isStreaming;
            private set
            {
                if (value != isStreaming)
                {
                    isStreaming = value;
                    StreamStateChanged(isStreaming, currentSong);
                }
            }
        }

        /// <summary>
        /// CurrentTime property
        /// </summary>
        public TimeSpan CurrentTime
        {
            get
            {
                if (Wave.Reader != null) return Wave.Reader.CurrentTime;
                else return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// TitleLength property
        /// </summary>
        public TimeSpan TitleLenght
        {
            get
            {
                if (Wave.Reader != null) return Wave.Reader.TotalTime;
                else return TimeSpan.Zero;
            }
        }


        /// <summary>
        /// IsBufferEmpty property
        /// </summary>
        public bool IsBufferEmpty { get; private set; }


        /// <summary>
        /// IsLoop property
        /// </summary>
        public bool IsLoop { get; set; } = false;

        /// <summary>
        /// is set if the bot is paused
        /// </summary>
        /// <remarks>unlocks it self, but must be set from outside</remarks>
        public bool IsPause { get; set; } = false;

        /// <summary>
        /// IsToAbort property
        /// </summary>
        private bool IsToAbort { get; set; } = false;

        private uint SkipTracks { get; set; }

        /// <summary>
        /// IsEarrape property
        /// </summary>
        public bool IsEarrape
        {
            get => isEarrape;
            set
            {
                isEarrape = value;
                //method sets Volume to 100, when isEarrape
                ConfigChanged(Pitch, Volume, Speed);
            }
        }

        private bool CanSeek { get; set; } = true;

        #endregion status propertys

        #region other vars

        private DiscordSocketClient Client { get; set; }
        private IAudioClient AudioCl { get; set; }
        private AudioOutStream OutStream { get; set; } = null;

        private BotWave Wave { get; set; } = new BotWave();

        #endregion other vars

        /// <summary>
        /// constructor inits important properties
        /// </summary>
        public Bot()
        {
            IsStreaming = false;
            IsChannelConnected = false;
            IsServerConnected = false;
            IsBufferEmpty = true;

            appliedPitch = pitch;
            appliedSpeed = speed;
            appliedVolume = volume;
        }

        #region controll stuff

        /// <summary>
        /// enqueues a btn into the queue, if queue is empy directly gather stream
        /// </summary>
        /// <param name="data"></param>
        protected async Task loadFileAsync(BotData data)
        {
            if (IsStreaming)
                await stopStreamAsync(true, false);

            getStream(data);
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


        private void ConfigChanged(float pitch, float volume, float speed, bool isForced=false)
        {
            //apply given parameters to all readers
            if (Wave.SourceResampler != null && Wave.ActiveResampler != null)
            {
                if (Wave.Touch != null)

                {
                    if (speed != appliedSpeed || isForced)
                    {
                        appliedSpeed = speed;
                        Wave.Touch.Tempo = appliedSpeed;
                    }

                    if (pitch != appliedPitch || isForced)
                    {
                        appliedPitch = pitch;
                        Wave.Touch.Pitch = appliedPitch;
                    }
                }


                if (Wave.Volume != null && (volume != appliedVolume || IsEarrape || isForced))
                {
                    if (isEarrape)
                        appliedVolume = 100;
                    else
                        appliedVolume = volume;

                    Wave.Volume.Volume = appliedVolume;
                }
            }
        }


        /// <summary>
        /// skips ahead to a timespan
        /// </summary>
        /// <param name="newTime">new Time</param>
        /// <param name="enforce">enforce the skip, even if nothing is playing</param>
        public void skipToTime(TimeSpan newTime, bool enforce = false)
        {
            if ((IsStreaming || enforce) && CanSeek)
            {
                Wave.Reader.CurrentTime = newTime;
            }
        }

        /// <summary>
        /// skip over a time period
        /// </summary>
        /// <param name="skipTime">timeSpan to skip over</param>
        public void skipOverTime(TimeSpan skipTime)
        {
            if (IsStreaming && CanSeek)
            {
                Wave.Reader.CurrentTime = Wave.Reader.CurrentTime.Add(skipTime);
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
                throw new BotException(BotException.type.connection,
                    "Not connected to the Servers, while Setting GameState", BotException.connectionError.NoServer);

            ActivityType type = ActivityType.Watching;
            if (isStreaming)
                type = ActivityType.Streaming;

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
            Wave.Reader = null;
            Wave.Capture = null;

            //see if file or uri was provided
            if (!string.IsNullOrWhiteSpace(data.uri))
            {
                Wave.Reader = new MediaFoundationReader(data.uri);
                CanSeek = Wave.Reader.CanSeek;
                //Wave.Capture = null;
            }
            else if (File.Exists(data.filePath))
            {
                Wave.Reader = new MediaFoundationReader(data.filePath);
                CanSeek = Wave.Reader.CanSeek;
                //Wave.Capture = null;
            }
            else if (!string.IsNullOrEmpty(data.deviceId))
            {
                var enumerator = new MMDeviceEnumerator();

                if (data.deviceId == "-1")
                {
                    //get earrape stream
                    MMDevice device = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);

                    Wave.Capture = new WasapiCapture(device);
                    Wave.Capture.WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(48000, 2);
                }
                else
                {
                    //device stream
                    MMDevice device = enumerator.GetDevice(data.deviceId);

                    Wave.Capture = new WasapiCapture(device);

                    ////if channels/sample count is not default, resample it later
                    if (Wave.Capture.WaveFormat.Channels != Wave.OutFormat.Channels ||
                        Wave.Capture.WaveFormat.SampleRate != Wave.OutFormat.SampleRate)
                    {
                        //send delegate to bot Handler
                        IncompatibleWave();
                    }

                    //convert from ieee to pcm
                    Wave.Capture.WaveFormat = new WaveFormat(Wave.Capture.WaveFormat.SampleRate, BotWave.bitDepth,
                        Wave.Capture.WaveFormat.Channels);
                }

                CanSeek = false;

                Wave.ActiveResampler = null;
                Wave.SourceResampler = null;

                Wave.Capture.DataAvailable += Capture_DataAvailable;

                SpeedAvailableChanged(false);
                //no override settings for stream available
            }
            else
                return;


            IsBufferEmpty = false;

            //set name of loaded song
            currentSong = data.name;

            if (Wave.Reader != null)
            {
                //create source and finally used resampler
                Wave.SourceResampler = new MediaFoundationResampler(Wave.Reader, Wave.OutFormat);

                //create additional providers for Volume, Speed and Pitch

                Wave.Volume = new VolumeWaveProvider16(Wave.SourceResampler);
                Wave.Touch = new NAudio.SoundTouch.SoundTouchWaveStream(Wave.Volume);
                //Wave.Pitch = new SmbPitchShiftingSampleProvider(Wave.Speed.ToSampleProvider());

                Wave.ActiveResampler = new MediaFoundationResampler(Wave.Touch, Wave.OutFormat);


                //will apply Earrape and loop
                loadOverrideSettings(data);


                //apply pitch and volume to the resampler, will also set NormalResampler
                ConfigChanged(Pitch, Volume, Speed, true);

                SpeedAvailableChanged(true);
            }
        }


        //receives Data from requested device
        private void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            var buff = e.Buffer;

            applyVolume(ref buff);

            OutStream.Write(buff, 0, e.BytesRecorded);
        }


        /// <summary>
        /// starts the stream
        /// </summary>
        /// <returns>Task</returns>
        /// <remarks>
        /// calls itself again as long as isLoop is true
        /// </remarks>
        private async Task startStreamAsync()
        {
            //IsChannelConnected gaurantees, to have IsServerConnected
            if (!IsChannelConnected)
            {
                if (!IsServerConnected)
                    throw new BotException(BotException.type.connection,
                        "Not connected to Server, while trying to start stream", BotException.connectionError.NoServer);
                else
                    throw new BotException(BotException.type.connection,
                        "Not connected to a channel, while trying to start stream",
                        BotException.connectionError.NoChannel);
            }

            if (!IsStreaming && IsServerConnected && AudioCl != null)
            {
                if (OutStream == null)
                    OutStream = AudioCl.CreatePCMStream(AudioApplication.Music);


                if (Wave.ActiveResampler == null)
                {
                    //streaming device streams
                    if (Wave.Capture != null)
                    {
                        //earrape, pitch is not available in first implementation
                        IsStreaming = true;
                        Wave.Capture.StartRecording();
                    }

                    return;
                }

                //streaming local or uri streams
                IsStreaming = true;
                //send stream in small packages
                int blockSize = Wave.OutFormat.AverageBytesPerSecond / packagesPerSecond;
                byte[] buffer = new byte[blockSize];


                IsPause = false;
                int byteCount;

                while ((byteCount = Wave.ActiveResampler.Read(buffer, 0, blockSize)) > 0)
                    //repeat, read new block into buffer -> stream buffer
                    //while (Wave.Reader.Position < Wave.Reader.Length)
                {
                    //int byteCount = Wave.ActiveResampler.Read(buffer, 0, blockSize);

                    if (IsToAbort || SkipTracks > 0)
                        break;

                    if (byteCount < blockSize)
                    {
                        //fill rest of stream with '0'
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                        IsBufferEmpty = true;
                    }

                    await OutStream.WriteAsync(buffer, 0, blockSize);
                }


                IsStreaming = false;

                //reopen the same file
                if (IsLoop && !IsToAbort && SkipTracks == 0)
                {
                    //move head to begin of file
                    skipToTime(TimeSpan.Zero, true);
                    await startStreamAsync();
                }
                //exit stream
                else
                {
                    //can't skip a track if nothing is running
                    SkipTracks = 0;

                    if (OutStream != null)
                    {
                        await OutStream.FlushAsync();
                        OutStream.Close();
                        OutStream = null;
                    }

                    IsToAbort = false;

                    //trigger end of file delegate, needed e.g. for playlist processing
                    if (!IsPause)
                        EndOfFile();
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
            var vol = Volume;

            if (IsEarrape)
                vol = 22;

            if (vol == 1)
                return;

            for (int i = 0; i < buffer.Length; i += 2)
            {
                //convert a byte-Pair into one char (with 2 bytes)

                short bytePair = (short) ((buffer[i + 1] & 0xFF) << 8 | (buffer[i] & 0xFF));

                bytePair = (short) (bytePair * vol);

                //convert char back to 2 bytes
                buffer[i] = (byte) bytePair;
                buffer[i + 1] = (byte) (bytePair >> 8);
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
                throw new BotException(BotException.type.connection,
                    "Not connected to the servers, while trying to connect to a channel",
                    BotException.connectionError.NoServer);
            }

            AudioCl = await ((ISocketAudioChannel) Client.GetChannel(channelId)).ConnectAsync(true);

            IsChannelConnected = true;
        }

        #endregion start stuff

        #region stop stuff

        /// <summary>
        /// stop running streams and disconnect from channel
        /// </summary>
        /// <returns>Task</returns>
        /// <see cref="stopStreamAsync(bool, bool)"/>
        public async Task disconnectFromChannelAsync()
        {
            if (!IsChannelConnected)
                return;

            await stopStreamAsync(false, true);

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
                await Task.Delay(5);

            await Client.StopAsync();
            await Client.LogoutAsync();

            IsServerConnected = false;
        }

        /// <summary>
        /// stop a running stream
        /// </summary>
        /// <param name="flushStream">flushes the current stream</param>
        /// <param name="closeStream">closes the current stream</param>
        /// <returns>Task</returns>
        public async Task stopStreamAsync(bool flushStream, bool closeStream)
        {
            if (!IsStreaming)
                return;


            //streaming from device
            if (Wave.Capture != null)
            {
                Wave.Capture.StopRecording();


                IsStreaming = false;

                if (!IsPause)
                {
                    Wave.Capture.DataAvailable -= Capture_DataAvailable;
                    EndOfFile();
                }
            }
            //streaming from buffer
            else
            {
                IsToAbort = true;

                //wait until last package is read in
                while (IsStreaming)
                    await Task.Delay(5);
            }


            if (OutStream != null)
            {
                if (flushStream)
                    await OutStream.FlushAsync();

                if (closeStream)
                {
                    OutStream.Close();
                    OutStream = null;
                }
            }


            //make sure to not block future streams
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
                throw new BotException(BotException.type.connection,
                    "Not connected to the servers, while trying to get channel list",
                    BotException.connectionError.NoServer);

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
        /// <returns>list of all servers, each contains a list of all clients, regarding acceptOffline. Returns null if Client is still Connecting</returns>
        protected List<List<SocketGuildUser>> getAllClients(bool acceptOffline)
        {
            if (!IsServerConnected)
                throw new BotException(BotException.type.connection,
                    "Not connected to the servers, while trying to get clint list",
                    BotException.connectionError.NoServer);

            List<List<SocketGuildUser>> guildList = new List<List<SocketGuildUser>>();

            if (Client.ConnectionState == ConnectionState.Connecting)
                return null;


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