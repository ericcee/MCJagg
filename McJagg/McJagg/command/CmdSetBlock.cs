using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdSetBlock : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "setblock";
        }

        public override string[] getAliases() { return new string[] { "set", "place", "placeblock" }; }

        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES},
                new ParamType[]{ParamType.PLAYER_LOCATION, ParamType.STRING_WITHOUT_SPACES}
            };
        }

        public override int getDefaultPermissionLevel()
        {
            return 0;
        }

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            if (executor is Player)
            {
                Player p = executor as Player;
                if (overload == 0)
                {
                    p.getMap().setBlockContent((int)p.getX(), (int)p.getY(), (int)p.getZ(), (int)map.Block.Type.ADMINIUM, true);
                    executor.commandOutput("Placed on x=" + (int)p.getX() + " y=" + (int)p.getY() + " z=" + (int)p.getZ());
                }
                else if (overload == 1)
                {
                    p.getMap().setBlockContent((int)p.getX(), (int)p.getY(), (int)p.getZ(), map.Block.parse(arguments[0].stringValue), true);
                }
                else if (overload == 2)
                {
                    p.getMap().setBlockContent((int)arguments[0].playerX, (int)arguments[0].playerY, (int)arguments[0].playerZ, map.Block.parse(arguments[1].stringValue), true);
                }
                executor.commandOutput("Block placed!");
            }
            else
            {
                executor.commandOutput("Can't use this command as non-player");
            }
        }
    }
}
