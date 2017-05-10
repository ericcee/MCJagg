using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdSetrank : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "setrank";
        }
        public override string[] getAliases() { return new string[] { "rankset" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ParamType.OFFLINE_PLAYER, ParamType.STRING_WITHOUT_SPACES},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 1000;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            OfflinePlayer op = arguments[0].offlinePlayerValue;
            string rank = arguments[1].stringValue;
            Rank r = Rank.find(rank);
            if (r == null)
            {
                executor.commandOutput("That rank does not exist!");
                return;
            }
            if (op == null)
            {
                executor.commandOutput("Could not find that player!");
                return;
            }
            if (executor is Player)
            {
                Player p = executor as Player;
                if (p.getRank().getPermissionLevel() < op.getRank().getPermissionLevel() && !p.getRank().canSetrankOnPeopleRankedHigher)
                {
                    executor.commandOutput("&4Rank " + p.getRank().getColoredName() + "&4 cannot change rank of " + op.getRank().getColoredName());
                    return;
                }
                else if (p.getRank().getPermissionLevel() == op.getRank().getPermissionLevel() && !p.getRank().canPromotePeopleToItself)
                {
                    executor.commandOutput("&4Rank " + p.getRank().getColoredName() + "&4 cannot change rank of " + op.getRank().getColoredName());
                    return;
                }
            }
            if (true)
            {
                op.setRank(r);
                Player p = Player.getPlayer(op.getName());
                if (p != null) p.client.sendUserTypeUpdate();
                executor.commandOutput("Rank of &a" + op.getName() + " &fhas been set to " + op.getNameAndRank());
            }
        }
    }
}
