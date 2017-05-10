using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdCreateRank : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }

        public override string getName()
        {
            return "createrank";
        }
        public override string[] getAliases() { return new string[] { "newrank", "makerank" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES, ParamType.INTEGER, ParamType.STRING_WITHOUT_SPACES,},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 0;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            string name = arguments[0].stringValue;
            int level = arguments[1].integerValue;
            string third = arguments[2].stringValue.ToLower();
            if (Rank.create(name, level, third == "yes" || third == "ja" || third == "ye"))
            {
                executor.commandOutput("Rank " + name + " with permission level " + level + " created!");
            }
            else
            {
                executor.commandOutput("Could not create rank! Maybe it already exists?");
            }
        }
    }
}
