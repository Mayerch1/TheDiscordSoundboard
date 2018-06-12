using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using NAudio.Wave;

using Discord;
using Discord.WebSocket;
using Discord.Audio;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Collections.Generic;

namespace DiscordBot
{
    //TODO: push

    public partial class ButtonForm : Form
    {
        #region Variables

        /*-----------------------------------------------------------*/

        //saved vars
        //!!Important!!
        //changing this will prevent you from loading the config
        //if you want more buttons (or if you want to safe disk space), you'll need to
        //generate a new config, then copy (remove) button-entries by hand
        private const int _maxBtnNum = 2000;

        //state vars
        public DiscordSocketClient _client;

        public IAudioClient _audioCl;
        public bool _isServerConnected = false;
        private MediaFoundationResampler _resampler;

        private WaveFormat _OutFormat = new WaveFormat(48000, 16, 2);

        public bool _isAudioConnected = false;
        public bool _isPlaying = false;
        public bool _isToAbort = false;
        public bool _isLooped = false;
        public int _fastForward = 0;

        //safe and safe-reladet vars
        private Data data = new Data();

        //buttons
        private ButtonData[] btn = new ButtonData[_maxBtnNum];

        /*---------------------------------------------------------*/

        #endregion Variables

        #region SaveAndLoadStuff

        public ButtonForm()
        {
            InitializeComponent();
            createTooltips();

            //load from file
            ButtonData[] loadingBtn = LoadAllSettings();

            //dropDown list
            loadFavoriteDrop();
            loadMessageDrop();

            btnNumBox.Maximum = _maxBtnNum;

            if (loadingBtn != null)
                btn = loadingBtn;

            //generate standart Btn
            if (loadingBtn == null)
            {
                btn = new ButtonData[_maxBtnNum];

                for (int i = 0; i < _maxBtnNum; i++)
                {
                    btn[i] = new ButtonData();
                    initBtnSettings(ref btn[i]);
                }
            }

            //create all Buttons
            for (int i = 0; i < data.btnNum; i++)
            {
                addBtnToLayout(i, btn[i]);
            }

            if (data.isAutoconnect)
                //dirty, but effective
                connectBtn_Click(null, null);
        }

