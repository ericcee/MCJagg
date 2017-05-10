using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdViewLog : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "viewlog";
        }
        public override string[] getAliases() { return new string[] { }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
                new ParamType[]{ParamType.MAP, ParamType.BLOCK_LOCATION},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 0;
        }
        private static bool isValidDimension(int dim)
        {
            return dim == 16 || dim == 32 || dim == 64 || dim == 128 || dim == 256 || dim == 512 || dim == 1024;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            map.Map m;
            int x, y, z;
            if (overload == 0)
            {
                map.Block block = getBlockInput(executor, "&4Click or place the block you want to inspect");
                m = block.getMap();
                x = block.getX();
                y = block.getY();
                z = block.getZ();
            }
            else if (overload == 1)
            {
                m = arguments[0].mapValue;
                x = arguments[1].blockX;
                y = arguments[1].blockY;
                z = arguments[1].blockZ;
            }
            else
            {
                executor.commandOutput("&4Invalid arguments");
                return;
            }
            List<BlockLog.Entry> entries = BlockLog.viewLogEntries(m, x, y, z);
            executor.commandOutput("&0##&3 Logs of block &b" + x + "&3, &b" + y + "&3, &b" + z+"&3 :");
            if (entries.Count == 0) executor.commandOutput("&2No logs available!");
            else foreach (BlockLog.Entry entry in entries)
            {
                executor.commandOutput(map.Block.parse(entry.oldContent).ToString() + "&2 > &3" + map.Block.parse(entry.newContent).ToString() + "&2 by " + OfflinePlayer.getOfflinePlayerById(entry.playerId).getNameWithRankColor() + "&2 (&3" + BlockLog.timestampToString(entry.timestamp) + "&2)");
                if (entry.undoId != 0 && entry.redoTimestamp == 0)
                {
                    executor.commandOutput("&0^ &4undone by " + OfflinePlayer.getOfflinePlayerById(entry.undoPlayerId).getNameWithRankColor() + "&4 "+BlockLog.timestampToString(entry.undoTimestamp)+"&2 (&4u"+entry.undoId+"&2)");
                }
                else if (entry.undoId == 0 && entry.redoTimestamp != 0)
                {
                    executor.commandOutput("&0^ &4undone by " + OfflinePlayer.getOfflinePlayerById(entry.undoPlayerId).getNameWithRankColor() + "&4 " + BlockLog.timestampToString(entry.undoTimestamp) + "&2 (&4u" + entry.undoIdBeforeRedo + "&2)");
                    executor.commandOutput("&0^ &4redone at &b" + BlockLog.timestampToString(entry.redoTimestamp));
                }
                else if (entry.undoId != 0 && entry.redoTimestamp != 0)
                {
                    executor.commandOutput("&0^ &4undone by undo code&b u" + entry.undoIdBeforeRedo);
                    executor.commandOutput("&0^ &4redone at &b" + BlockLog.timestampToString(entry.redoTimestamp));
                    executor.commandOutput("&0^ &4undone again by " + OfflinePlayer.getOfflinePlayerById(entry.undoPlayerId).getNameWithRankColor() + "&4 " + BlockLog.timestampToString(entry.undoTimestamp) + "&2 (&4u" + entry.undoIdBeforeRedo + "&2)");
                }
            }
        }
    }
}
