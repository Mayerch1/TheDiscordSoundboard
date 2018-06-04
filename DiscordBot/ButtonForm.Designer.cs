namespace DiscordBot
{
    partial class ButtonForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ButtonForm));
            this.connectBtn = new System.Windows.Forms.Button();
            this.abortBtn = new System.Windows.Forms.Button();
            this.channelIdLbl = new System.Windows.Forms.Label();
            this.tokenButton = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.filePage = new System.Windows.Forms.TabPage();
            this.flowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.settingsPage = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.autoConnectBox = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.favoriteBtn = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.urlBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.presetBtn = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.streamBox = new System.Windows.Forms.CheckBox();
            this.leaveMsgBox = new System.Windows.Forms.TextBox();
            this.joinMsgBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.joinMsgBtn = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnNumBox = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.donateBtn = new System.Windows.Forms.Button();
            this.volumeSlider = new System.Windows.Forms.TrackBar();
            this.earRapeBox = new System.Windows.Forms.CheckBox();
            this.loopBox = new System.Windows.Forms.CheckBox();
            this.connectContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.connectContextEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.channelIdBox = new System.Windows.Forms.ComboBox();
            this.tabControl.SuspendLayout();
            this.filePage.SuspendLayout();
            this.settingsPage.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel10.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeSlider)).BeginInit();
            this.connectContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectBtn
            // 
            this.connectBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.connectBtn.Location = new System.Drawing.Point(4, 273);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 1;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            this.connectBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.connectBtn_MouseDown);
            // 
            // abortBtn
            // 
            this.abortBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.abortBtn.Location = new System.Drawing.Point(678, 273);
            this.abortBtn.Name = "abortBtn";
            this.abortBtn.Size = new System.Drawing.Size(75, 23);
            this.abortBtn.TabIndex = 8;
            this.abortBtn.Text = "Abort";
            this.abortBtn.UseVisualStyleBackColor = true;
            this.abortBtn.Click += new System.EventHandler(this.abortBtn_Click);
            // 
            // channelIdLbl
            // 
            this.channelIdLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.channelIdLbl.AutoSize = true;
            this.channelIdLbl.Location = new System.Drawing.Point(85, 278);
            this.channelIdLbl.Name = "channelIdLbl";
            this.channelIdLbl.Size = new System.Drawing.Size(63, 13);
            this.channelIdLbl.TabIndex = 9;
            this.channelIdLbl.Text = "Channel ID:";
            // 
            // tokenButton
            // 
            this.tokenButton.Location = new System.Drawing.Point(3, 3);
            this.tokenButton.Name = "tokenButton";
            this.tokenButton.Size = new System.Drawing.Size(75, 23);
            this.tokenButton.TabIndex = 7;
            this.tokenButton.Text = "Token...";
            this.tokenButton.UseVisualStyleBackColor = true;
            this.tokenButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.filePage);
            this.tabControl.Controls.Add(this.settingsPage);
            this.tabControl.Location = new System.Drawing.Point(4, 2);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(749, 265);
            this.tabControl.TabIndex = 10;
            // 
            // filePage
            // 
            this.filePage.Controls.Add(this.flowLayout);
            this.filePage.Location = new System.Drawing.Point(4, 22);
            this.filePage.Name = "filePage";
            this.filePage.Padding = new System.Windows.Forms.Padding(3);
            this.filePage.Size = new System.Drawing.Size(741, 239);
            this.filePage.TabIndex = 0;
            this.filePage.Text = "Files";
            this.filePage.UseVisualStyleBackColor = true;
            // 
            // flowLayout
            // 
            this.flowLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayout.AutoScroll = true;
            this.flowLayout.Location = new System.Drawing.Point(6, 6);
            this.flowLayout.Name = "flowLayout";
            this.flowLayout.Size = new System.Drawing.Size(729, 230);
            this.flowLayout.TabIndex = 4;
            // 
            // settingsPage
            // 
            this.settingsPage.Controls.Add(this.tableLayoutPanel1);
            this.settingsPage.Controls.Add(this.donateBtn);
            this.settingsPage.Location = new System.Drawing.Point(4, 22);
            this.settingsPage.Name = "settingsPage";
            this.settingsPage.Size = new System.Drawing.Size(741, 239);
            this.settingsPage.TabIndex = 1;
            this.settingsPage.Text = "Settings";
            this.settingsPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 256F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(694, 233);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel2.Controls.Add(this.panel7);
            this.flowLayoutPanel2.Controls.Add(this.panel2);
            this.flowLayoutPanel2.Controls.Add(this.panel3);
            this.flowLayoutPanel2.Controls.Add(this.panel4);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(222, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(213, 227);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // panel7
            // 
            this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel7.Controls.Add(this.label5);
            this.panel7.Location = new System.Drawing.Point(3, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(252, 17);
            this.panel7.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Misc";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.autoConnectBox);
            this.panel2.Location = new System.Drawing.Point(3, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(252, 23);
            this.panel2.TabIndex = 14;
            // 
            // autoConnectBox
            // 
            this.autoConnectBox.AutoSize = true;
            this.autoConnectBox.Location = new System.Drawing.Point(3, 3);
            this.autoConnectBox.Name = "autoConnectBox";
            this.autoConnectBox.Size = new System.Drawing.Size(88, 17);
            this.autoConnectBox.TabIndex = 11;
            this.autoConnectBox.Text = "AutoConnect";
            this.autoConnectBox.UseVisualStyleBackColor = true;
            this.autoConnectBox.CheckedChanged += new System.EventHandler(this.autoconnectBox_CheckedChanged);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.favoriteBtn);
            this.panel3.Location = new System.Drawing.Point(3, 55);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(252, 30);
            this.panel3.TabIndex = 15;
            // 
            // favoriteBtn
            // 
            this.favoriteBtn.Location = new System.Drawing.Point(3, 3);
            this.favoriteBtn.Name = "favoriteBtn";
            this.favoriteBtn.Size = new System.Drawing.Size(75, 23);
            this.favoriteBtn.TabIndex = 10;
            this.favoriteBtn.Text = "Favorites...";
            this.favoriteBtn.UseVisualStyleBackColor = true;
            this.favoriteBtn.Click += new System.EventHandler(this.favoriteBtn_Click);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.Controls.Add(this.tokenButton);
            this.panel4.Location = new System.Drawing.Point(3, 91);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(252, 30);
            this.panel4.TabIndex = 16;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel3.Controls.Add(this.panel8);
            this.flowLayoutPanel3.Controls.Add(this.panel10);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(441, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(250, 227);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // panel8
            // 
            this.panel8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel8.Controls.Add(this.label6);
            this.panel8.Location = new System.Drawing.Point(3, 3);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(247, 17);
            this.panel8.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Gamestate";
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.urlBox);
            this.panel10.Controls.Add(this.label2);
            this.panel10.Controls.Add(this.presetBtn);
            this.panel10.Controls.Add(this.label7);
            this.panel10.Controls.Add(this.streamBox);
            this.panel10.Controls.Add(this.leaveMsgBox);
            this.panel10.Controls.Add(this.joinMsgBox);
            this.panel10.Controls.Add(this.label8);
            this.panel10.Controls.Add(this.joinMsgBtn);
            this.panel10.Location = new System.Drawing.Point(3, 26);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(247, 201);
            this.panel10.TabIndex = 20;
            // 
            // urlBox
            // 
            this.urlBox.Location = new System.Drawing.Point(6, 62);
            this.urlBox.Name = "urlBox";
            this.urlBox.Size = new System.Drawing.Size(234, 20);
            this.urlBox.TabIndex = 23;
            this.urlBox.TextChanged += new System.EventHandler(this.urlBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Stream Url";
            // 
            // presetBtn
            // 
            this.presetBtn.Location = new System.Drawing.Point(165, 117);
            this.presetBtn.Name = "presetBtn";
            this.presetBtn.Size = new System.Drawing.Size(75, 23);
            this.presetBtn.TabIndex = 20;
            this.presetBtn.Text = "Presets...";
            this.presetBtn.UseVisualStyleBackColor = true;
            this.presetBtn.Click += new System.EventHandler(this.presetBtn_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "On Join";
            // 
            // streamBox
            // 
            this.streamBox.AutoSize = true;
            this.streamBox.Location = new System.Drawing.Point(6, 92);
            this.streamBox.Name = "streamBox";
            this.streamBox.Size = new System.Drawing.Size(83, 17);
            this.streamBox.TabIndex = 21;
            this.streamBox.Text = "is Streaming";
            this.streamBox.UseVisualStyleBackColor = true;
            this.streamBox.CheckedChanged += new System.EventHandler(this.streamBox_CheckedChanged);
            // 
            // leaveMsgBox
            // 
            this.leaveMsgBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.leaveMsgBox.Location = new System.Drawing.Point(6, 178);
            this.leaveMsgBox.Name = "leaveMsgBox";
            this.leaveMsgBox.Size = new System.Drawing.Size(234, 20);
            this.leaveMsgBox.TabIndex = 15;
            this.leaveMsgBox.TextChanged += new System.EventHandler(this.leaveMsgBox_TextChanged);
            // 
            // joinMsgBox
            // 
            this.joinMsgBox.FormattingEnabled = true;
            this.joinMsgBox.Location = new System.Drawing.Point(6, 16);
            this.joinMsgBox.Name = "joinMsgBox";
            this.joinMsgBox.Size = new System.Drawing.Size(234, 21);
            this.joinMsgBox.TabIndex = 18;
            this.joinMsgBox.SelectedIndexChanged += new System.EventHandler(this.joinMsgBox_SelectedIndexChanged);
            this.joinMsgBox.TextUpdate += new System.EventHandler(this.joinMsgBox_SelectedIndexChanged);
            this.joinMsgBox.DropDownClosed += new System.EventHandler(this.joinMsgBox_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 162);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "On leave";
            // 
            // joinMsgBtn
            // 
            this.joinMsgBtn.Location = new System.Drawing.Point(165, 88);
            this.joinMsgBtn.Name = "joinMsgBtn";
            this.joinMsgBtn.Size = new System.Drawing.Size(75, 23);
            this.joinMsgBtn.TabIndex = 19;
            this.joinMsgBtn.Text = "Set now";
            this.joinMsgBtn.UseVisualStyleBackColor = true;
            this.joinMsgBtn.Click += new System.EventHandler(this.joinMsgBtn_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.panel9);
            this.flowLayoutPanel1.Controls.Add(this.panel1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(213, 227);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // panel9
            // 
            this.panel9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel9.Controls.Add(this.label4);
            this.panel9.Location = new System.Drawing.Point(3, 3);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(252, 17);
            this.panel9.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Buttons";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnNumBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(253, 42);
            this.panel1.TabIndex = 12;
            // 
            // btnNumBox
            // 
            this.btnNumBox.Location = new System.Drawing.Point(6, 16);
            this.btnNumBox.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.btnNumBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.btnNumBox.Name = "btnNumBox";
            this.btnNumBox.Size = new System.Drawing.Size(55, 20);
            this.btnNumBox.TabIndex = 7;
            this.btnNumBox.Value = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.btnNumBox.ValueChanged += new System.EventHandler(this.btnNumBox_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Total";
            // 
            // donateBtn
            // 
            this.donateBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.donateBtn.BackgroundImage = global::DiscordBot.Properties.Resources.paypal;
            this.donateBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.donateBtn.Location = new System.Drawing.Point(704, 3);
            this.donateBtn.Name = "donateBtn";
            this.donateBtn.Size = new System.Drawing.Size(33, 35);
            this.donateBtn.TabIndex = 8;
            this.donateBtn.UseVisualStyleBackColor = true;
            this.donateBtn.Click += new System.EventHandler(this.donateBtn_Click);
            // 
            // volumeSlider
            // 
            this.volumeSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.volumeSlider.LargeChange = 10;
            this.volumeSlider.Location = new System.Drawing.Point(299, 273);
            this.volumeSlider.Maximum = 200;
            this.volumeSlider.Name = "volumeSlider";
            this.volumeSlider.Size = new System.Drawing.Size(171, 45);
            this.volumeSlider.TabIndex = 3;
            this.volumeSlider.TickFrequency = 25;
            this.volumeSlider.Value = 25;
            // 
            // earRapeBox
            // 
            this.earRapeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.earRapeBox.AutoSize = true;
            this.earRapeBox.Location = new System.Drawing.Point(476, 277);
            this.earRapeBox.Name = "earRapeBox";
            this.earRapeBox.Size = new System.Drawing.Size(63, 17);
            this.earRapeBox.TabIndex = 4;
            this.earRapeBox.Text = "Earrape";
            this.earRapeBox.UseVisualStyleBackColor = true;
            this.earRapeBox.CheckedChanged += new System.EventHandler(this.earRapeBox_CheckedChanged);
            // 
            // loopBox
            // 
            this.loopBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loopBox.AutoSize = true;
            this.loopBox.Location = new System.Drawing.Point(545, 277);
            this.loopBox.Name = "loopBox";
            this.loopBox.Size = new System.Drawing.Size(50, 17);
            this.loopBox.TabIndex = 5;
            this.loopBox.Text = "Loop";
            this.loopBox.UseVisualStyleBackColor = true;
            this.loopBox.CheckedChanged += new System.EventHandler(this.loopBox_CheckedChanged);
            // 
            // connectContext
            // 
            this.connectContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectContextEntry});
            this.connectContext.Name = "connectContext";
            this.connectContext.Size = new System.Drawing.Size(144, 26);
            // 
            // connectContextEntry
            // 
            this.connectContextEntry.Name = "connectContextEntry";
            this.connectContextEntry.Size = new System.Drawing.Size(143, 22);
            this.connectContextEntry.Text = "Autoconnect";
            this.connectContextEntry.Click += new System.EventHandler(this.connectContextEntry_Click);
            // 
            // channelIdBox
            // 
            this.channelIdBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.channelIdBox.FormattingEnabled = true;
            this.channelIdBox.Location = new System.Drawing.Point(154, 273);
            this.channelIdBox.Name = "channelIdBox";
            this.channelIdBox.Size = new System.Drawing.Size(139, 21);
            this.channelIdBox.Sorted = true;
            this.channelIdBox.TabIndex = 5;
            // 
            // ButtonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 308);
            this.Controls.Add(this.channelIdBox);
            this.Controls.Add(this.loopBox);
            this.Controls.Add(this.earRapeBox);
            this.Controls.Add(this.volumeSlider);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.channelIdLbl);
            this.Controls.Add(this.abortBtn);
            this.Controls.Add(this.connectBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ButtonForm";
            this.Text = "SoundBoard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.tabControl.ResumeLayout(false);
            this.filePage.ResumeLayout(false);
            this.settingsPage.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeSlider)).EndInit();
            this.connectContext.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.Button abortBtn;
        private System.Windows.Forms.Label channelIdLbl;
        private System.Windows.Forms.Button tokenButton;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage filePage;
        private System.Windows.Forms.FlowLayoutPanel flowLayout;
        private System.Windows.Forms.TrackBar volumeSlider;
        private System.Windows.Forms.CheckBox earRapeBox;
        private System.Windows.Forms.CheckBox loopBox;
        private System.Windows.Forms.ContextMenuStrip connectContext;
        private System.Windows.Forms.ToolStripMenuItem connectContextEntry;
        private System.Windows.Forms.TabPage settingsPage;
        private System.Windows.Forms.NumericUpDown btnNumBox;
        private System.Windows.Forms.Button donateBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button favoriteBtn;
        private System.Windows.Forms.CheckBox autoConnectBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Button joinMsgBtn;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox leaveMsgBox;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.ComboBox channelIdBox;
        private System.Windows.Forms.Button presetBtn;
        private System.Windows.Forms.ComboBox joinMsgBox;
        private System.Windows.Forms.CheckBox streamBox;
        private System.Windows.Forms.TextBox urlBox;
        private System.Windows.Forms.Label label2;
    }
}

