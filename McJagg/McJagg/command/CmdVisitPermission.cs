using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdVisitPermission : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "visitpermission";
        }
        public override string[] getAliases() { return new string[] {
            "permissionvisit",
            "setvisitpermission",
            "mapvisitpermission",
            "permissionlevelvisit",
            "setvisitpermissionlevel",
            "mapvisitpermissionlevel",
            "visitpermissionlevel",
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
            int lvl = int.MaxValue;
            if (overload == 0)
            {
                executor.commandOutput("Map &a"+m.getName()+"&f has visit permission level &a"+m.getVisitPermissionLevel());
                lvl = m.getVisitPermissionLevel();
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
                m.setVisitPermissionLevel(level);
                executor.commandOutput("Permission level of map &a" + m.getName() + "&f was set to &a" + level);
                lvl = level;
            }
            try { executor.commandOutput("Which corresponds to rank " + Rank.findRankByPermissionLevel(lvl).getColoredName()); }
            catch { executor.commandOutput("Which does not corrspond to any rank!"); }
        }
    }
}
