using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdTest : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "test";
        }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]
            {
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES},
                new ParamType[]{ParamType.PLAYER_LOCATION, ParamType.FLOAT, ParamType.FLOAT},
                new ParamType[]{ParamType.INTEGER, ParamType.INTEGER, ParamType.INTEGER,},
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES, ParamType.INTEGER, ParamType.INTEGER,},
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES, ParamType.INTEGER, ParamType.FLOAT,},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 0;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            //Console.WriteLine("hello world overload=" + overload);
            executor.commandOutput("hello world overload="+overload);
            executor.commandOutput("I have " + arguments.Length + " arguments");
            for (int i=0; i<arguments.Length; i++)
            {
                executor.commandOutput(arguments[i].toString());
            }
        }
    }
}