        private void createTooltips()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(channelIdBox, "Sets the channel to join the next time");
            toolTip.SetToolTip(channelIdLbl, "Sets the channel to join the next time");
            toolTip.SetToolTip(volumeSlider, "Sets the Volume between 0.0 and 1.0");
            toolTip.SetToolTip(connectBtn, "Connect the bot to the server");
            toolTip.SetToolTip(earRapeBox, "If you just need that extra-boost for your audio :)");
            toolTip.SetToolTip(abortBtn, "If the bot is playing, stop it");
            toolTip.SetToolTip(leaveMsgBox, "Only visible for a few seconds");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_isServerConnected && _isAudioConnected)
                disconnectFromServer();
            SaveSettings(btn);
        }

        private void initBtnSettings(ref ButtonData btn)
        {
            //set to default values
            btn.Text = " ";
            btn.file = data.placeholderFile;

            btn.settingsOpened = false;
        }

        private void addBtnToLayout(int num, ButtonData thisBtn)
        {
            Button button = new Button();
            button.Tag = num;
            button.Height = 40;
            button.Width = 95;
            button.Click += new EventHandler(btn_Click);
            button.MouseDown += new MouseEventHandler(btn_MouseDown);

            flowLayout.Controls.Add(button);

            button.Text = thisBtn.Text;
        }

        private void SaveSettings(ButtonData[] btnData)
        {
            for (int i = 0; i < _maxBtnNum; i++)
            {
                btnData[i].settingsOpened = false;
            }

            Stream stream;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            //save token as binary
            try
            {
                stream = File.Open(data.tokenFile, FileMode.Create);
                binaryFormatter.Serialize(stream, data.token);
                stream.Close();
            }
            catch { }

            //write settings as text
            StreamWriter file = File.CreateText(data.settingsFile);
            file.WriteLine(true);
            file.WriteLine(Data.safeVersion);
            file.WriteLine(data.activeChannelID);
            file.WriteLine(data.isAutoconnect);
            file.WriteLine(data.btnNum);

            file.WriteLine(volumeSlider.Value);

            file.WriteLine(data.joinMsg);
            file.WriteLine(data.url);
            file.WriteLine(data.isStreaming);
            file.WriteLine(data.leaveMsg);

            //favs
            file.WriteLine(data.favorites.Count);
            foreach (var element in data.favorites)
            {
                file.WriteLine(element.ToString());
            }
            //messages
            file.WriteLine(data.statusMessages.Count);
            foreach (var element in data.statusMessages)
            {
                file.WriteLine(element.ToString());
            }
            file.WriteLine(data.isBot);

            file.Flush();

            file = File.CreateText(data.buttonFile);
            //to recognise new format
            //write buttons as text
            foreach (var singleBtn in btn)
            {
                file.WriteLine(singleBtn.ToString());
            }

            file.Flush();
            file.Close();
        }

        private ButtonData[] LoadAllSettings()
        {
            Stream stream;
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            //load token from binary stream
            try
            {
                stream = File.Open(data.tokenFile, FileMode.Open);
                data.token = (string)binaryFormatter.Deserialize(stream);
                stream.Close();
            }
            catch
            {
                tokenButton.PerformClick();
            }

            //loud other settings from file
            if (!loadConfig())
            {
                //if old filetype
                return loadOldSettingsFormat();
            }

            //load buttons from file
            StreamReader file;
            try
            {
                file = File.OpenText(data.buttonFile);
            }
            catch
            {
                return null;
            }

            ButtonData[] loadingBtn = new ButtonData[_maxBtnNum];
            for (int i = 0; i < _maxBtnNum; i++)
            {
                loadingBtn[i] = new ButtonData();
                loadingBtn[i].parse(file.ReadLine());
            }

            file.Close();
            return loadingBtn;
        }

        //return false, if old datatype was found
        private bool loadConfig()
        {
            StreamReader file;
            try
            {
                file = File.OpenText(data.settingsFile);
            }
            catch
            {
                return true;
            }
            //if old format, quit
            if (!bool.TryParse(file.ReadLine(), out bool isNewFormat) || !isNewFormat)
            {
                file.Close();
                return false;
            }

            int dataVer = int.Parse(file.ReadLine());

            //load only version 1 or better
            if (dataVer >= 1)
            {
                data.activeChannelID = ulong.Parse(file.ReadLine());
                channelIdBox.Text = data.activeChannelID.ToString();
                data.isAutoconnect = bool.Parse(file.ReadLine());
                autoConnectBox.Checked = data.isAutoconnect;

                //btn to display
                data.btnNum = int.Parse(file.ReadLine());
                if (btnNumBox.Value <= _maxBtnNum)
                    btnNumBox.Value = data.btnNum;

                data.volume = int.Parse(file.ReadLine());
                volumeSlider.Value = data.volume;

                //read messages
                data.joinMsg = file.ReadLine();
                data.url = file.ReadLine();
                data.isStreaming = bool.Parse(file.ReadLine());
                data.leaveMsg = file.ReadLine();

                joinMsgBox.Text = data.joinMsg;
                urlBox.Text = data.url;
                streamBox.Checked = data.isStreaming;
                leaveMsgBox.Text = data.leaveMsg;

                //read favorite channetId's
                int favLen = int.Parse(file.ReadLine());
                for (int i = 0; i < favLen; i++)
                {
                    ChannelFavorite item = new ChannelFavorite(file.ReadLine());
                    data.favorites.Add(item);
                }

                int msgLen = int.Parse(file.ReadLine());
                for (int i = 0; i < msgLen; i++)
                {
                    StatusMessage item = new StatusMessage(file.ReadLine());
                    data.statusMessages.Add(item);
                }
            }

            if (dataVer >= 2)
            {
                data.isBot = bool.Parse(file.ReadLine());
            }
            else
            {
                data.isBot = true;
            }

            //newer versions might load additional data
            //in 'else', defaults for old vers. will be set

            file.Close();
            return true;
        }

        private ButtonData[] loadOldSettingsFormat()
        {
            //load rest from text file
            StreamReader file;
            try
            {
                file = File.OpenText(data.settingsFile);
            }
            catch
            {
                return null;
            }

            data.activeChannelID = ulong.Parse(file.ReadLine());
            data.isAutoconnect = bool.Parse(file.ReadLine());

            //btn to display
            data.btnNum = int.Parse(file.ReadLine());

            ButtonData[] loadingBtn = new ButtonData[_maxBtnNum];
            for (int i = 0; i < _maxBtnNum; i++)
            {
                loadingBtn[i] = new ButtonData();
                loadingBtn[i].parse(file.ReadLine());
            }

            file.Close();
            return loadingBtn;
        }

        private void loadFavoriteDrop()
        {
            channelIdBox.Items.Clear();
            string[] comboList = new string[data.favorites.Count];

            for (int i = 0; i < data.favorites.Count; i++)
            {
                comboList[i] = data.favorites[i].name;
            }
            channelIdBox.Items.AddRange(comboList);
        }

        private void loadMessageDrop()
        {
            joinMsgBox.Items.Clear();
            string[] comboList = new string[data.statusMessages.Count];

            for (int i = 0; i < data.statusMessages.Count; i++)
            {
                comboList[i] = data.statusMessages[i].name;
            }
            joinMsgBox.Items.AddRange(comboList);
        }

        #endregion SaveAndLoadStuff

        #region ConnectToServerAndAudio

        private async void connectToServer()
        {
            _client = new DiscordSocketClient();
            TokenType myToken;

            if (data.isBot)
                myToken = TokenType.Bot;
            else
                myToken = TokenType.User;
            try
            {
                await _client.LoginAsync(myToken, data.token);
            }
            catch
            {
                showTokenError("Can't login. Check Token");
                return;
            }

            await _client.StartAsync();

            await setGameState(data.joinMsg, data.url, data.isStreaming);

            _isServerConnected = true;
            tokenButton.Enabled = false;
            connectBtn.Text = "Disconnect";
        }

        private async Task<IAudioClient> connectToChannel()
        {
            data.activeChannelID = getActiveChannelId();
            if (data.activeChannelID == 0)
                return null;
            try
            {
                IAudioClient tmpAudio = await ((ISocketAudioChannel)_client.GetChannel(data.activeChannelID)).ConnectAsync();
                _isAudioConnected = true;
                return tmpAudio;
            }
            catch
            {
                //do nothing, _isAudioConnectes stays false
                return null;
            }
        }

        private ulong getActiveChannelId()
        {
            string select = channelIdBox.Text;

            if (ulong.TryParse(select, out ulong id))
                return id;

            for (int i = 0; i < data.favorites.Count; i++)
            {
                if (data.favorites[i].name == select)
                    return data.favorites[i].id;
            }
            return 0;
        }

        #endregion ConnectToServerAndAudio

        private async void disconnectFromServer()
        {
            abortBtn.PerformClick();

            await _client.SetGameAsync(data.leaveMsg);

            while (_isAudioConnected)
            {/*wait until stream buffer is empty*/
                await Task.Delay(10);
            }

            await _client.StopAsync();
            await _client.LogoutAsync();

            _isServerConnected = false;

            connectBtn.Text = "Connect";
            tokenButton.Enabled = true;
        }

        private async void connectPlayAudioFile(ButtonData btn, AudioOutStream stream = null)
        {
            //check for existing audioConnection
            if (!_isAudioConnected)
                _audioCl = await connectToChannel();

            if (_isPlaying)
            {
                abortBtn.PerformClick();
                return;
            }

            //check for existing serverConnectitn
            if (_isServerConnected && !_isPlaying && _audioCl != null)
            {
                if (stream == null)
                    stream = _audioCl.CreatePCMStream(AudioApplication.Music);

                _resampler = null;

                int channelCount = 2;
                var OutFormat = new WaveFormat(48000, 16, channelCount);

                //choose between URl and local file
                if (btn.file.StartsWith("http"))
                {
                    _resampler = await getStreamStream(btn, OutFormat);
                }
                else
                    _resampler = getFileStream(btn, OutFormat);

                if (_resampler == null)
                {
                    _isToAbort = false;
                    stream.Close();
                    await _audioCl.StopAsync();
                    _isAudioConnected = false;
                    return;
                }

                volumeSlider.Enabled = false;
                if (earRapeBox.Checked)
                    earRapeBox.Enabled = false;

                //load and play audio file
                int blockSize = OutFormat.AverageBytesPerSecond / 50;
                byte[] buffer = new byte[blockSize];
                int byteCount;

                _isPlaying = true;

                //start buffering
                while ((byteCount = _resampler.Read(buffer, 0, blockSize)) > 0)
                {
                    //check each loop for abort
                    if (_isToAbort)
                        break;

                    //for uneven endings, fill with 0
                    if (byteCount < blockSize)
                    {
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }
                    //write to binary stream

                    if (_fastForward > 0)
                    {
                        _fastForward--;
                        continue;
                    }
                    await stream.WriteAsync(buffer, 0, blockSize);
                }
                _isPlaying = false;
                volumeSlider.Enabled = earRapeBox.Enabled = true;

                // disconnect
                if (!_isLooped || _isToAbort)
                {
                    //wait, so last package can be played
                    await Task.Delay(1250);
                    _isToAbort = false;

                    stream.Close();
                    await _audioCl.StopAsync();

                    _isAudioConnected = false;
                }
                //play loop
                else
                {
                    //keep stream open, for gap-less looping
                    connectPlayAudioFile(btn, stream);
                }
            }
        }

        private async Task<MediaFoundationResampler> getStreamStream(ButtonData btn, WaveFormat waveFormat)
        {
            //maybe, but only maybe in the far future, like really really far
            return null;
        }

        private MediaFoundationResampler getFileStream(ButtonData btn, WaveFormat OutFormat)
        {
            Mp3FileReader MP3Reader;
            WaveFileReader WavReader;

            MediaFoundationResampler resampler;
            VolumeWaveProvider16 volume;

            //choose between wav and mp3
            if (btn.file == "")
                return null;

            if (btn.file[btn.file.Length - 1] == 'v')//*.waV
            {
                try
                {
                    WavReader = new WaveFileReader(btn.file);
                }
                catch
                {
                    return null;
                }
                volume = new VolumeWaveProvider16(WavReader);
            }
            else if (btn.file[btn.file.Length - 1] == '3')
            {
                try
                {
                    MP3Reader = new Mp3FileReader(btn.file);
                }
                catch
                {
                    return null;
                }
                volume = new VolumeWaveProvider16(MP3Reader);
            }
            else
            {
                return null;
            }

            float volumeLever = volumeSlider.Value / 200.0f;
            volume.Volume = volumeLever;

            if (earRapeBox.Checked)
                volume.Volume = 100;
            //converts the incoming wav/mp3 into opus compatible format

            resampler = new MediaFoundationResampler(volume, OutFormat);
            resampler.ResamplerQuality = 60;

            return resampler;
        }

        private void changePlayingVolume()
        {
            if (_isPlaying && _resampler != null)
            {
                //needed volume providers
                VolumeWaveProvider16 vol;
                //get from current stream
                vol = new VolumeWaveProvider16(_resampler);

                if (earRapeBox.Checked)
                {
                    earRapeBox.Enabled = false;
                    vol.Volume = 100;
                    _resampler = new MediaFoundationResampler(vol, _OutFormat);
                }
            }
        }

        private void openButtonSettings(ref ButtonData btn)
        {
            if (btn.settingsOpened)
                return;

            btn.settingsOpened = true;
            btn = getSettings(btn);
            btn.settingsOpened = false;
        }

        private ButtonData getSettings(ButtonData btn)
        {
            //open settings window, return new ButtonData Class
            BtnSettings settingsWindow = new BtnSettings(btn);
            settingsWindow.Location = this.Location;
            settingsWindow.ShowDialog();
            return settingsWindow.btn_n;
        }

        private void showTokenError(string msg = "")
        {
            MessageBox.Show(msg, "Authorization error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async Task<bool> setGameState(string msg, string url, bool stream)
        {
            //url must be a valid url
            StreamType type;
            if (stream)
                type = StreamType.Twitch;
            else
                type = StreamType.NotStreaming;

            try
            {
                if (url != "")
                    await _client.SetGameAsync(msg, url, type);
                else
                    await _client.SetGameAsync(msg);
            }
            catch
            {
                return false;
            }

            return true;
        }

        #region ButtonEvents

        private void btn_Click(object sender, EventArgs e)
        {
            //start playing routine
            Button button = (Button)sender;

            connectPlayAudioFile(btn[int.Parse(button.Tag.ToString())]);
        }

        //private void playButton_Click(object sender, EventArgs e)
        //{
        //    ButtonData newBtn = new ButtonData();
        //    newBtn.settingsOpened = false;
        //    newBtn.Text = "myUrl";
        //    newBtn.file = urlBox.Text;

        //    connectPlayAudioFile(newBtn);
        //}

        private void btn_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            //open settings, and refresh button label
            Button button = (Button)sender;
            openButtonSettings(ref btn[int.Parse(button.Tag.ToString())]);
            button.Text = btn[int.Parse(button.Tag.ToString())].Text;
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            if (!_isServerConnected)
            {
                if (data.token == "" || data.token == " ")
                    showTokenError("Token is empty");
                connectToServer();
            }
            else
            {
                disconnectFromServer();
            }
        }

        private void connectBtn_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextPos = this.Location;
                contextPos.X += connectBtn.Location.X;
                contextPos.Y += connectBtn.Location.Y;

                connectContextEntry.Checked = data.isAutoconnect;
                connectContext.Show(contextPos);
            }
        }

        private void abortBtn_Click(object sender, EventArgs e)
        {
            if (_isPlaying)
                _isToAbort = true;
        }

        private void channelIdBox_TextChanged(object sender, EventArgs e)
        {
            if (ulong.TryParse(channelIdBox.Text, out ulong result))
                data.activeChannelID = result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //open window to enter/view new token
            TokenWindow token = new TokenWindow(data.token, data.isBot);
            token.Location = this.Location;
            token.ShowDialog();
            data.token = token.token;
            data.isBot = token.isBot;
        }

        private void favoriteBtn_Click(object sender, EventArgs e)
        {
            channelIDFavorite channelWindow = new channelIDFavorite(data.favorites);
            channelWindow.Location = this.Location;
            channelWindow.ShowDialog();
            data.favorites = channelWindow.favorites;
            loadFavoriteDrop();
        }

        private void presetBtn_Click(object sender, EventArgs e)
        {
            msgPresetWindow msgPreset = new msgPresetWindow(data.statusMessages);
            msgPreset.Location = this.Location;
            msgPreset.ShowDialog();
            data.statusMessages = msgPreset.statusMessages;
            loadMessageDrop();
        }

        private void loopBox_CheckedChanged(object sender, EventArgs e)
        {
            _isLooped = loopBox.Checked;
        }

        private void btnNumBox_ValueChanged(object sender, EventArgs e)
        {
            if (btnNumBox.Value > data.btnNum)
            {
                for (int i = data.btnNum; i < btnNumBox.Value; i++)
                {
                    addBtnToLayout(i, btn[i]);
                }
                data.btnNum = (int)btnNumBox.Value;
            }
            else if (btnNumBox.Value < data.btnNum)
            {
                for (int i = data.btnNum - 1; i >= btnNumBox.Value; i--)
                {
                    flowLayout.Controls.RemoveAt(i);
                }
                data.btnNum = (int)btnNumBox.Value;
            }
        }

        private void donateBtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.paypal.me/CJMayer/3,99"));
        }

        private void connectContextEntry_Click(object sender, EventArgs e)
        {
            data.isAutoconnect = !data.isAutoconnect;
            autoConnectBox.Checked = data.isAutoconnect;
        }

        private void autoconnectBox_CheckedChanged(object sender, EventArgs e)
        {
            data.isAutoconnect = autoConnectBox.Checked;
        }

        private void leaveMsgBox_TextChanged(object sender, EventArgs e)
        {
            data.leaveMsg = leaveMsgBox.Text;
        }

        private async void joinMsgBtn_Click(object sender, EventArgs e)
        {
            await setGameState(data.joinMsg, data.url, data.isStreaming);
        }

        private void joinMsgBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string select = joinMsgBox.Text;

            for (int i = 0; i < data.statusMessages.Count; i++)
            {
                if (data.statusMessages[i].name == select)
                {
                    data.url = data.statusMessages[i].url;
                    urlBox.Text = data.statusMessages[i].url;
                }
            }
            data.joinMsg = joinMsgBox.Text;
        }

        private void streamBox_CheckedChanged(object sender, EventArgs e)
        {
            data.isStreaming = streamBox.Checked;
        }

        private void urlBox_TextChanged(object sender, EventArgs e)
        {
            data.url = urlBox.Text;
        }

        private void earRapeBox_CheckedChanged(object sender, EventArgs e)
        {
            changePlayingVolume();
        }

        #endregion ButtonEvents

        private void forwardBtn_Click(object sender, EventArgs e)
        {
            //TODO: adjust value
            if (_isPlaying)
                _fastForward += 50;
        }
    }

    //all config data,
    public class Data
    {
        public const int safeVersion = 2;

        public int btnNum = 40;
        public ulong activeChannelID = 409439103965462528;
        public bool isAutoconnect = false;
        public int volume = 25;

        public string joinMsg = "I'm ready";
        public bool isStreaming = false;
        public string url = "";

        public bool isBot = true;

        public string leaveMsg = "Mission Completed";

        /*-----------------------------------------------------------*/

        //if nothing is assigned to button
        public string placeholderFile = @"F:\Christian\Music\Soundboard\Airhorn.mp3";

        //is also old button file
        public string settingsFile = "SoundboardConfig.cfg";

        public string buttonFile = "ButtonConfig.cfg";
        public string tokenFile = "sensitiveToken.cfg";

        public List<ChannelFavorite> favorites = new List<ChannelFavorite>();
        public List<StatusMessage> statusMessages = new List<StatusMessage>();
        /*---------------------------------------------------------*/

        //SENSITIVE: remember to remove before push
        //hardcode token here, don't need to safe it in the filesystem
        public string token = "";

        /*----------------------------------------------------------*/
    }

    public struct ChannelFavorite
    {
        public ulong id;
        public string name;

        public ChannelFavorite(string s)
        {
            var seek = s.IndexOf(';');
            id = ulong.Parse(s.Substring(0, seek));
            name = s.Substring(seek + 1);
        }

        public override string ToString()
        {
            return id + ";" + name;
        }
    }

    public struct StatusMessage
    {
        public string name, url;

        public StatusMessage(string s)
        {
            var seek = s.IndexOf(';');
            int nameLen = int.Parse(s.Substring(0, seek));

            var subStr = s.Substring(seek + 1);

            name = subStr.Substring(0, nameLen);
            url = subStr.Substring(nameLen);
        }

        public override string ToString()
        {
            return name.Length + ";" + name + url;
        }
    }

    public class ButtonData
    {
        public override string ToString()
        {
            //settingsOpened must be false on saving
            string tmp = Text.Length.ToString() + ";" + Text + file;
            return tmp;
        }

        public void parse(string toParse)
        {
            settingsOpened = false;
            var seek = toParse.IndexOf(';');
            var textLen = int.Parse(toParse.Substring(0, seek));

            var subStr = toParse.Substring(seek + 1);

            Text = subStr.Substring(0, textLen);
            file = subStr.Substring(textLen);
        }

        public bool settingsOpened;
        public string Text;
        public string file;
    }
}