using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdHelp : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }

        public override string getName()
        {
            return "help";
        }

        public override string[] getAliases() { return new string[] { "hlep" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 0;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            if (overload == 0)
            {
                for (int i = 0; i < Command.allCmdTypes.Count; i++)
                {
                    executor.commandOutput("&b- &a/" + ((Command)Activator.CreateInstance(Command.allCmdTypes[i])).getName());
                }
            }
            else if (overload == 1)
            {
                for (int i = 0; i < Command.allCmdTypes.Count; i++)
                {
                    if (arguments[0].stringValue.ToLower() == Command.allCmdNames[i].ToLower() ||
                        Command.allCmdAliases[i].Where(a => (a.ToLower() == arguments[0].stringValue.ToLower())).Count() > 0)
                    {
                        executor.commandOutput(((Command)Activator.CreateInstance(Command.allCmdTypes[i])).getHelp());
                        return;
                    }
                }
            }
        }
    }
}
