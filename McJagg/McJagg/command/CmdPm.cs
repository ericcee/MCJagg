using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdPm : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "pm";
        }

        public override string[] getAliases() { return new string[] { "tell", "message", "msg" }; }

        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.ONLINE_PLAYER_NOT_NULL, ParamType.STRING_WITH_SPACES}
            };
        }

        public override int getDefaultPermissionLevel()
        {
            return 0;
        }

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            arguments[0].onlinePlayerValue.client.sendMessage(executor.name()+" says to you: "+arguments[1].stringValue);
        }
    }
}
