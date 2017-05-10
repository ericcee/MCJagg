using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdGoto : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }

        public override string getName()
        {
            return "goto";
        }
        public override string[] getAliases() { return new string[] { "gotomap", "joinmap","g" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 0;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            string name = arguments[0].stringValue;
            map.Map m = map.Map.getMap(name);
            if (m==null)
            {
                executor.commandOutput("A map with that name does not exist!");
                return;
            }

            Player p = executor as Player;

            if (p != null)
            {
                if (!p.sendToMap(m, false))
                {
                    executor.commandOutput("You don't have permission to visit that map!");
                }
            }
            else
            {
                executor.commandOutput("You are not a player!");
            }
        }
    }
}
