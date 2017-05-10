using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdCreateMap : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "createmap";
        }
        public override string[] getAliases() { return new string[] { "newmap" , "makemap" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES, ParamType.INTEGER, ParamType.INTEGER, ParamType.INTEGER},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 500;
        }
        private static bool isValidDimension(int dim)
        {
            return dim == 16 || dim == 32 || dim == 64 || dim == 128 || dim == 256 || dim == 512 || dim == 1024;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            string name = arguments[0].stringValue;
            int width = arguments[1].integerValue;
            int height = arguments[2].integerValue;
            int depth = arguments[3].integerValue;

            if (name.Contains('/'))
            {
                executor.commandOutput("Map name may not contain /");
                return;
            }
            if (map.Map.getMap(name) != null)
            {
                executor.commandOutput("A map with that name already exists!");
                return;
            }

            if (!isValidDimension(width) || !isValidDimension(height) || !isValidDimension(depth))
            {
                executor.commandOutput("Map dimensions must be powers of 2");
                return;
            }

            executor.commandOutput("Creating map...");

            map.Map m = map.Map.create(name, width, height, depth);

            Player p = executor as Player;

            if (p != null)
            {
                p.sendToMap(m, true);
                Console.WriteLine("spawnX = " + p.getMap().getSpawnLocationX());
                /*p.client.sendSpawn(
                    0xFF,
                    p.getName(),
                    p.getMap().getSpawnLocationX(),
                    p.getMap().getSpawnLocationY(),
                    p.getMap().getSpawnLocationZ(),
                    0,
                    0);*/
            }
        }
    }
}
