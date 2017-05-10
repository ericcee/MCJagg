using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace McJagg
{
    public partial class MainWindow : Form
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        public static bool editingPlayerRank = false;

        public static OfflinePlayer selectedPlayer = null;

        public class ConsoleCommandSender : command.CommandExecutor
        {
            public override void commandOutput(string message)
            {
                windowLog(message);
            }
            public override string name()
            {
                return "$console";
            }
        }
        private static ConsoleCommandSender commandSender = new ConsoleCommandSender();
        private static MainWindow m = null;
        public MainWindow()
        {
            m = this;
            InitializeComponent();

            new Thread(new ThreadStart(delegate
            {
                while (!listBoxFoundOfflinePlayers.Created) { Thread.Sleep(50); }
                updateOfflinePlayerSearch();
                for (int i=0; i<Config.ranks.ranks.Count; i++)
                {
                    addRank(Config.ranks.ranks[i]);
                }
            })).Start();
        }

        public static void windowLog(string msg)
        {
            m.textBoxLog.Invoke(new MethodInvoker(delegate
            {
                m.textBoxLog.AppendText("\r\n" + msg);
            }));
        }
        public static void windowDebugLog(string msg)
        {
            if (m.checkBoxShowDebugMessages.Checked)
            {
                m.textBoxLog.Invoke(new MethodInvoker(delegate
                {
                    m.textBoxLog.AppendText("\r\n" + msg);
                }));
            }
        }
        public static void windowErrorLog(string msg)
        {
            // TODO ? if (m.checkBoxShowDebugMessages.Checked)
            {
                m.textBoxLog.Invoke(new MethodInvoker(delegate
                {
                    m.textBoxLog.AppendText("\r\n" + msg);
                }));
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public static void addOnlinePlayer(Player player)
        {
            try
            {
                m.listBoxOnlinePlayers.Invoke(new MethodInvoker(delegate
                {
                    m.listBoxOnlinePlayers.Items.Add(player.getName());
                }));
                if (player.offlinePlayer == selectedPlayer)
                {
                    m.Invoke(new MethodInvoker(delegate
                    {
                        m.updateSelectedPlayerInfo();
                    }));
                }
            }
            catch { }
        }
        public static void removeOnlinePlayer(Player player)
        {
            try
            {
                m.listBoxOnlinePlayers.Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        m.listBoxOnlinePlayers.Items.Remove(player.getName());
                    }
                    catch { }
                }));
                if (player.offlinePlayer == selectedPlayer)
                {
                    selectedPlayer = null;
                    editingPlayerRank = false;
                    m.Invoke(new MethodInvoker(delegate
                    {
                        m.updateSelectedPlayerInfo();
                    }));
                }
            }
            catch { }
        }

        private static string currentOfflinePlayerQuery = null;

        public static void updateOfflinePlayerSearch()
        {
            m.textBoxSearchOfflinePlayers.Invoke(new MethodInvoker(delegate{
                // If nothing has changed, we don't have to refresh the search
                if (currentOfflinePlayerQuery == m.textBoxSearchOfflinePlayers.Text) return;

                // Otherwise we have to do things :-(
                currentOfflinePlayerQuery = m.textBoxSearchOfflinePlayers.Text;
                m.listBoxFoundOfflinePlayers.Items.Clear();

                IEnumerable<OfflinePlayer> results = OfflinePlayer.search(currentOfflinePlayerQuery);
                foreach (OfflinePlayer op in results)
                {
                    m.listBoxFoundOfflinePlayers.Items.Add(op.getName());
                }
                if (m.listBoxFoundOfflinePlayers.Items.Count == 0)
                {
                    m.listBoxFoundOfflinePlayers.Items.Add("No results found! :-(");
                }
            }));
        }


        private void textBoxSearchOfflinePlayers_KeyUp(object sender, KeyEventArgs e)
        {
            updateOfflinePlayerSearch();
        }

        private void textBoxSearchOfflinePlayers_KeyDown(object sender, KeyEventArgs e)
        {
            updateOfflinePlayerSearch();
        }

        private void textBoxRunCommand_TextChanged(object sender, EventArgs e)
        {

        }

        private List<string> commandHistory = new List<string>();
        private int currentHistoryUp = -1;

        private void textBoxRunCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                int prevCurrentHistoryUp = currentHistoryUp;
                if (e.KeyCode == Keys.Up)
                {
                    currentHistoryUp++;
                    int index = commandHistory.Count - 1 - currentHistoryUp;
                    if (index >= commandHistory.Count || index < 0)
                    {
                        // Out of bounds: cancel it!
                        currentHistoryUp--;
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    currentHistoryUp--;
                    if (currentHistoryUp < -1)
                    {
                        // Out of bounds: cancel it!
                        currentHistoryUp++;
                    }
                }
                if (prevCurrentHistoryUp != currentHistoryUp)
                {
                    if (currentHistoryUp == -1) textBoxRunCommand.Text = "";
                    else
                    {
                        int index = commandHistory.Count - 1 - currentHistoryUp;
                        textBoxRunCommand.Text = commandHistory[index];
                    }
                }
                return;
            }
            if (e.KeyCode == Keys.Enter)
            {
                string cmd = textBoxRunCommand.Text.Trim();
                textBoxRunCommand.Text = "";
                if (cmd == "") return;
                try
                {
                    if (!cmd.StartsWith("/")) cmd = "/" + cmd;
                    logger.log("Console executing command: "+cmd);
                    if (currentHistoryUp != -1)
                    {
                        if (cmd != commandHistory[commandHistory.Count - 1 - currentHistoryUp])
                        {
                            commandHistory.Add(cmd);
                            currentHistoryUp = -1;
                        }
                    }
                    else if (currentHistoryUp == -1)
                    {
                        commandHistory.Add(cmd);
                    }
                    command.Command.execute(cmd, commandSender);
                }
                catch (Exception ee)
                {
                    logger.log("ERROR EXECUTING COMMAND: "+ee.Message);
                    logger.log(ee.StackTrace);
                }
            }
        }


        public static void addRank(Rank rank)
        {
            m.comboBoxSelectedPlayerRank.Invoke(new MethodInvoker(delegate
            {
                m.comboBoxSelectedPlayerRank.Items.Add(rank.getName());
            }));
        }
        public static void removeRank(Rank rank)
        {
            m.comboBoxSelectedPlayerRank.Invoke(new MethodInvoker(delegate
            {
                m.comboBoxSelectedPlayerRank.Items.Remove(rank.getName());
            }));
        }
        private void updateSelectedPlayerInfo()
        {
            if (selectedPlayer == null)
            {
                textBoxSelectedPlayerName.Text = "";
                textBoxSelectedPlayerX.Text = "";
                textBoxSelectedPlayerY.Text = "";
                textBoxSelectedPlayerZ.Text = "";
                textBoxSelectedPlayerMap.Text = "";
                comboBoxSelectedPlayerRank.SelectedIndex = -1;
                comboBoxSelectedPlayerRank.Enabled = false;
                buttonSaveRank.Enabled = false;
                buttonKick.Enabled = false;
            }
            else
            {
                Player player = Player.getPlayer(selectedPlayer.getName());
                if (player == null)
                {
                    textBoxSelectedPlayerX.Text = "";
                    textBoxSelectedPlayerY.Text = "";
                    textBoxSelectedPlayerZ.Text = "";
                    textBoxSelectedPlayerMap.Text = "";
                    buttonKick.Enabled = false;
                }
                else
                {
                    textBoxSelectedPlayerX.Text = ""+player.getX();
                    textBoxSelectedPlayerY.Text = "" + player.getY();
                    textBoxSelectedPlayerZ.Text = "" + player.getZ();
                    textBoxSelectedPlayerMap.Text = "" + player.getMap().getName();
                    buttonKick.Enabled = true;
                }
                textBoxSelectedPlayerName.Text = selectedPlayer.getName();
                if (!editingPlayerRank) comboBoxSelectedPlayerRank.SelectedItem = (string)selectedPlayer.getRank().getName();
                //else if ((string)comboBoxSelectedPlayerRank.SelectedItem == selectedPlayer.getRank().getName()) editingPlayerRank = false;
                comboBoxSelectedPlayerRank.Enabled = true;
                buttonSaveRank.Enabled = true;
            }
        }

        public static void consoleRunCommand(string cmd)
        {
            if (!cmd.StartsWith("/")) cmd = "/" + cmd;
            command.Command.execute(cmd, commandSender);
        }

        /// <summary>
        /// PLAYER SELECTED
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxOnlinePlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxOnlinePlayers.SelectedIndex;
            if (index >= 0)
            {
                selectedPlayer = null;
                editingPlayerRank = false;
                updateSelectedPlayerInfo();
                try
                {
                    selectedPlayer = OfflinePlayer.getOfflinePlayer((string)listBoxOnlinePlayers.Items[index]);
                    updateSelectedPlayerInfo();
                }
                catch { }
            }
        }

        private void listBoxFoundOfflinePlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxFoundOfflinePlayers.SelectedIndex;
            if (index >= 0)
            {
                selectedPlayer = null;
                updateSelectedPlayerInfo();
                try
                {
                    selectedPlayer = OfflinePlayer.getOfflinePlayer(
                        (string)listBoxFoundOfflinePlayers.Items[index]
                        
                        );
                    editingPlayerRank = false;
                    updateSelectedPlayerInfo();
                }
                catch(Exception ee) { logger.debugLog(ee.Message + ee.StackTrace); }
            }
        }

        private void checkBoxShowConsoleWindow_CheckedChanged(object sender, EventArgs e)
        {
            var handle = GetConsoleWindow();
            if (checkBoxShowConsoleWindow.Checked)
            {
                // Show
                ShowWindow(handle, SW_SHOW);
            }
            else
            {
                // Hide
                ShowWindow(handle, SW_HIDE);
            }
        }

        private void checkBoxShowGUI_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowGUI.Checked == false) Hide();
        }

        public static void show()
        {
            m.Invoke(new MethodInvoker(delegate
            {
                m.Show();
                m.checkBoxShowGUI.Invoke(new MethodInvoker(delegate
                {
                    m.checkBoxShowGUI.Checked = true;
                }));
            }));
        }
        public static void hide()
        {
            m.Invoke(new MethodInvoker(delegate
            {
                m.checkBoxShowGUI.Invoke(new MethodInvoker(delegate
                {
                    m.checkBoxShowGUI.Checked = false;
                }));
                m.Hide();
            }));
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
        
        private void MainWindow_Load(object sender, EventArgs e)
        {

        }
        
        private void buttonSaveRank_Click(object sender, EventArgs e)
        {
            if (selectedPlayer == null) return;
            if (!buttonSaveRank.Enabled) return;
            consoleRunCommand("/setrank " + textBoxSelectedPlayerName.Text + " " + comboBoxSelectedPlayerRank.Text);
        }

        public static void updatePlayerMap(Player player, map.Map map)
        {
            if (selectedPlayer != null &&
                player != null &&
                player.offlinePlayer != null &&
                player.offlinePlayer.getName().ToLower() == selectedPlayer.getName().ToLower())
            {
                m.Invoke(new MethodInvoker(delegate
                {
                    m.updateSelectedPlayerInfo();
                }));
            }
        }
        public static void updatePlayerPosition(Player player)
        {
            if (selectedPlayer != null &&
                player != null &&
                player.offlinePlayer != null &&
                player.offlinePlayer == selectedPlayer)
            {
                m.Invoke(new MethodInvoker(delegate
                {
                    m.updateSelectedPlayerInfo();
                }));
            }
        }

        public void setTextAsync(TextBox tb, string newText)
        {
            tb.Invoke(new MethodInvoker(delegate
            {
                tb.Text = newText;
            }));
        }

        private void comboBoxSelectedPlayerRank_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedPlayer == null) return;
            if (((string)comboBoxSelectedPlayerRank.SelectedItem).ToLower() == selectedPlayer.getRank().getName().ToLower())
            {
                editingPlayerRank = false;
            }
            else
            {
                editingPlayerRank = true;
            }
        }

        private void comboBoxSelectedPlayerRank_Leave(object sender, EventArgs e)
        {
            if (selectedPlayer == null) return;
            if (((string)comboBoxSelectedPlayerRank.SelectedItem).ToLower() == selectedPlayer.getRank().getName().ToLower())
            {
                editingPlayerRank = false;
            }
            else
            {
                editingPlayerRank = true;
            }
        }

        private void comboBoxSelectedPlayerRank_MouseDown(object sender, MouseEventArgs e)
        {
            editingPlayerRank = true;
        }

        private void buttonKick_Click(object sender, EventArgs e)
        {
            consoleRunCommand("/kick "+textBoxSelectedPlayerName.Text+" "+textBoxKickMessage.Text);
        }
    }
}
