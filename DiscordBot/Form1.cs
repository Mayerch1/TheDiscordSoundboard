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
        private const int btnNum = 380;
        private ulong _placeholderID = 409439103965462528;
        /*-----------------------------------------------------------*/

        private ButtonData[] btn = new ButtonData[btnNum];

        //user to join
        private ulong _userId = 140149964020908032;

        //if nothing is assigned to button
        private string _placeholderFile = @"F:\Christian\Music\Alles_nur_Spass_Mix.mp3";

        //btnSettings and tokenSave
        private string _settingsFile = "Settings.cfg";

        private string _tokenFile = "sensitiveToken.cfg";

        /*---------------------------------------------------------*/

        private DiscordSocketClient _client;
        private IAudioClient _audioCl;

        //TODO: remove before push
        private string _token = "NDQ2MDUyMTcxNTAzMzcwMjQy.Ddza1g.2MdcNp9Yb94jc3BJHrz1wx4NU-Q";

        /*----------------------------------------------------------*/

        //state vars
        private bool _isServerConnected = false;

        private bool _isAudioConnected = false;
        private bool _isPlaying = false;
        private bool _isToAbort = false;
        /*---------------------------------------------------------*/

        #endregion Variables

        #region SaveAndLoadStuff

        public Form1()
        {
            InitializeComponent();
            channelIdBox.Text = _placeholderID.ToString();

            //load from file
            ButtonData[] loadingBtn = LoadSettings();
            if (loadingBtn != null)
                btn = loadingBtn;

            if (btn == null)
                btn = new ButtonData[btnNum];

            //create all Buttons
            for (int i = 0; i < btnNum; i++)
            {
                Button button = new Button();
                button.Tag = i;
                button.Height = 40;
                button.Width = 95;
                button.Click += new EventHandler(btn_Click);
                button.MouseDown += new MouseEventHandler(btn_MouseDown);

                flowLayout.Controls.Add(button);

                //if file is not existing, use default
                if (loadingBtn == null)
                {
                    btn[i] = new ButtonData();
                    initBtnSettings(ref btn[i]);
                }
                button.Text = btn[i].Text;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveSettings(btn);
        }

        private void initBtnSettings(ref ButtonData btn)
        {
            //set to default values
            btn.Text = "Button";
            btn.file = _placeholderFile;

            btn.settingsOpened = false;
        }

        private void SaveSettings(ButtonData[] btnData)
        {
            for (int i = 0; i < btnNum; i++)
            {
                btnData[i].settingsOpened = false;
            }

            Stream stream;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            //save binary
            try
            {
                stream = File.Open(_tokenFile, FileMode.Create);
                binaryFormatter.Serialize(stream, _token);
            }
            catch { }

            try
            {
                stream = File.Open(_settingsFile, FileMode.Create);
            }
            catch
            {
                return;
            }
            binaryFormatter.Serialize(stream, btnData);

            stream.Close();
        }

        private ButtonData[] LoadSettings()
        {
            Stream stream;
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            ButtonData[] loadingBtn;

            //load from binary stream
            try
            {
                stream = File.Open(_tokenFile, FileMode.Open);
                _token = (string)binaryFormatter.Deserialize(stream);
            }
            catch
            {
                tokenButton.PerformClick();
            }

            try
            {
                stream = File.Open(_settingsFile, FileMode.Open);
            }
            catch
            {
                return null;
            }

            loadingBtn = (ButtonData[])binaryFormatter.Deserialize(stream);
            stream.Close();
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
                showTokenError();
                return;
            }

            await _client.StartAsync();
            _isServerConnected = true;
            tokenButton.Enabled = false;
            connectBtn.Text = "Exit";
        }

        private async Task<IAudioClient> connectToChannel()
        {
            //try 5 times
            ulong voiceID = getChannelId();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var tmpAudio = await ((ISocketAudioChannel)_client.GetChannel(voiceID)).ConnectAsync();
                    _isAudioConnected = true;
                    return tmpAudio;
                }
                catch
                {
                    //do nothing, _isAudioConnectes stays false
                }
            }
            return null;
        }

        private ulong getChannelId()
        {//TODO: connect to client
            //get channel ID from user with '_userID'
            return _placeholderID;
        }

        #endregion ConnectToServerAndAudio

        private void disconnectFromServer()
        {
            _client.StopAsync();
            _client.LogoutAsync();
            Environment.Exit(0);
        }

        private async void connectPlayAudioFile(ButtonData btn)
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
                var stream = _audioCl.CreatePCMStream(AudioApplication.Music);

                //TODO: handle mono/stereo
                int channelCount = 2;
                var OutFormat = new WaveFormat(48000, 16, channelCount);

                Mp3FileReader MP3Reader;
                WaveFileReader WavReader;
                MediaFoundationResampler resampler;

                //choose between wav and mp3
                if (btn.file[btn.file.Length - 1] == 'v')
                {
                    try
                    {
                        WavReader = new WaveFileReader(btn.file);
                    }
                    catch
                    {
                        return;
                    }
                    resampler = new MediaFoundationResampler(WavReader, OutFormat);
                }
                else
                {
                    try
                    {
                        MP3Reader = new Mp3FileReader(btn.file);
                    }
                    catch
                    {
                        return;
                    }

                    resampler = new MediaFoundationResampler(MP3Reader, OutFormat);
                }

                resampler.ResamplerQuality = 60;

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
                    {
                        _isToAbort = false;
                        _isPlaying = false;
                        break;
                    }

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

        private void showTokenError()
        {
            MessageBox.Show("Invalid Token", "Token Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #region ButtonEvents

        private void btn_Click(object sender, EventArgs e)
        {
            //start playing routine
            Button button = (Button)sender;

            connectPlayAudioFile(btn[int.Parse(button.Tag.ToString())]);
        }

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
                if (_token == "")
                    showTokenError();
                connectToServer();
            }
            else
            {
                disconnectFromServer();
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
                _placeholderID = result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //open window to enter/view new token
            TokenWindow token = new TokenWindow(_token);
            token.Location = this.Location;
            token.ShowDialog();
            _token = token.token;
        }

        #endregion ButtonEvents
    }

    [Serializable()]
    public class ButtonData
    {
        public bool settingsOpened;
        public string Text;
        public string file;
    }
}