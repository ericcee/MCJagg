using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdFreeze : Command
    {
        public override string getHelp()
        {
            return "Frezee a player";
        }

        public override string getName()
        {
            return "freeze";
        }
        public override string[] getAliases() { return new string[] { "unfreeze", "defreeze" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.OFFLINE_PLAYER},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 100;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            OfflinePlayer p = arguments[0].offlinePlayerValue;
            if (p == null)
            {
                executor.commandOutput("&4Cannot find that player");
                return;
            }
            if (alias == "freeze")
            {
                if (p.frozen)
                {
                    executor.commandOutput(p.getNameAndRank() + " &fwas already frozen!");
                }
                else
                {
                    p.frozen = true;
                    server.server.serv.broadcastMessage(p.getNameAndRank() + " &fhas been frozen!");
                    Config.saveOfflinePlayers();
                }
            }
            else
            {
                if (!p.frozen)
                {
                    executor.commandOutput(p.getNameAndRank() + " &fwas not frozen!");
                }
                else
                {
                    p.frozen = false;
                    server.server.serv.broadcastMessage(p.getNameAndRank() + " &fhas been unfrozen!");
                    Config.saveOfflinePlayers();
                }
            }
        }
    }
}
