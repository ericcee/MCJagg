using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdUndo : Command
    {
        public override string getHelp()
        {
            return
                "&2Parameters of /undo:\n" +
                //"&a- &b[username]\n"+
                "&a- &b[integer] [sec|min|hour|day|year]&2 Maximum depth in time&a (required)\n" +
                "&a- &b[username]&a (required)\n" +
                "&a- &bplacement&a (optional)\n" +
                "&a- &bremoval&a (optional)\n" +
                "&a- &barea&a (optional)\n" +

                "&2Parameters of /redo:\n" +
                //"&a- &b[username]\n"+
                "&a- &b[integer] [sec|min|hour|day|year]&2 Minimum depth in time&a (optional)\n" +
                "&a- &b[username]&a (optional)\n" +
                "&a- &b[u-code]&a (optional)\n" +
                "&a- &bforce&a (optional)\n" +
                "&a- &barea&a (optional)\n" +
                "";
        }
        public override string getName()
        {
            return "undo";
        }
        public override string[] getAliases() { return new string[] { "redo" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
                new ParamType[]{ParamType.STRING_WITH_SPACES},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 100;
        }
        private static bool isValidDimension(int dim)
        {
            return dim == 16 || dim == 32 || dim == 64 || dim == 128 || dim == 256 || dim == 512 || dim == 1024;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            {
                if (overload == 0 && alias == "undo")
                {
                    executor.commandOutput(getHelp());
                    return;
                }
                string[] parts = new string[0];
                if (overload == 1) parts = arguments[0].stringValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int prevInt = 0;
                bool prevWasInt = false;
                OfflinePlayer op = null;
                int timeAgoLimitSecs = -1;
                map.Block area1 = null, area2 = null;
                map.Map m = null;
                bool onlyPlacements = false;
                bool onlyRemoves = false;
                bool force = false;
                int uCode = -1;
                for (int i=0; i<parts.Length; i++)
                {
                    string p = parts[i].ToLower();
                    int ii;
                    if (int.TryParse(p, out ii))
                    {
                        prevInt = ii;
                        prevWasInt = true;
                    }
                    else if (prevWasInt)
                    {
                        if (p.ToLower().StartsWith("sec")) timeAgoLimitSecs = prevInt;
                        else if (p.ToLower().StartsWith("min")) timeAgoLimitSecs = prevInt * 60;
                        else if (p.ToLower().StartsWith("hour")) timeAgoLimitSecs = prevInt * 60 * 60;
                        else if (p.ToLower().StartsWith("day")) timeAgoLimitSecs = prevInt * 24 * 60 * 60;
                        else if (p.ToLower().StartsWith("year")) timeAgoLimitSecs = prevInt * 365 * 24 * 60 * 60;
                        else
                        {
                            executor.commandOutput("&4Unexpected parameter&b " + parts[i] + "&4 after time number!");
                            return;
                        }
                        prevWasInt = false;
                    }
                    else if (op == null && (op = OfflinePlayer.getOfflinePlayerIfExists(p)) != null) { }
                    else if (m == null && (m = map.Map.getMap(p)) != null) { }
                    else if (p == "force") force = true;
                    else if (p == "placement" || p == "place" || p=="placements") onlyPlacements = true;
                    else if (p == "removes" || p == "remove" || p=="destroys" || p=="removal" || p=="removals") onlyRemoves = true;
                    else if (p == "area" || p == "region")
                    {
                        area1 = getBlockInput(executor, "Please select the first corner of your region");
                        if (area1 == null) return;
                        area2 = getBlockInput(executor, "Please select the second corner of your region");
                        if (area2 == null) return;
                    }
                    else if (alias == "redo" && p[0] == 'u' && p.Length >= 2 && int.TryParse(p.Substring(1), out uCode)) { }
                    else
                    {
                        executor.commandOutput("&4Invalid parameter&b " + parts[i]);
                        return;
                    }
                }
                if (m == null && executor is Player) m = ((Player)executor).getMap();

                if (m == null)
                {
                    executor.commandOutput("&4No map selected!");
                    return;
                }
                if (op == null && alias == "undo") // player id not necessary for redo
                {
                    executor.commandOutput("&4No player selected!");
                    return;
                }
                if (timeAgoLimitSecs == -1 && alias == "undo") // time limit not necessary for redo
                {
                    executor.commandOutput("&4No time limit given!");
                    return;
                }

                if (alias == "undo")
                {
                    if (force)
                    {
                        executor.commandOutput("&4You can't use force on an undo, only on redo.");
                        return;
                    }
                    int[] amounts;
                    if (area1 == null) amounts = BlockLog.undo(this, m, op.getServerWideId(), timeAgoLimitSecs, onlyRemoves, onlyPlacements);
                    else
                    {
                        amounts = BlockLog.undo(this, m, op.getServerWideId(), timeAgoLimitSecs,
                            Math.Min(area1.getX(), area2.getX()),
                            Math.Min(area1.getY(), area2.getY()),
                            Math.Min(area1.getZ(), area2.getZ()),
                            Math.Max(area1.getX(), area2.getX()),
                            Math.Max(area1.getY(), area2.getY()),
                            Math.Max(area1.getZ(), area2.getZ()), onlyRemoves, onlyPlacements);
                    }

                    executor.commandOutput("&3" + amounts[0] + "&b changes on &3" + amounts[1] + "&b blocks have been undone!");
                }
                else if (alias == "redo")
                {
                    if (op == null && uCode == -1)
                    {
                        executor.commandOutput("&aForce keyword was ignored!");
                        if (area1 == null) BlockLog.listUndoIds(this, m, 0, 0, 0, m.getWidth()-1, m.getHeight()-1, m.getDepth()-1);
                        else
                        {
                            BlockLog.listUndoIds(this, m,
                            Math.Min(area1.getX(), area2.getX()),
                            Math.Min(area1.getY(), area2.getY()),
                            Math.Min(area1.getZ(), area2.getZ()),
                            Math.Max(area1.getX(), area2.getX()),
                            Math.Max(area1.getY(), area2.getY()),
                            Math.Max(area1.getZ(), area2.getZ()));
                        }
                        return;
                    }
                    int[] amounts = new int[]{0, 0};
                    if (uCode != -1 && op == null)
                    {
                        if (area1 == null) amounts = BlockLog.redoByUndoId(force, this, m, uCode, timeAgoLimitSecs);
                        else
                        {
                            amounts = BlockLog.redoByUndoId(force, this, m, uCode, timeAgoLimitSecs,
                                Math.Min(area1.getX(), area2.getX()),
                                Math.Min(area1.getY(), area2.getY()),
                                Math.Min(area1.getZ(), area2.getZ()),
                                Math.Max(area1.getX(), area2.getX()),
                                Math.Max(area1.getY(), area2.getY()),
                                Math.Max(area1.getZ(), area2.getZ()));
                        }
                    }
                    else if (uCode == -1 && op != null)
                    {
                        if (area1 == null) amounts = BlockLog.redoByPlayerId(force, this, m, op.getServerWideId(), timeAgoLimitSecs);
                        else
                        {
                            amounts = BlockLog.redoByPlayerId(force, this, m, op.getServerWideId(), timeAgoLimitSecs,
                                Math.Min(area1.getX(), area2.getX()),
                                Math.Min(area1.getY(), area2.getY()),
                                Math.Min(area1.getZ(), area2.getZ()),
                                Math.Max(area1.getX(), area2.getX()),
                                Math.Max(area1.getY(), area2.getY()),
                                Math.Max(area1.getZ(), area2.getZ()));
                        }
                    }
                    else if (uCode != -1 && op != null)
                    {
                        if (area1 == null) amounts = BlockLog.redoByPlayerIdAndUndoId(force, this, m, op.getServerWideId(), uCode, timeAgoLimitSecs);
                        else
                        {
                            amounts = BlockLog.redoByPlayerIdAndUndoId(force, this, m, op.getServerWideId(), uCode, timeAgoLimitSecs,
                                Math.Min(area1.getX(), area2.getX()),
                                Math.Min(area1.getY(), area2.getY()),
                                Math.Min(area1.getZ(), area2.getZ()),
                                Math.Max(area1.getX(), area2.getX()),
                                Math.Max(area1.getY(), area2.getY()),
                                Math.Max(area1.getZ(), area2.getZ()));
                        }
                    }

                    executor.commandOutput("&3" + amounts[0] + "&b changes on &3" + amounts[1] + "&b blocks have been redone!");
                }
            }
        }
    }
}
