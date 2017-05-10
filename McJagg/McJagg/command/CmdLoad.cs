using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdLoad : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            ((Player)executor).getMap().initializePhysics();
            server.server.serv.broadcastMessage("&3Physics loaded in "+ ((Player)executor).getMap().getName());
        }

        public override int getDefaultPermissionLevel()
        {
            return 600;
        }

        public override string getName()
        {
            return "load";
        }

        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
            };
        }
    }
}
