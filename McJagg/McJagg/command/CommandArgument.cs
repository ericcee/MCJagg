using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    public class CommandArgument
    {
        public Command.ParamType type;
        public Player onlinePlayerValue;
        public OfflinePlayer offlinePlayerValue;
        public int blockX, blockY, blockZ;
        public double playerX, playerY, playerZ;
        public byte playerYaw = 0, playerPitch = 0;
        public string stringValue;
        public int integerValue;
        public float floatValue;
        public map.Map mapValue;

        public static CommandArgument makeString(string str, Command.ParamType type)
        {
            CommandArgument arg = new CommandArgument();
            arg.type = type;
            arg.stringValue = str;
            return arg;
        }
        public static CommandArgument makeInt(int i)
        {
            CommandArgument arg = new CommandArgument();
            arg.type = Command.ParamType.INTEGER;
            arg.integerValue = i;
            return arg;
        }
        public static CommandArgument makeFloat(float f)
        {
            CommandArgument arg = new CommandArgument();
            arg.type = Command.ParamType.FLOAT;
            arg.floatValue = f;
            return arg;
        }
        public static CommandArgument makeMap(map.Map m)
        {
            CommandArgument arg = new CommandArgument();
            arg.type = Command.ParamType.MAP;
            arg.mapValue = m;
            return arg;
        }
        public static CommandArgument makeOnlinePlayer(Player p)
        {
            CommandArgument arg = new CommandArgument();
            arg.type = (p == null) ? Command.ParamType.ONLINE_PLAYER_NULL : Command.ParamType.ONLINE_PLAYER_NOT_NULL;
            arg.onlinePlayerValue = p;
            return arg;
        }
        public static CommandArgument makeOfflinePlayer(OfflinePlayer op)
        {
            CommandArgument arg = new CommandArgument();
            arg.type = Command.ParamType.OFFLINE_PLAYER;
            arg.offlinePlayerValue = op;
            return arg;
        }
        public static CommandArgument makeBlockLocation(int x, int y, int z)
        {
            CommandArgument arg = new CommandArgument();
            arg.type = Command.ParamType.BLOCK_LOCATION;
            arg.blockX = x;
            arg.blockY = y;
            arg.blockZ = z;
            return arg;
        }
        public static CommandArgument makePlayerLocation(double x, double y, double z, byte yaw=0, byte pitch=0)
        {
            CommandArgument arg = new CommandArgument();
            arg.type = Command.ParamType.PLAYER_LOCATION;
            arg.playerX = x;
            arg.playerY = y;
            arg.playerZ = z;
            arg.playerYaw = yaw;
            arg.playerPitch = pitch;
            return arg;
        }



        public string toString()
        {
            switch (type)
            {
                case Command.ParamType.STRING_WITHOUT_SPACES:
                case Command.ParamType.STRING_WITH_SPACES:
                    return "[STRING:" + stringValue + "]";
                case Command.ParamType.FLOAT:
                    return "[FLOAT:" + floatValue + "]";
                case Command.ParamType.INTEGER:
                    return "[INTEGER:" + integerValue + "]";
                case Command.ParamType.PLAYER_LOCATION:
                    return "[PLAYER_LOCATION:" + playerX + ", " + playerY + ", " + playerZ + "]";
                case Command.ParamType.BLOCK_LOCATION:
                    return "[BLOCK_LOCATION:" + blockX + ", " + blockY + ", " + blockZ + "]";
                case Command.ParamType.MAP:
                    return "[MAP:" + (mapValue==null ? "NULL" : mapValue.getName()) + "]";
            }
            return null;
        }
    }
}
