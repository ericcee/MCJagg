using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdBuildPermission : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "buildpermission";
        }
        public override string[] getAliases() { return new string[] {
            "permissionbuild",
            "setbuildpermission",
            "mapbuildpermission",
            "permissionlevelbuild",
            "setbuildpermissionlevel",
            "mapbuildpermissionlevel",
            "buildpermissionlevel",
        }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.MAP},
                new ParamType[]{ParamType.MAP, ParamType.STRING_WITHOUT_SPACES}
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 1000;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            map.Map m = arguments[0].mapValue;
            if (overload == 0)
            {
                if (m.getPlaceBlockPermissionLevel() != m.getRemoveBlockPermissionLevel())
                {
                    executor.commandOutput("Map &a" + m.getName() + "&f has place block permission level &a" + m.getPlaceBlockPermissionLevel());
                    int lvlPb = m.getPlaceBlockPermissionLevel();
                    try { executor.commandOutput("Which corresponds to rank " + Rank.findRankByPermissionLevel(lvlPb).getColoredName()); }
                    catch { executor.commandOutput("&aWhich does not corrspond to any rank!"); }

                    executor.commandOutput("Map &a" + m.getName() + "&f has remove block permission level &a" + m.getRemoveBlockPermissionLevel());
                    int lvlRb = m.getRemoveBlockPermissionLevel();
                    try { executor.commandOutput("Which corresponds to rank " + Rank.findRankByPermissionLevel(lvlRb).getColoredName()); }
                    catch { executor.commandOutput("&aWhich does not corrspond to any rank!"); }
                }
                else
                {
                    executor.commandOutput("Map &a" + m.getName() + "&f has build permission level &a" + m.getRemoveBlockPermissionLevel());
                    int lvlRb = m.getRemoveBlockPermissionLevel();
                    try { executor.commandOutput("Which corresponds to rank " + Rank.findRankByPermissionLevel(lvlRb).getColoredName()); }
                    catch { executor.commandOutput("&aWhich does not corrspond to any rank!"); }
                }
            }
            else if (overload == 1)
            {
                string levelStr = arguments[1].stringValue;
                Rank r = Rank.find(levelStr);
                int level;
                if (!int.TryParse(levelStr, out level))
                {
                    if (r == null)
                    {
                        executor.commandOutput("Please supply a permission level or a rank.");
                        return;
                    }
                    level = r.getPermissionLevel();
                }
                m.setPlaceBlockPermissionLevel(level);
                m.setRemoveBlockPermissionLevel(level);
                executor.commandOutput("Build permission level of map &a" + m.getName() + "&f was set to &a" + level);
                try { executor.commandOutput("Which corresponds to rank " + Rank.findRankByPermissionLevel(level).getColoredName()); }
                catch { executor.commandOutput("&aWhich does not corrspond to any rank!"); }
            }
        }
    }
}
