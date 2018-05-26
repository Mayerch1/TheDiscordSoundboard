using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using NAudio.Wave;

using Discord;
using Discord.WebSocket;
using Discord.Audio;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DiscordBot
{
    public partial class Form1 : Form
    {
        #region Variables

        /*-----------------------------------------------------------*/

        //!!Important!!
        //changing this will prevent you from loading the config
        //if you want more buttons (or if you want to safe disk space), you'll need to
        //generate a new config, then copy (remove) button-entries by hand

        //saved vars
        private const int maxBtnNum = 2000;

        private int _btnNum = 40;
        private ulong _channelID = 409439103965462528;
        private bool _isAutoconnect = false;
        /*-----------------------------------------------------------*/

        private ButtonData[] btn = new ButtonData[maxBtnNum];

        //if nothing is assigned to button
        private string _placeholderFile = @"F:\Christian\Music\Soundboard\Airhorn.mp3";

        //btnSettings and tokenSave
        private string _settingsFile = "SoundboardConfig.cfg";

        private string _tokenFile = "sensitiveToken.cfg";

        /*---------------------------------------------------------*/

        private DiscordSocketClient _client;
        private IAudioClient _audioCl;

        //SENSITIVE: remove before push

        //hardcode token here, don't need to safe it in the filesystem

        private string _token = "";

        /*----------------------------------------------------------*/

        //state vars
        private bool _isServerConnected = false;

        private bool _isAudioConnected = false;
        private bool _isPlaying = false;
        private bool _isToAbort = false;
        private bool _isLooped = false;
        /*---------------------------------------------------------*/

        #endregion Variables

        #region SaveAndLoadStuff

        public Form1()
        {
            InitializeComponent();
            createTooltips();

            //load from file
            ButtonData[] loadingBtn = LoadSettings();

            channelIdBox.Text = _channelID.ToString();

            btnNumBox.Maximum = maxBtnNum;
            if (btnNumBox.Value <= maxBtnNum)
                btnNumBox.Value = _btnNum;

            if (loadingBtn != null)
                btn = loadingBtn;

            //generate standart Btn
            if (loadingBtn == null)
            {
                btn = new ButtonData[maxBtnNum];

                for (int i = 0; i < maxBtnNum; i++)
                {
                    btn[i] = new ButtonData();
                    initBtnSettings(ref btn[i]);
                }
            }

            //create all Buttons
            for (int i = 0; i < _btnNum; i++)
            {
                addBtnToLayout(i, btn[i]);
            }

            if (_isAutoconnect)
                //dirty, but effective
                connectBtn_Click(null, null);
        }

        private void createTooltips()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnNumBox, "Sets the amount of shown buttons");
            toolTip.SetToolTip(channelIdBox, "Sets the channel to join the next time");
            toolTip.SetToolTip(channelIdLbl, "Sets the channel to join the next time");
            toolTip.SetToolTip(volumeSlider, "Sets the Volume between 0.0 and 1.0");
            toolTip.SetToolTip(connectBtn, "Connect the bot to the server");
            toolTip.SetToolTip(earRapeBox, "If you just need that extra-boost for your audio :)");
            toolTip.SetToolTip(abortBtn, "If the bot is playing, stop it");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings(btn);
        }

        private void initBtnSettings(ref ButtonData btn)
        {
            //set to default values
            btn.Text = " ";
            btn.file = _placeholderFile;

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
            for (int i = 0; i < maxBtnNum; i++)
            {
                btnData[i].settingsOpened = false;
            }

            Stream stream;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            //save token as binary
            try
            {
                stream = File.Open(_tokenFile, FileMode.Create);
                binaryFormatter.Serialize(stream, _token);
                stream.Close();
            }
            catch { }

            //write settings as text
            StreamWriter file = File.CreateText(_settingsFile);

            file.WriteLine(_channelID);
            file.WriteLine(_isAutoconnect);
            file.WriteLine(_btnNum);

            foreach (var singleBtn in btn)
            {
                file.WriteLine(singleBtn.ToString());
            }

            file.Flush();
            file.Close();
        }

        private ButtonData[] LoadSettings()
        {
            Stream stream;
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            //load from binary stream
            try
            {
                stream = File.Open(_tokenFile, FileMode.Open);
                _token = (string)binaryFormatter.Deserialize(stream);
                stream.Close();
            }
            catch
            {
                tokenButton.PerformClick();
            }

            //load rest from text file
            StreamReader file;
            try
            {
                file = File.OpenText(_settingsFile);
            }
            catch
            {
                return null;
            }

            _channelID = ulong.Parse(file.ReadLine());
            _isAutoconnect = bool.Parse(file.ReadLine());

            //for later use (dynamic Btn Count)
            _btnNum = int.Parse(file.ReadLine());

            ButtonData[] loadingBtn = new ButtonData[maxBtnNum];
            for (int i = 0; i < maxBtnNum; i++)
            {
                loadingBtn[i] = new ButtonData();
                loadingBtn[i].parse(file.ReadLine());
            }

            file.Close();
            return loadingBtn;
        }

        #endregion SaveAndLoadStuff

        #region ConnectToServerAndAudio

        private async void connectToServer()
        {
            _client = new DiscordSocketClient();

            try
            {
                await _client.LoginAsync(TokenType.Bot, _token);
            }
            catch
            {
                showTokenError("Can't login. Check Token");
                return;
            }

            await _client.StartAsync();
            await _client.SetGameAsync("Preparing to Earrape", "http://www.bdfm-clan.de", StreamType.NotStreaming);

            _isServerConnected = true;
            tokenButton.Enabled = false;
            connectBtn.Text = "Disconnect";
        }

        private async Task<IAudioClient> connectToChannel()
        {
            try
            {
                IAudioClient tmpAudio = await ((ISocketAudioChannel)_client.GetChannel(_channelID)).ConnectAsync();
                _isAudioConnected = true;
                return tmpAudio;
            }
            catch
            {
                //do nothing, _isAudioConnectes stays false
                return null;
            }
        }

        #endregion ConnectToServerAndAudio

        private async void disconnectFromServer()
        {
            abortBtn.PerformClick();

            await _client.SetGameAsync("Mission Completed", "http://www.bdfm-clan.de", StreamType.NotStreaming);

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

                MediaFoundationResampler resampler = null;

                int channelCount = 2;
                var OutFormat = new WaveFormat(48000, 16, channelCount);

                //choose between URl and local file
                if (btn.file.StartsWith("http"))
                {
                    resampler = await getStreamStream(btn, OutFormat);
                }
                else
                    resampler = getFileStream(btn, OutFormat);

                if (resampler == null)
                    return;

                volumeSlider.Enabled = earRapeBox.Enabled = false;

                //load and play audio file
                int blockSize = OutFormat.AverageBytesPerSecond / 50;
                byte[] buffer = new byte[blockSize];
                int byteCount;

                _isPlaying = true;

                //start buffering
                while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0)
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

                    await stream.WriteAsync(buffer, 0, blockSize);
                }

                _isPlaying = false;
                volumeSlider.Enabled = earRapeBox.Enabled = true;

                // disconnect
                if (!_isLooped || _isToAbort)
                {
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
            //maybe, but only maybe in the future
            return null;
        }

        private MediaFoundationResampler getFileStream(ButtonData btn, WaveFormat OutFormat)
        {
            Mp3FileReader MP3Reader;
            WaveFileReader WavReader;

            MediaFoundationResampler resampler;
            VolumeWaveProvider16 volume;

            //choose between wav and mp3
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
            else
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

            float volumeLever = volumeSlider.Value / 200.0f;
            volume.Volume = volumeLever;

            if (earRapeBox.Checked)
                volume.Volume = 100;
            //converts the incoming wav/mp3 into opus compatible format
            resampler = new MediaFoundationResampler(volume, OutFormat);
            resampler.ResamplerQuality = 60;
            return resampler;
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
                if (_token == "" || _token == " ")
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

                connectContextEntry.Checked = _isAutoconnect;
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
                _channelID = result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //open window to enter/view new token
            TokenWindow token = new TokenWindow(_token);
            token.Location = this.Location;
            token.ShowDialog();
            _token = token.token;
        }

        private void loopBox_CheckedChanged(object sender, EventArgs e)
        {
            _isLooped = loopBox.Checked;
        }

        private void btnNumBox_ValueChanged(object sender, EventArgs e)
        {
            if (btnNumBox.Value > _btnNum)
            {
                for (int i = _btnNum; i < btnNumBox.Value; i++)
                {
                    addBtnToLayout(i, btn[i]);
                }
                _btnNum = (int)btnNumBox.Value;
            }
            else if (btnNumBox.Value < _btnNum)
            {
                for (int i = _btnNum - 1; i >= btnNumBox.Value; i--)
                {
                    flowLayout.Controls.RemoveAt(i);
                }
                _btnNum = (int)btnNumBox.Value;
            }
        }

        #endregion ButtonEvents

        private void connectContextEntry_Click(object sender, EventArgs e)
        {
            _isAutoconnect = !_isAutoconnect;
            connectContextEntry.Checked = _isAutoconnect;
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