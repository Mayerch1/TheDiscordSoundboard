using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBot
{
    public partial class TokenWindow : Form
    {
        public string token;

        public TokenWindow(string _token)
        {
            InitializeComponent();
            token = _token;

            MessageBox.Show("Anyone with this token, can controll the bot and possibly abuse his rights.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tokenBox.Text = token.ToString();
            tokenBox.PasswordChar = '\0';
        }

        private void tokenBox_TextChanged(object sender, EventArgs e)
        {
            token = tokenBox.Text;
        }
    }
}