using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdTeleport : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "teleport";
        }
        public override string[] getAliases() { return new string[] { "tp" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.PLAYER_LOCATION},
                new ParamType[]{ParamType.ONLINE_PLAYER_NOT_NULL, ParamType.PLAYER_LOCATION},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 100;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            if (overload == 1)
            {
                executor.commandOutput("Teleporting " + arguments[0].onlinePlayerValue.getName() + " to " + arguments[1].playerX + " " + arguments[1].playerY + " " + arguments[1].playerZ);
                arguments[0].onlinePlayerValue.teleportTo(arguments[1].playerX, arguments[1].playerY, arguments[1].playerZ);
            }
            else if (overload == 0)
            {
                executor.commandOutput("Teleporting you to " + arguments[0].playerX + " " + arguments[0].playerY + " " + arguments[0].playerZ);
                ((Player)executor).teleportTo(arguments[0].playerX, arguments[0].playerY, arguments[0].playerZ);
            }
        }
    }
}
