using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdSpawn : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "spawn";
        }
        public override string[] getAliases() { return new string[] { "gotospawn" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 0;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            Player p = executor as Player;
            p.teleportTo(p.getMap().getSpawnLocation(), false);
        }
    }
}
