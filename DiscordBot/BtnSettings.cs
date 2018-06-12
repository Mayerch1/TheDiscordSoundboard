using System;
using System.IO;
using System.Windows.Forms;

namespace DiscordBot
{
    public partial class BtnSettings : Form
    {
        public ButtonData btn_n;

        public BtnSettings(ButtonData btn)
        {
            InitializeComponent();

            btn_n = btn;
            btnTextBox.Text = btn_n.Text;
            fileChooseBox.Text = btn_n.file;
            filechooser.Filter = "mp3/wav files (*.mp3/*.wav)|*.mp3;*.wav|mp3 files (*.mp3)|*.mp3|wav files (*.wav)|*.wav";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTextBox_TextChanged(object sender, EventArgs e)
        {
            btn_n.Text = btnTextBox.Text;
        }

        private void fileBtn_Click(object sender, EventArgs e)
        {
            filechooser.ShowDialog();

            var newFile = filechooser.FileName.ToString();

            if (newFile != "fileChooser" && newFile != " ")
            {
                // btn_n.file = newFile;
                fileChooseBox.Text = newFile;
                getFileName(newFile);
            }
        }

        private void getFileName(string file)
        {
            //write fileName into Text var of btn_n
            if (btn_n.Text != " ")
                return;
            var fileName = Path.GetFileName(file);
            btn_n.Text = fileName.Substring(0, fileName.LastIndexOf('.'));
            btnTextBox.Text = btn_n.Text;
        }

        private void fileChooseBox_TextChanged(object sender, EventArgs e)
        {
            btn_n.file = fileChooseBox.Text;
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            btnTextBox.Text = " ";
            btn_n.Text = " ";
            btn_n.file = "";
            fileChooseBox.Text = "";
        }
    }
}