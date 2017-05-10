using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdTemplate : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        // The name of this command
        public override string getName()
        {
            return "template";
        }

        // Here you can optionally supply different names for this command
        public override string[] getAliases() { return new string[] { "templ", "tmpl" }; }

        // Here you should list the combinations of parameter types that this command accepts.
        // Possible types are:
        //
        // ParamType.BLOCK_LOCATION     (integer x y z coordinates)
        // ParamType.PLAYER_LOCATION    (floating point x y z coordinates)
        //
        // ParamType.INTEGER
        // ParamType.FLOAT
        //
        // ParamType.ONLINE_PLAYER_NULL (you will receive a Player object, it might be null if the user enters a non-existent player name)
        // ParamType.ONLINE_PLAYER_NOT_NULL (you will receive a Player object, it is guaranteed not to be null)
        // ParamType.OFFLINE_PLAYER     (you will receive an OfflinePlayer object. note that this ParamType also accepts online players)
        // 
        // If you need to parse all or some arguments yourself, use only this one, or this one at the end:
        // ParamType.STRING_WITH_SPACE

        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.ONLINE_PLAYER_NOT_NULL, ParamType.PLAYER_LOCATION}
            };
        }

        // The default permission level for this command.
        // This can be overriden by ranks.json
        public override int getDefaultPermissionLevel()
        {
            return 0;
        }

        // You should use executor.commandOutput("message"); to produce command output
        // arguments array will contain the arguments to your command.

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            executor.commandOutput("Server should now teleport " + arguments[0].onlinePlayerValue.getNameAndRank() + " to " + arguments[1].playerX + " " + arguments[1].playerY + " " + arguments[1].playerZ);
        }
    }
}
