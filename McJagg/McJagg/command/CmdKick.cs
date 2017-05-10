using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdKick : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            if (overload == 1)
            {
                server.server.serv.broadcastMessage(arguments[0].onlinePlayerValue.getNameAndRank() + "&f was &4kicked&f because " + arguments[1].stringValue);
                arguments[0].onlinePlayerValue.client.disconnect("&9"+arguments[1].stringValue);
            }
            else
            {
                server.server.serv.broadcastMessage(arguments[0].onlinePlayerValue.getNameAndRank() + "&f was &4kicked&f");
                arguments[0].onlinePlayerValue.client.disconnect("&9You've been kicked! :D");
            }
        }

        public override int getDefaultPermissionLevel()
        {
            return 300;
        }

        public override string getName()
        {
            return "kick";
        }

        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.ONLINE_PLAYER_NOT_NULL},
                new ParamType[]{ParamType.ONLINE_PLAYER_NOT_NULL, ParamType.STRING_WITH_SPACES},
            };
        }
    }
}
