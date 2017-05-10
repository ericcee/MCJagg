using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    public class CmdClear : Command
    {
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
            };
        }

        public override string getName()
        {
            return "clear";
        }

        public override string getHelp()
        {
            return "/clear cleares the screen";
        }

        public override int getDefaultPermissionLevel()
        {
            return 0;
        }

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            for (int i = 0; i < 12; i++) ((Player)executor).client.sendMessage("");
        }
    }
}
