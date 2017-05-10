using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;

namespace McJagg
{
    public class Rank
    {
        public string name;
        public int permissionLevel;
        public bool canDestroyBedrock;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue('a')]
        public char color;
        public bool canSetrankOnPeopleRankedHigher;
        public bool canPromotePeopleToItself;

        public List<string> disallowedBlockTypes;
        public Rank(string name, int permissionLevel, bool canDestroyBedrock, char color='a')
        {
            this.name = name;
            this.permissionLevel = permissionLevel;
            this.canDestroyBedrock = canDestroyBedrock;
            this.color = color;
            canSetrankOnPeopleRankedHigher = permissionLevel >= 1000;
            canPromotePeopleToItself = permissionLevel >= 1000;
            disallowedBlockTypes = new List<string>();
        }
        public string getName() { return name; }
        public string getColoredName() { return "&" + color + name; }
        public int getPermissionLevel() { return permissionLevel; }
        public char getColor() { return color; }

        public bool canPlaceBlockType(int blockType)
        {
            if (blockType == (int)map.Block.Type.ADMINIUM) return canDestroyBedrock;
            for (int i=0; i<disallowedBlockTypes.Count; i++)
            {
                if (blockType == (int)map.Block.parse(disallowedBlockTypes[i]))
                {
                    return false;
                }
            }
            return true;
        }
        public bool canPlaceBlockType(map.Block.Type blockType)
        {
            return canPlaceBlockType((int)blockType);
        }
        public bool canPlaceBlockType(string blockType)
        {
            return canPlaceBlockType(map.Block.parse(blockType));
        }

        public void setName(string newName)
        {
            foreach (OfflinePlayer op in Config.offlinePlayers.offlinePlayers)
            {
                op.rank = newName;
            }
            name = newName;
            Config.saveOfflinePlayers();
            Config.saveRanksConfig();
        }
        public void setColor(char newColor)
        {
            this.color = newColor;
            Config.saveRanksConfig();
        }

        public static Rank find(string name)
        {
            return Config.findRank(name);
        }
        public static Rank findRankByPermissionLevel(int level)
        {
            return Config.findRankByPermissionLevel(level);
        }
        public static bool create(string name, int permissionLevel, bool canDestroyBedrock, char color='a')
        {
            if (find(name) != null) return false;
            Rank r = new Rank(name, permissionLevel, canDestroyBedrock, color);
            Config.addRank(r);
            return true;
        }
    }
}
