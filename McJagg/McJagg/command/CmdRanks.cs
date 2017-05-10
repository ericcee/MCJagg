using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdRanks : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "ranks";
        }
        public override string[] getAliases() { return new string[] { "listranks", "listrank", "rankslist", "ranklist" }; }
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
            executor.commandOutput("&aAll ranks on this server:");
            foreach (Rank r in McJagg.Config.ranks.ranks)
            {
                executor.commandOutput("&f- "+r.getColoredName());
            }
        }
    }
}
