using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdRenameRank : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "renamerank";
        }
        public override string[] getAliases() { return new string[] { "namerank", "setrankname", "changerankname", "rankrename", "rankname" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES, ParamType.STRING_WITHOUT_SPACES,},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 1000;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            string oldName = arguments[0].stringValue;
            string newName = arguments[1].stringValue;
            Rank r = Rank.find(oldName);

            if (r == null)
            {
                executor.commandOutput("Rank &4" + r.getName() + " &fdoes not exist!");
            }
            else
            {
                oldName = r.getName();
                r.setName(newName);
                executor.commandOutput("Rank &" + r.getColor() + oldName + "&f was renamed to " + r.getColoredName());
            }
        }
    }
}
