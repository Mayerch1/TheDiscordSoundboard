using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            btn_n.file = filechooser.FileName.ToString();
            fileChooseBox.Text = btn_n.file;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.paypal.me/CJMayer/3,99"));
        }
    }
}