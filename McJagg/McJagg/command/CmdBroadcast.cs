using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdBroadcast : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "broadcast";
        }

        public override string[] getAliases() { return new string[] { "say" }; }

        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.STRING_WITH_SPACES}
            };
        }

        public override int getDefaultPermissionLevel()
        {
            return 1000;
        }

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            server.server.serv.broadcastMessage(arguments[0].stringValue);
        }
    }
}
