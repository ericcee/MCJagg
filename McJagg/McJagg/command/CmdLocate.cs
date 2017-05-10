using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdLocate : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "locate";
        }
        public override string[] getAliases() { return new string[] { }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 100;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            Player p = executor as Player;
            executor.commandInputer.setLocationInput(p.getLocation());
        }
    }
}
