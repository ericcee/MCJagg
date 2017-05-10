using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace McJagg.server
{
    public class server
    {
        public static server serv = null;

        public string salt = string.Empty;
        private string HeartbeatServer = string.Empty;

        public string motd = string.Empty;

        private int Port = 0;

        public int MaxPlayer = 0;

        bool Public = false;

        private TcpListener tcpserver = null;

        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();

        private Thread heartbeat = null;
        private Thread serveth = null;

        public List<client> players = new List<client>();
        public List<map.Map> loadedMaps = new List<map.Map>();

        private string hearbeatUrl {
            get {
                return HeartbeatServer +
                "?port=" + Port +
                "&name=" + (Config.server.serverName ?? "An awesome new MCjagg server!") +
                "&public=" + Public +
                "&max=" + MaxPlayer +
                "&version=" + "7" +
                "&salt=" + salt +
                "&users=" + users + 
                "&software=MCJagg 0.599";
            }
        }

        public int users
        {
            get
            {
                return players.ToArray().Length;
            }
        }

        public server(string heartbeatServer, int port, string _motd, bool _public, int maxPlayer)
        {
            salt = generateSalt();
            HeartbeatServer = heartbeatServer;
            Public = _public;
            MaxPlayer = maxPlayer;
            motd = _motd;
            Port = port;
            map.Map.getMap(Config.server.spawnMap);
            map.Map.loadedMaps.ToArray()[0].initializePhysics();
        }

        private string generateSalt() {
            string table = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string _salt = string.Empty;

            byte[] randomBytes = new byte[16];

            rngCsp.GetBytes(randomBytes);
            for (int i = 0; i < randomBytes.Length; i++) {
                _salt += table[randomBytes[i] % table.Length];
            }

            return _salt;
        }

        private client playerAlreadyExist(string key) {
            foreach (client z in players) {
                if (z.key.Equals(key)) return z;
            }
            return null;
        }

        public bool sendHeartbeat() {
            WebClient webx = new WebClient();
            webx.Proxy = null;
            webx.UseDefaultCredentials = true;
            try
            {
                logger.log(webx.DownloadString(hearbeatUrl));
            }
            catch {
                return false;
            }
            return true;
        }

        /*public void removePlayer(client player) {
            if (playerAlreadyExist(player.key) != null)
            {
                logger.debugLog(playerAlreadyExist(player.key).ToString());
                players.Remove(playerAlreadyExist(player.key));
            }

            if (player.pingTh != null) {
                player.pingTh.Abort();
                try { player.ServerThread.Abort(); }
                catch { }
            }
        }*/

        public void broadcastMessage(string message)
        {
            logger.log("Broadcast: " + message);
            for (int i = 0; i < players.Count; i++)
            {
                players[i].sendMessage(message, 0xFF);
            }
        }

       public void serve() {
           serv = this;
            int id = 0;
            tcpserver = new TcpListener(Port);
            tcpserver.Start();

            heartbeat = new Thread(new ThreadStart(delegate {
                logger.log("Heartbeat thread started!");
                while (true) {
                    if (!sendHeartbeat()) {
                        logger.errorLog("Failed to send Heartbeat!");
                    }
                    Thread.Sleep(45000);
                }
            }));
            heartbeat.Start();
            AntiCheat e = new AntiCheat(map.Map.getMap("main"));
            e.initialize();
            while (true) {

                TcpClient currtcp = tcpserver.AcceptTcpClient();

                client newPlayer = new client(currtcp,this,id);
                string ipddr = ((IPEndPoint)currtcp.Client.RemoteEndPoint).Address.ToString();
                logger.log("Client joined "+ipddr);
                if (!newPlayer.verify() && !ipddr.Equals("127.0.0.1"))
                {
                    newPlayer.disconnect("Login failed, please try again!"); // TODO: Config loading string 
                    continue;
                }
                else if (newPlayer.isBanned)
                {
                    newPlayer.disconnect(Config.server.bannedMessage);
                    continue;
                }
                else if (playerAlreadyExist(newPlayer.key)!=null) {
                    client z = playerAlreadyExist(newPlayer.key);
                    z.disconnect("Your account logged in again!");
                }
                newPlayer.setMotd(this.motd);
                newPlayer.serve();
                
                newPlayer.joinWorld(map.Map.loadedMaps.ToArray()[0]);
               /* for (int i = 0; i < players.Count; i++)
                {
                    if (players[i] != newPlayer)
                    {
                        if (players[i].currentMap == newPlayer.currentMap)
                        {
                            players[i].sendSpawn((byte)newPlayer.id, newPlayer.name, newPlayer.currentMap.getSpawnLocationX(), newPlayer.currentMap.getSpawnLocationY(), newPlayer.currentMap.getSpawnLocationZ(), 0, 0);
                            newPlayer.sendSpawn((byte)players[i].id, players[i].name, players[i].lastX, players[i].lastY, players[i].lastZ, 0, 0);
                        }
                    }
                }*/
                newPlayer.sendMessage("&aHey and welcome we are currently testing the McJagg Server.", 0xFF);
                newPlayer.sendMessage("&atype '/goto guest' to build", 0xFF);

                Console.WriteLine("spawnX = " + newPlayer.currentMap.getSpawnLocationX());
                newPlayer.sendSpawn(
                    0xFF,
                    newPlayer.name,
                    newPlayer.currentMap.getSpawnLocationX(),
                    newPlayer.currentMap.getSpawnLocationY(),
                    newPlayer.currentMap.getSpawnLocationZ(),
                    0,
                    0);

                newPlayer.startReadThread();

                newPlayer.playerObject = new Player(newPlayer, newPlayer.name);

                MainWindow.addOnlinePlayer(newPlayer.playerObject);
                MainWindow.updatePlayerMap(newPlayer.playerObject, newPlayer.currentMap);

                players.Add(newPlayer);

                
                logger.log("Player "+newPlayer.name+" joined the server!");
                broadcastMessage("# Player " + newPlayer.name + " joined the server :-)");
                id++;
            }
        }

        public void close() {
            serveth.Abort();
        }
    }
}
