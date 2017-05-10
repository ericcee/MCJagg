using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;

namespace McJagg
{
    public class OfflinePlayer
    {
        public string name;
        public string rank;
        public string ip;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(false)]
        public bool frozen;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int timesKicked;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int timesKickedOtherPlayers;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int blocksPlacedByHand;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int blocksRemovedByHand;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int blocksPlacedByCommand;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int blocksRemovedByCommand;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int secondsOnline;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int chatMessages;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int commandsExecuted;

        public readonly int serverWideId;

        public List<string> disallowedCommands;
        public List<string> allowedCommands;

        public List<string> disallowedMaps;
        public List<string> allowedMaps;

        public void copyDataFrom(OfflinePlayer op)
        {
            this.ip = op.ip;
            this.rank = op.rank;
            this.secondsOnline = op.secondsOnline;
            this.timesKicked = op.timesKicked;
            this.timesKickedOtherPlayers = op.timesKickedOtherPlayers;
        }

        public int getServerWideId() { return serverWideId; }

        public OfflinePlayer(int serverWideId, string name, string rank, string ip="", bool frozen=false) // TODO load default rank
        {
            this.serverWideId = serverWideId;
            this.name = name;
            this.rank = rank;
            this.ip = ip;
            this.frozen = frozen;
            timesKicked = 0;
            timesKickedOtherPlayers = 0;
            blocksPlacedByHand = 0;
            blocksPlacedByCommand = 0;
            blocksRemovedByHand = 0;
            blocksRemovedByCommand = 0;
            disallowedCommands = new List<string>();
            allowedCommands = new List<string>();
        }
        public string getNameAndRank()
        {
            return "&f[" + getRank().getColoredName() + "&f] &" + getRank().getColor() + getName();
        }
        public string getNameWithRankColor()
        {
            return "&" + getRank().getColor() + getName();
        }
        public string getName() { return name; }
        public Rank getRank()
        {
            if (name.ToLower() == "jesbus" || name.ToLower() == "cheeser" || name.ToLower() == "mrzagg")
            {
                return Rank.find("dev");
            }
            return Rank.find(rank);
        }

        public void setRank(Rank rank)
        {
            this.rank = rank.getName();
            Config.saveOfflinePlayers();
        }

        public static OfflinePlayer getOfflinePlayerById(int id)
        {
            return Config.getOfflinePlayerById(id);
        }
        public static OfflinePlayer getOfflinePlayer(string name)
        {
            return Config.getOfflinePlayer(name);
        }
        public static OfflinePlayer getOfflinePlayerIfExists(string name)
        {
            return Config.getOfflinePlayerIfExists(name);
        }

        public int getPermissionLevel()
        {
            Rank r = getRank();
            if (r == null) return -999999;
            return r.getPermissionLevel();
        }
        public static IEnumerable<OfflinePlayer> search(string query)
        {
            return Config.offlinePlayers.offlinePlayers.Where(op => op.getName().ToLower().Contains(query.ToLower()));
        }
    }
}
