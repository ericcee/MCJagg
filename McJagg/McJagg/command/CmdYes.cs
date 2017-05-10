using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdYes : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "yes";
        }
        public override string[] getAliases() { return new string[] { "confirm" }; }
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
            if (executor.commandInputer == null)
            {
                executor.commandOutput("&4There's nothing to say&a /yes&4 to!");
                return;
            }
            executor.commandInputer.setYesCancelInput(true);
        }
    }
}
