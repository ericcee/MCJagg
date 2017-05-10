using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdSetSpawn : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "setspawn";
        }
        public override string[] getAliases() { return new string[] { "putspawnhere" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.PLAYER_LOCATION},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 1000;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            Player p = executor as Player;
            p.getMap().setSpawnLocation(arguments[0].playerX, arguments[0].playerY, arguments[0].playerZ, arguments[0].playerYaw, arguments[0].playerPitch);
            executor.commandOutput("Spawn position set!");
        }
    }
}
