using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace McJagg.command
{
    class CmdMaps : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "maps";
        }
        public override string[] getAliases() { return new string[] { "listmaps" , "maplist" }; }
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
            string[] names = Directory.GetDirectories("maps");
            executor.commandOutput("&aThere are &b"+names.Length+"&a maps on this server:");
            foreach (string name in names)
            {
                executor.commandOutput(" - " + name);
            }
        }
    }
}
