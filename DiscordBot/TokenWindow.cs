using System;
using System.Windows.Forms;

namespace DiscordBot
{
    public partial class TokenWindow : Form
    {
        public string token;
        public bool isBot = false;

        public TokenWindow(string _token, bool _isBot)
        {
            InitializeComponent();
            
            isBot = _isBot;
            token = _token;

            isBotBox.Checked = isBot;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "show")
            {
                var result = MessageBox.Show("Anyone with this token, can controll the bot and possibly abuse his rights.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    tokenBox.Text = token.ToString();
                    tokenBox.PasswordChar = '\0';
                    button1.Text = "hide";
                }
            }
            else
            {
                tokenBox.PasswordChar = '*';
                button1.Text = "show";
            }
        }

        private void tokenBox_TextChanged(object sender, EventArgs e)
        {
            token = tokenBox.Text;
        }

        private void isBotBox_CheckedChanged(object sender, EventArgs e)
        {
            isBot = isBotBox.Checked;
        }
    }
}