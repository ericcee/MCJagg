using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json;
using System.IO;

namespace McJagg
{
    class Config
    {
        public static Server server = null;
        public static Ranks ranks = null;
        public static BannedLists bannedLists = null;




        public class Server
        {
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            [DefaultValue("A new MCJagg server")]
            public string serverName;
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            [DefaultValue("Welcome to this awesome new MCJagg server!")]
            public string motd;

            public bool debugLogMessages;
            public string heartbeatURL;
            public string bannedMessage;
            public int port;
            public bool offline;
            public string spawnMap;
            public string defaultRank;
            public bool afkMessagesEnabled;
            public int afkSeconds;
            public bool showGui;

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            [DefaultValue(1)]
            public int playerIdCounterDontChangeThis;

            public void setDefaults()
            {
                serverName = "A new MCJagg server";
                motd = "Welcome to this awesome new MCJagg server!";
                debugLogMessages = false;
                heartbeatURL = "http://www.classicube.net/heartbeat.jsp";
                bannedMessage = "You are banned from this server!";
                port = 25565;
                offline = false;
                spawnMap = "main";
                defaultRank = "guest";
                afkMessagesEnabled = true;
                afkSeconds = 30;
                playerIdCounterDontChangeThis = 1;
                showGui = true;
            }
        }
        public static void loadServerConfig()
        {
            if (File.Exists("server.json"))
            {
                server = JsonConvert.DeserializeObject<Server>(File.ReadAllText("server.json"));
            }
            else
            {
                server = new Server();
                server.setDefaults();
                saveServerConfig();
            }
        }
        public static void saveServerConfig()
        {
            File.WriteAllText("server.json", JsonConvert.SerializeObject(server, Formatting.Indented));
        }





        public class BannedLists
        {
            public List<string> bannedPlayers;
            public List<string> bannedIps;

            public void clear()
            {
                bannedPlayers = new List<string>();
                bannedIps = new List<string>();
            }
        }
        public static void loadBannedConfig()
        {
            if (File.Exists("banned.json"))
            {
                bannedLists = JsonConvert.DeserializeObject<BannedLists>(File.ReadAllText("banned.json"));
            }
            else
            {
                bannedLists = new BannedLists();
                bannedLists.clear();
                saveBannedConfig();
            }
        }
        public static void saveBannedConfig()
        {
            File.WriteAllText("banned.json", JsonConvert.SerializeObject(bannedLists, Formatting.Indented));
        }






        public class CommandPermission
        {
            public string command;
            public int permissionLevel;
            public CommandPermission() { }
            public CommandPermission(string command, int permissionLevel)
            {
                this.command = command;
                this.permissionLevel = permissionLevel;
            }
        }
        public class Ranks
        {
            public List<Rank> ranks;
            public Dictionary<string, int> commandPermissions;
            public void clear()
            {
                ranks = new List<Rank>();
                commandPermissions = new Dictionary<string, int>();
                ranks.Add(new Rank("guest", 0, false, 'f'));
                ranks.Add(new Rank("builder", 10, false));
                ranks.Add(new Rank("admin", 100, true, '4'));
                ranks.Add(new Rank("owner", 1000, true, '0'));
                ranks.Add(new Rank("dev", 10000, true, '9'));
            }
        }
        public static void loadRanksConfig()
        {
            if (File.Exists("ranks.json"))
            {
                ranks = JsonConvert.DeserializeObject<Ranks>(File.ReadAllText("ranks.json"));
                Rank dev = Rank.find("dev");
                if (dev == null)
                {
                    ranks.ranks.Add(new Rank("dev", 10000, true));
                    saveRanksConfig();
                }
                else
                {
                    dev.permissionLevel = 10000;
                    dev.color = '9';
                    dev.canDestroyBedrock = true;
                }
            }
            else
            {
                ranks = new Ranks();
                ranks.clear();
                saveRanksConfig();
            }
        }
        public static void saveRanksConfig()
        {
            File.WriteAllText("ranks.json", JsonConvert.SerializeObject(ranks, Formatting.Indented));
        }





        public static OfflinePlayers offlinePlayers = null;
        public class OfflinePlayers
        {
            public List<OfflinePlayer> offlinePlayers;
            public void clear()
            {
                offlinePlayers = new List<OfflinePlayer>();
            }
        }
        public static void loadOfflinePlayers()
        {
            if (File.Exists("players.json"))
            {
                offlinePlayers = JsonConvert.DeserializeObject<OfflinePlayers>(File.ReadAllText("players.json"));
                foreach (OfflinePlayer op in offlinePlayers.offlinePlayers)
                {
                    if (op.disallowedCommands == null) op.disallowedCommands = new List<string>();
                    if (op.allowedCommands == null) op.allowedCommands = new List<string>();
                    if (op.disallowedMaps == null) op.disallowedMaps = new List<string>();
                    if (op.allowedMaps == null) op.allowedMaps = new List<string>();
                }
                saveOfflinePlayers();
            }
            else
            {
                offlinePlayers = new OfflinePlayers();
                offlinePlayers.clear();
                saveOfflinePlayers();
            }
        }
        public static void saveOfflinePlayers()
        {
            File.WriteAllText("players.json", JsonConvert.SerializeObject(offlinePlayers, Formatting.Indented));
        }
        public static void reloadOfflinePlayers()
        {
            OfflinePlayers fromFile;
            if (File.Exists("players.json"))
            {
                fromFile = JsonConvert.DeserializeObject<OfflinePlayers>(File.ReadAllText("players.json"));
                foreach (OfflinePlayer op in fromFile.offlinePlayers)
                {
                    if (op.disallowedCommands == null) op.disallowedCommands = new List<string>();
                    if (op.allowedCommands == null) op.allowedCommands = new List<string>();
                    if (op.disallowedMaps == null) op.disallowedMaps = new List<string>();
                    if (op.allowedMaps == null) op.allowedMaps = new List<string>();
                }

            }
            else
            {
                fromFile = new OfflinePlayers();
                fromFile.clear();
            }

            for (int i = 0; i < fromFile.offlinePlayers.Count; i++)
            {
                OfflinePlayer opFromFile = fromFile.offlinePlayers[i];
                OfflinePlayer.getOfflinePlayer(opFromFile.getName()).copyDataFrom(opFromFile);
            }

            for (int i = 0; i < offlinePlayers.offlinePlayers.Count; i++)
            {
                OfflinePlayer opExisting = offlinePlayers.offlinePlayers[i];
                for (int j = 0; j < fromFile.offlinePlayers.Count; j++)
                {
                    OfflinePlayer opFromFile = fromFile.offlinePlayers[j];
                    if (opExisting.getName().ToLower() == opFromFile.getName().ToLower())
                    {
                        goto itStillExists;
                    }
                }
                offlinePlayers.offlinePlayers.RemoveAt(i);
                i--;
            itStillExists: ;
            }
        }

