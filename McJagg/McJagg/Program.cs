using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace McJagg
{
    class Program
    {
        public static MainWindow mainWindow = null;
        [STAThread]
        static void Main(string[] args)
        {
            command.Command.loadCommands();
            Config.loadBannedConfig();
            Config.loadOfflinePlayers();
            Config.loadRanksConfig();
            Config.loadServerConfig();

            if (Config.server.showGui) {
                new Thread(new ThreadStart(delegate
                {
                    Application.EnableVisualStyles();
                    Application.Run(mainWindow = new MainWindow());
                })).Start();
            }

            map.Map.startSaveThread();

            if (!map.Map.exists(Config.server.spawnMap))
            {
                map.Map.create(Config.server.spawnMap, 128, 64, 128);
                map.Map.create("guest", 128, 64, 128);
            }
            
            server.server e = new server.server("http://www.classicube.net/heartbeat.jsp", 12345, "classic -hax +ophax",true, 21);
            e.serve();
        }
    }
}
