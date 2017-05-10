using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdUnload : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            ((Player)executor).getMap().unloadPhysics();
            server.server.serv.broadcastMessage("&3Physics unloaded in " + ((Player)executor).getMap().getName());
        }

        public override int getDefaultPermissionLevel()
        {
            return 1000;
        }

        public override string getName()
        {
            return "unload";
        }

        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{}
            };
        }
    }
}