        public class XYZD
        {
            public double x, y, z;
            public byte yaw, pitch;
        }
        public class Map
        {
            public XYZD spawnLocation;
            public int visitPermissionLevel;
            public int placeBlockPermissionLevel;
            public int removeBlockPermissionLevel;
            public int editCommandPermissionLevel;
            public bool hacksAllowed;
            public bool physicsEnabled;
            public int physicsSpeed;
            public string motd;

            public void clear(map.Map map)
            {
                motd = "A awesome new Map!";
                spawnLocation = new XYZD();
                spawnLocation.x = map.getWidth() / 2;
                spawnLocation.y = map.getHeight() / 2 + 2;
                spawnLocation.z = map.getDepth() / 2;
                spawnLocation.yaw = 0;
                spawnLocation.pitch = 0;
                visitPermissionLevel = 0;
                placeBlockPermissionLevel = 0;
                removeBlockPermissionLevel = 0;
                editCommandPermissionLevel = 0;
                hacksAllowed = true;
                physicsEnabled = true; // TODO should physics be on by default?
                physicsSpeed = 1000;
            }
        }
        public static Config.Map loadMapConfig(map.Map map)
        {
            logger.debugLog("fetching config file for map " + map.getName());
            Config.Map config;
            if (File.Exists("maps/"+map.getName()+"/config.json"))
            {
                config = JsonConvert.DeserializeObject<Config.Map>(File.ReadAllText("maps/" + map.getName() + "/config.json"));
            }
            else
            {
                logger.debugLog("Creating config file for map " + map.getName());
                config = new Config.Map();
                config.clear(map);
                saveMapConfig(config, map);
            }
            return config;
        }
        public static void saveMapConfig(Config.Map config, map.Map map)
        {
            File.WriteAllText("maps/" + map.getName() + "/config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
        }





        public static OfflinePlayer getOfflinePlayerIfExists(string name)
        {
            for (int i = 0; i < offlinePlayers.offlinePlayers.Count; i++)
            {
                if (offlinePlayers.offlinePlayers[i].getName().ToLower() == name.ToLower())
                {
                    return offlinePlayers.offlinePlayers[i];
                }
            }
            return null;
        }
        public static OfflinePlayer getOfflinePlayerById(int id)
        {
            for (int i = 0; i < offlinePlayers.offlinePlayers.Count; i++)
            {
                if (offlinePlayers.offlinePlayers[i].getServerWideId() == id)
                {
                    return offlinePlayers.offlinePlayers[i];
                }
            }
            return null;
        }
        public static OfflinePlayer getOfflinePlayer(string name)
        {
            OfflinePlayer op = getOfflinePlayerIfExists(name);
            if (op == null)
            {
                offlinePlayers.offlinePlayers.Add(op = new OfflinePlayer(Config.server.playerIdCounterDontChangeThis++, name, rank: Config.server.defaultRank));
                saveOfflinePlayers();
                saveServerConfig();
            }
            return op;
        }
        public static void addRank(Rank rank)
        {
            ranks.ranks.Add(rank);
            saveRanksConfig();
        }
        public static Rank findRank(string name)
        {
            for (int i=0; i<ranks.ranks.Count; i++)
            {
                if (ranks.ranks[i].getName().ToLower() == name.ToLower())
                {
                    return ranks.ranks[i];
                }
            }
            if (name.ToLower() == "dev")
            {
                Rank dev = new Rank("dev", 10000, true, '9');
                addRank(dev);
                return dev;
            }
            return null;
        }
        public static Rank findRankByPermissionLevel(int level)
        {
            Rank r = null;
            int above = int.MaxValue;
            for (int i = 0; i < ranks.ranks.Count; i++)
            {
                int lvl = ranks.ranks[i].getPermissionLevel();
                if (lvl >= level)
                {
                    if (lvl-level < above)
                    {
                        r = ranks.ranks[i];
                        above = lvl - level;
                    }
                }
            }
            return r;
        }
        public static void banPlayer(string name)
        {
            bannedLists.bannedPlayers.Add(name);
            saveBannedConfig();
        }
        public static void banIp(string ip)
        {
            bannedLists.bannedIps.Add(ip);
            saveBannedConfig();
        }
    }
}
