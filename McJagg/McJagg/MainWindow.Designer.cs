namespace McJagg
{
    partial class MainWindow
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
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxShowGUI = new System.Windows.Forms.CheckBox();
            this.checkBoxShowConsoleWindow = new System.Windows.Forms.CheckBox();
            this.checkBoxShowDebugMessages = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBoxOnlinePlayers = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listBoxFoundOfflinePlayers = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSearchOfflinePlayers = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBoxSelectedPlayerMap = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxKickMessage = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonKick = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxSelectedPlayerRank = new System.Windows.Forms.ComboBox();
            this.buttonSaveRank = new System.Windows.Forms.Button();
            this.textBoxSelectedPlayerName = new System.Windows.Forms.TextBox();
            this.textBoxSelectedPlayerX = new System.Windows.Forms.TextBox();
            this.textBoxSelectedPlayerY = new System.Windows.Forms.TextBox();
            this.textBoxSelectedPlayerZ = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxRunCommand = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(13, 43);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(391, 381);
            this.textBoxLog.TabIndex = 0;
            this.textBoxLog.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxShowGUI);
            this.groupBox1.Controls.Add(this.checkBoxShowConsoleWindow);
            this.groupBox1.Controls.Add(this.checkBoxShowDebugMessages);
            this.groupBox1.Controls.Add(this.textBoxLog);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 433);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // checkBoxShowGUI
            // 
            this.checkBoxShowGUI.AutoSize = true;
            this.checkBoxShowGUI.Checked = true;
            this.checkBoxShowGUI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowGUI.Location = new System.Drawing.Point(256, 20);
            this.checkBoxShowGUI.Name = "checkBoxShowGUI";
            this.checkBoxShowGUI.Size = new System.Drawing.Size(75, 17);
            this.checkBoxShowGUI.TabIndex = 3;
            this.checkBoxShowGUI.Text = "Show GUI";
            this.checkBoxShowGUI.UseVisualStyleBackColor = true;
            this.checkBoxShowGUI.CheckedChanged += new System.EventHandler(this.checkBoxShowGUI_CheckedChanged);
            // 
            // checkBoxShowConsoleWindow
            // 
            this.checkBoxShowConsoleWindow.AutoSize = true;
            this.checkBoxShowConsoleWindow.Checked = true;
            this.checkBoxShowConsoleWindow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowConsoleWindow.Location = new System.Drawing.Point(156, 20);
            this.checkBoxShowConsoleWindow.Name = "checkBoxShowConsoleWindow";
            this.checkBoxShowConsoleWindow.Size = new System.Drawing.Size(93, 17);
            this.checkBoxShowConsoleWindow.TabIndex = 2;
            this.checkBoxShowConsoleWindow.Text = "Show console";
            this.checkBoxShowConsoleWindow.UseVisualStyleBackColor = true;
            this.checkBoxShowConsoleWindow.CheckedChanged += new System.EventHandler(this.checkBoxShowConsoleWindow_CheckedChanged);
            // 
            // checkBoxShowDebugMessages
            // 
            this.checkBoxShowDebugMessages.AutoSize = true;
            this.checkBoxShowDebugMessages.Location = new System.Drawing.Point(13, 20);
            this.checkBoxShowDebugMessages.Name = "checkBoxShowDebugMessages";
            this.checkBoxShowDebugMessages.Size = new System.Drawing.Size(136, 17);
            this.checkBoxShowDebugMessages.TabIndex = 1;
            this.checkBoxShowDebugMessages.Text = "Show debug messages";
            this.checkBoxShowDebugMessages.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBoxOnlinePlayers);
            this.groupBox2.Location = new System.Drawing.Point(428, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 179);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Online players";
            // 
            // listBoxOnlinePlayers
            // 
            this.listBoxOnlinePlayers.FormattingEnabled = true;
            this.listBoxOnlinePlayers.Location = new System.Drawing.Point(7, 19);
            this.listBoxOnlinePlayers.Name = "listBoxOnlinePlayers";
            this.listBoxOnlinePlayers.Size = new System.Drawing.Size(193, 147);
            this.listBoxOnlinePlayers.TabIndex = 5;
            this.listBoxOnlinePlayers.SelectedIndexChanged += new System.EventHandler(this.listBoxOnlinePlayers_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listBoxFoundOfflinePlayers);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.textBoxSearchOfflinePlayers);
            this.groupBox3.Location = new System.Drawing.Point(428, 199);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(208, 306);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search offline player";
            // 
            // listBoxFoundOfflinePlayers
            // 
            this.listBoxFoundOfflinePlayers.FormattingEnabled = true;
            this.listBoxFoundOfflinePlayers.Location = new System.Drawing.Point(10, 45);
            this.listBoxFoundOfflinePlayers.Name = "listBoxFoundOfflinePlayers";
            this.listBoxFoundOfflinePlayers.Size = new System.Drawing.Size(190, 251);
            this.listBoxFoundOfflinePlayers.TabIndex = 3;
            this.listBoxFoundOfflinePlayers.SelectedIndexChanged += new System.EventHandler(this.listBoxFoundOfflinePlayers_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Filter:";
            // 
            // textBoxSearchOfflinePlayers
            // 
            this.textBoxSearchOfflinePlayers.Location = new System.Drawing.Point(45, 19);
            this.textBoxSearchOfflinePlayers.Name = "textBoxSearchOfflinePlayers";
            this.textBoxSearchOfflinePlayers.Size = new System.Drawing.Size(155, 20);
            this.textBoxSearchOfflinePlayers.TabIndex = 1;
            this.textBoxSearchOfflinePlayers.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSearchOfflinePlayers_KeyDown);
            this.textBoxSearchOfflinePlayers.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxSearchOfflinePlayers_KeyUp);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBoxSelectedPlayerMap);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.textBoxKickMessage);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.buttonKick);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.comboBoxSelectedPlayerRank);
            this.groupBox4.Controls.Add(this.buttonSaveRank);
            this.groupBox4.Controls.Add(this.textBoxSelectedPlayerName);
            this.groupBox4.Controls.Add(this.textBoxSelectedPlayerX);
            this.groupBox4.Controls.Add(this.textBoxSelectedPlayerY);
            this.groupBox4.Controls.Add(this.textBoxSelectedPlayerZ);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(643, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 492);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Selected player info";
            // 
            // textBoxSelectedPlayerMap
            // 
            this.textBoxSelectedPlayerMap.Location = new System.Drawing.Point(67, 170);
            this.textBoxSelectedPlayerMap.Name = "textBoxSelectedPlayerMap";
            this.textBoxSelectedPlayerMap.ReadOnly = true;
            this.textBoxSelectedPlayerMap.Size = new System.Drawing.Size(127, 20);
            this.textBoxSelectedPlayerMap.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 173);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Map:";
            // 
            // textBoxKickMessage
            // 
            this.textBoxKickMessage.Location = new System.Drawing.Point(10, 230);
            this.textBoxKickMessage.Name = "textBoxKickMessage";
            this.textBoxKickMessage.Size = new System.Drawing.Size(184, 20);
            this.textBoxKickMessage.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 212);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Kick message:";
            // 
            // buttonKick
            // 
            this.buttonKick.Enabled = false;
            this.buttonKick.Location = new System.Drawing.Point(10, 256);
            this.buttonKick.Name = "buttonKick";
            this.buttonKick.Size = new System.Drawing.Size(184, 23);
            this.buttonKick.TabIndex = 11;
            this.buttonKick.Text = "Kick";
            this.buttonKick.UseVisualStyleBackColor = true;
            this.buttonKick.Click += new System.EventHandler(this.buttonKick_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 119);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Rank:";
            // 
            // comboBoxSelectedPlayerRank
            // 
            this.comboBoxSelectedPlayerRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectedPlayerRank.FormattingEnabled = true;
            this.comboBoxSelectedPlayerRank.Location = new System.Drawing.Point(67, 116);
            this.comboBoxSelectedPlayerRank.Name = "comboBoxSelectedPlayerRank";
            this.comboBoxSelectedPlayerRank.Size = new System.Drawing.Size(127, 21);
            this.comboBoxSelectedPlayerRank.TabIndex = 9;
            this.comboBoxSelectedPlayerRank.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectedPlayerRank_SelectedIndexChanged);
            this.comboBoxSelectedPlayerRank.Leave += new System.EventHandler(this.comboBoxSelectedPlayerRank_Leave);
            this.comboBoxSelectedPlayerRank.MouseDown += new System.Windows.Forms.MouseEventHandler(this.comboBoxSelectedPlayerRank_MouseDown);
            // 
            // buttonSaveRank
            // 
            this.buttonSaveRank.Enabled = false;
            this.buttonSaveRank.Location = new System.Drawing.Point(10, 143);
            this.buttonSaveRank.Name = "buttonSaveRank";
            this.buttonSaveRank.Size = new System.Drawing.Size(184, 23);
            this.buttonSaveRank.TabIndex = 8;
            this.buttonSaveRank.Text = "Save rank";
            this.buttonSaveRank.UseVisualStyleBackColor = true;
            this.buttonSaveRank.Click += new System.EventHandler(this.buttonSaveRank_Click);
            // 
            // textBoxSelectedPlayerName
            // 
            this.textBoxSelectedPlayerName.Location = new System.Drawing.Point(67, 17);
            this.textBoxSelectedPlayerName.Name = "textBoxSelectedPlayerName";
            this.textBoxSelectedPlayerName.ReadOnly = true;
            this.textBoxSelectedPlayerName.Size = new System.Drawing.Size(127, 20);
            this.textBoxSelectedPlayerName.TabIndex = 7;
            // 
            // textBoxSelectedPlayerX
            // 
            this.textBoxSelectedPlayerX.Location = new System.Drawing.Point(67, 43);
            this.textBoxSelectedPlayerX.Name = "textBoxSelectedPlayerX";
            this.textBoxSelectedPlayerX.ReadOnly = true;
            this.textBoxSelectedPlayerX.Size = new System.Drawing.Size(127, 20);
            this.textBoxSelectedPlayerX.TabIndex = 6;
            // 
            // textBoxSelectedPlayerY
            // 
            this.textBoxSelectedPlayerY.Location = new System.Drawing.Point(67, 66);
            this.textBoxSelectedPlayerY.Name = "textBoxSelectedPlayerY";
            this.textBoxSelectedPlayerY.ReadOnly = true;
            this.textBoxSelectedPlayerY.Size = new System.Drawing.Size(127, 20);
            this.textBoxSelectedPlayerY.TabIndex = 5;
            // 
            // textBoxSelectedPlayerZ
            // 
            this.textBoxSelectedPlayerZ.Location = new System.Drawing.Point(67, 89);
            this.textBoxSelectedPlayerZ.Name = "textBoxSelectedPlayerZ";
            this.textBoxSelectedPlayerZ.ReadOnly = true;
            this.textBoxSelectedPlayerZ.Size = new System.Drawing.Size(127, 20);
            this.textBoxSelectedPlayerZ.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Current Z:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Current Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Current X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Username:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxRunCommand);
            this.groupBox5.Location = new System.Drawing.Point(12, 453);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(410, 52);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Run command";
            // 
            // textBoxRunCommand
            // 
            this.textBoxRunCommand.Location = new System.Drawing.Point(7, 19);
            this.textBoxRunCommand.Name = "textBoxRunCommand";
            this.textBoxRunCommand.Size = new System.Drawing.Size(397, 20);
            this.textBoxRunCommand.TabIndex = 0;
            this.textBoxRunCommand.TextChanged += new System.EventHandler(this.textBoxRunCommand_TextChanged);
            this.textBoxRunCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxRunCommand_KeyDown);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 517);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainWindow";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxShowDebugMessages;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSearchOfflinePlayers;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox listBoxOnlinePlayers;
        private System.Windows.Forms.ListBox listBoxFoundOfflinePlayers;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSelectedPlayerY;
        private System.Windows.Forms.TextBox textBoxSelectedPlayerZ;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBoxRunCommand;
        private System.Windows.Forms.TextBox textBoxSelectedPlayerX;
        private System.Windows.Forms.TextBox textBoxSelectedPlayerName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxSelectedPlayerRank;
        private System.Windows.Forms.Button buttonSaveRank;
        private System.Windows.Forms.Button buttonKick;
        private System.Windows.Forms.TextBox textBoxKickMessage;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxSelectedPlayerMap;
        private System.Windows.Forms.CheckBox checkBoxShowConsoleWindow;
        private System.Windows.Forms.CheckBox checkBoxShowGUI;
    }
}