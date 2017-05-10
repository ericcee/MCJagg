using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdSetRankColor : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "setrankcolor";
        }
        public override string[] getAliases() { return new string[] { "rankcolor", "colorrank", "setrankcolour", "rankcolour", "colourrank" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES, ParamType.STRING_WITHOUT_SPACES,},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 500;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            string rankName = arguments[0].stringValue;
            string newColor = arguments[1].stringValue.Replace("&", "").Replace("%", "");
            Rank r = Rank.find(rankName);

            if (r == null)
            {
                executor.commandOutput("Rank &4" + r.getName() + " &fdoes not exist!");
            }
            else if (newColor.Length != 1)
            {
                executor.commandOutput("Invalid color code!");
            }
            else if ((newColor[0] >= 'a' && newColor[0] <= 'f') || (newColor[0] >= '0' && newColor[0] <= '9'))
            {
                r.setColor(newColor[0]);
                executor.commandOutput("Rank &a" + r.getName() + "&f was recolored to &" + newColor[0] + newColor[0] + newColor[0] + newColor[0] + newColor[0]);
            }
            else
            {
                executor.commandOutput("Invalid color code!");
            }
        }
    }
}
