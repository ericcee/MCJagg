using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    public class CmdExpandMap : Command
    {
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.MAP, ParamType.INTEGER, ParamType.INTEGER, ParamType.INTEGER},
            };
        }

        public override string getName()
        {
            return "expandmap";
        }

        public override string getHelp()
        {
            return "/expandmap - expeand a map :)";
        }

        public override int getDefaultPermissionLevel()
        {
            return 100;
        }

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            arguments[0].mapValue.increaseSize(arguments[1].integerValue, arguments[2].integerValue, arguments[3].integerValue);
        }
    }
}
