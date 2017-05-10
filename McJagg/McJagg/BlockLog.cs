using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace McJagg
{
    class BlockLog
    {
        // Static stuff
        private static List<BlockLog> loadedBlockLogs = new List<BlockLog>();
        private static List<map.Map> loadedBlockLogMaps = new List<map.Map>();
        private static BlockLog getBlockLog(map.Map map)
        {
            for (int i=0; i<loadedBlockLogMaps.Count; i++)
            {
                if (loadedBlockLogMaps[i] == map) return loadedBlockLogs[i];
            }
            BlockLog bl = new BlockLog(map);
            loadedBlockLogs.Add(bl);
            loadedBlockLogMaps.Add(map);
            return bl;
        }
        public static void blockChanged(map.Map map, Player player, int x, int y, int z, int oldContent, int newContent)
        {
            getBlockLog(map).blockChanged(player.getServerWideId(), x, y, z, oldContent, newContent);
        }
        public static List<Entry> viewLogEntries(map.Map map, int x, int y, int z, int amount = 10, int beforeChangeId = int.MaxValue)
        {
            return getBlockLog(map).viewLogEntries(x, y, z, amount, beforeChangeId);
        }


        public static int[] undo(command.Command cmd, map.Map map, int targetPlayerId, int lastNseconds, bool onlyRemoves = false, bool onlyPlacements = false)
        {
            return getBlockLog(map).undo(cmd, targetPlayerId, lastNseconds, onlyRemoves, onlyPlacements);
        }
        public static int[] undo(command.Command cmd, map.Map map, int targetPlayerId, int lastNseconds, int minX, int minY, int minZ, int maxX, int maxY, int maxZ, bool onlyRemoves = false, bool onlyPlacements = false)
        {
            return getBlockLog(map).undo(cmd, targetPlayerId, lastNseconds, minX, minY, minZ, maxX, maxY, maxZ, onlyRemoves, onlyPlacements);
        }


        public static int[] redoByPlayerId(bool force, command.Command cmd, map.Map map, int targetPlayerId, int untilNsecondsAgo)
        {
            return getBlockLog(map).redoByPlayerId(force, cmd, targetPlayerId, untilNsecondsAgo);
        }
        public static int[] redoByPlayerId(bool force, command.Command cmd, map.Map map, int targetPlayerId, int untilNsecondsAgo, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            return getBlockLog(map).redoByPlayerId(force, cmd, targetPlayerId, untilNsecondsAgo, minX, minY, minZ, maxX, maxY, maxZ);
        }


        public static int[] redoByUndoId(bool force, command.Command cmd, map.Map map, int targetUndoId, int untilNsecondsAgo)
        {
            return getBlockLog(map).redoByUndoId(force, cmd, targetUndoId, untilNsecondsAgo);
        }
        public static int[] redoByUndoId(bool force, command.Command cmd, map.Map map, int targetUndoId, int untilNsecondsAgo, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            return getBlockLog(map).redoByUndoId(force, cmd, targetUndoId, untilNsecondsAgo, minX, minY, minZ, maxX, maxY, maxZ);
        }


        public static int[] redoByPlayerIdAndUndoId(bool force, command.Command cmd, map.Map map, int targetPlayerId, int targetUndoId, int untilNsecondsAgo)
        {
            return redoByPlayerIdAndUndoId(force, cmd, map, targetPlayerId, targetUndoId, untilNsecondsAgo, -1, -1, -1, map.getWidth()-1, map.getHeight()-1, map.getDepth()-1);
        }
        public static int[] redoByPlayerIdAndUndoId(bool force, command.Command cmd, map.Map map, int targetPlayerId, int targetUndoId, int untilNsecondsAgo, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            return getBlockLog(map).redo(force, cmd, targetUndoId, targetUndoId, untilNsecondsAgo, minX, minY, minZ, maxX, maxY, maxZ);
        }


        public static void listUndoIds(command.Command cmd, map.Map map, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            getBlockLog(map).listUndoIds(cmd, minX, minY, minZ, maxX, maxY, maxZ);
        }

        // Object stuff
        private Mutex fileMutex = new Mutex();
        private map.Map map;
        private readonly string blockLogFilePath;
        private long changeIdCount = 1;
        private int undoIdCount = 1;
        public static void shiftAllByOffset(map.Map map, int dx, int dy, int dz)
        {
            getBlockLog(map).shiftAllByOffset(dx, dy, dz);
        }
        private void shiftAllByOffset(int dx, int dy, int dz)
        {
            fileMutex.WaitOne();
            {
                FileStream fs = new FileStream(blockLogFilePath, FileMode.Open);

                byte[] entryFormatBuff = new byte[1];
                byte[] playerIdBuff = new byte[4];
                byte[] timestampBuff = new byte[8];
                byte[] xyzBuff = new byte[3 * 4];
                byte[] contentBeforeBuff = new byte[4];
                byte[] contentAfterBuff = new byte[4];
                byte[] changeIdBuff = new byte[8];
                byte[] undoIdBuff = new byte[4];
                byte[] undoTimestampBuff = new byte[8];
                byte[] undoPlayerIdBuff = new byte[4];
                byte[] undoIdBeforeRedoBuff = new byte[4];
                byte[] redoTimestampBuff = new byte[8];

                long posInFile = -1 - 4 - 8 - 3 * 4 - 4 - 4 - 8 - 4 - 8 - 4 - 4 - 8;

                while (true)
                {
                    if (fillBufferCheckEnd(entryFormatBuff, fs)) break;
                    if (fillBufferCheckEnd(playerIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(timestampBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(xyzBuff, fs)) throw new Exception("Corrupt block log file :-(");

                    int xx = BitConverter.ToInt32(xyzBuff, 0);
                    int yy = BitConverter.ToInt32(xyzBuff, 4);
                    int zz = BitConverter.ToInt32(xyzBuff, 8);

                    xx += dx;
                    yy += dy;
                    zz += dz;

                    fs.Seek(-12, SeekOrigin.Current);
                    fs.Write(BitConverter.GetBytes((int)xx), 0, 4);
                    fs.Write(BitConverter.GetBytes((int)yy), 0, 4);
                    fs.Write(BitConverter.GetBytes((int)zz), 0, 4);

                    if (fillBufferCheckEnd(contentBeforeBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(contentAfterBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(changeIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoTimestampBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoPlayerIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoIdBeforeRedoBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(redoTimestampBuff, fs)) throw new Exception("Corrupt block log file :-(");

                    posInFile += 1 + 4 + 8 + 3 * 4 + 4 + 4 + 8 + 4 + 8 + 4 + 4 + 8;
                }
                fs.Close();
            }
            fileMutex.ReleaseMutex();
        }
        private BlockLog(map.Map map)
        {
            fileMutex.WaitOne();
            {
                this.map = map;
                this.blockLogFilePath = "maps/" + map.getName() + "/blockLog.jbl";
                FileStream blockLogFileStream = new FileStream(blockLogFilePath, FileMode.OpenOrCreate);
                BinaryReader br = new BinaryReader(blockLogFileStream);

                // Read block log to find the highest undoId and changeId
                byte[] changeIdBuff = new byte[8];
                byte[] undoIdBuff = new byte[4];
                while (br.PeekChar() >= 0)
                {
                    fillBuffer(new byte[1 + 4 + 8 + 4 * 3 + 4 + 4], br);
                    fillBuffer(changeIdBuff, br);
                    fillBuffer(undoIdBuff, br);
                    
                    long changeId = BitConverter.ToInt64(changeIdBuff, 0);
                    int undoId = BitConverter.ToInt32(undoIdBuff, 0);
                    
                    if (changeId >= changeIdCount) changeIdCount = changeId + 1;
                    if (undoId >= undoIdCount) undoIdCount = undoId + 1;


                    fillBuffer(new byte[8 + 4], br);
                    fillBuffer(undoIdBuff, br); // undoIdBeforeRedo
                    fillBuffer(new byte[8], br); // redoTimestmap

                    undoId = BitConverter.ToInt32(undoIdBuff, 0);

                    if (undoId >= undoIdCount) undoIdCount = undoId + 1;
                }

                logger.log("changeIdCount=" + changeIdCount + " undoIdCount=" + undoIdCount);

                try { br.Close(); }
                catch { }
                try { blockLogFileStream.Close(); }
                catch { }
            }
            fileMutex.ReleaseMutex();
        }
        private void blockChanged(int playerId, int x, int y, int z, int oldContent, int newContent)
        {
            fileMutex.WaitOne();
            {
                FileStream fs = new FileStream(blockLogFilePath, FileMode.Append);
                fs.WriteByte(0x01);
                fs.Write(BitConverter.GetBytes((int)playerId), 0, 4);
                fs.Write(BitConverter.GetBytes((long)getTimestampNow()), 0, 8);
                fs.Write(BitConverter.GetBytes((int)x), 0, 4);
                fs.Write(BitConverter.GetBytes((int)y), 0, 4);
                fs.Write(BitConverter.GetBytes((int)z), 0, 4);
                fs.Write(BitConverter.GetBytes((int)oldContent), 0, 4);
                fs.Write(BitConverter.GetBytes((int)newContent), 0, 4);
                fs.Write(BitConverter.GetBytes((long)(changeIdCount++)), 0, 8);
                fs.Write(BitConverter.GetBytes((int)0), 0, 4);

                fs.Write(BitConverter.GetBytes((long)0), 0, 8);
                fs.Write(BitConverter.GetBytes((int)0), 0, 4);
                fs.Write(BitConverter.GetBytes((int)0), 0, 4);
                fs.Write(BitConverter.GetBytes((long)0), 0, 8);

                fs.Flush();
                fs.Close();
            }
            fileMutex.ReleaseMutex();
        }
        private List<Entry> viewLogEntries(int x, int y, int z, int amount, int beforeChangeId)
        {
            List<Entry> ret = new List<Entry>();

            fileMutex.WaitOne();
            {
                FileStream blockLogFileStream = new FileStream(blockLogFilePath, FileMode.OpenOrCreate);
                BinaryReader br = new BinaryReader(blockLogFileStream);

                byte[] entryFormatBuff = new byte[1];
                byte[] playerIdBuff = new byte[4];
                byte[] timestampBuff = new byte[8];
                byte[] xyzBuff = new byte[3 * 4];
                byte[] contentBeforeBuff = new byte[4];
                byte[] contentAfterBuff = new byte[4];
                byte[] changeIdBuff = new byte[8];
                byte[] undoIdBuff = new byte[4];
                byte[] undoTimestampBuff = new byte[8];
                byte[] undoPlayerIdBuff = new byte[4];
                byte[] undoIdBeforeRedoBuff = new byte[4];
                byte[] redoTimestampBuff = new byte[8];

                while (br.PeekChar() >= 0)
                {
                    fillBuffer(entryFormatBuff, br);
                    fillBuffer(playerIdBuff, br);
                    fillBuffer(timestampBuff, br);
                    fillBuffer(xyzBuff, br);
                    fillBuffer(contentBeforeBuff, br);
                    fillBuffer(contentAfterBuff, br);
                    fillBuffer(changeIdBuff, br);
                    fillBuffer(undoIdBuff, br);
                    fillBuffer(undoTimestampBuff, br);
                    fillBuffer(undoPlayerIdBuff, br);
                    fillBuffer(undoIdBeforeRedoBuff, br);
                    fillBuffer(redoTimestampBuff, br);

                    byte entryFormat = entryFormatBuff[0];
                    int playerId = BitConverter.ToInt32(playerIdBuff, 0);
                    long timestamp = BitConverter.ToInt64(timestampBuff, 0);
                    int xx = BitConverter.ToInt32(xyzBuff, 0);
                    int yy = BitConverter.ToInt32(xyzBuff, 4);
                    int zz = BitConverter.ToInt32(xyzBuff, 8);
                    int contentBefore = BitConverter.ToInt32(contentBeforeBuff, 0);
                    int contentAfter = BitConverter.ToInt32(contentAfterBuff, 0);
                    long changeId = BitConverter.ToInt64(changeIdBuff, 0);
                    int undoId = BitConverter.ToInt32(undoIdBuff, 0);
                    long undoTimestamp = BitConverter.ToInt64(undoTimestampBuff, 0);
                    int undoPlayerId = BitConverter.ToInt32(undoPlayerIdBuff, 0);
                    int undoIdBeforeRedo = BitConverter.ToInt32(undoIdBeforeRedoBuff, 0);
                    long redoTimestamp = BitConverter.ToInt64(redoTimestampBuff, 0);

                    if (xx != x || yy != y || zz != z) continue;

                    if (changeId >= beforeChangeId)
                    {
                        break;
                    }

                    ret.Add(new Entry(entryFormat, playerId, timestamp, xx, yy, zz, contentBefore, contentAfter, changeId, undoId, undoTimestamp, undoPlayerId, undoIdBeforeRedo, redoTimestamp));

                    if (ret.Count > amount)
                    {
                        ret.RemoveAt(0);
                    }
                }

                logger.log("changeIdCount=" + changeIdCount + " undoIdCount=" + undoIdCount);

                try { br.Close(); }
                catch { }
                try { blockLogFileStream.Close(); }
                catch { }
            }
            fileMutex.ReleaseMutex();

            return ret;
        }
        private int[] undo(command.Command cmd, int targetPlayerId, int lastNseconds, bool onlyRemoves = false, bool onlyPlacements = false)
        {
            return undo(cmd, targetPlayerId, lastNseconds, 0, 0, 0, map.getWidth()-1, map.getHeight()-1, map.getDepth()-1, onlyRemoves, onlyPlacements);
        }
        private int[] undo(command.Command cmd, int targetPlayerId, int lastNseconds, int minX, int minY, int minZ, int maxX, int maxY, int maxZ, bool onlyRemoves = false, bool onlyPlacements = false)
        {
            long sinceTimestamp = getTimestampNow() - lastNseconds * 1000;
            int blocksUndone = 0;
            int changesUndone = 0;

            List<int> blocksToUndoXs = new List<int>();
            List<int> blocksToUndoYs = new List<int>();
            List<int> blocksToUndoZs = new List<int>();
            List<long> blocksToUndoXYZhashes = new List<long>();
            List<long> blocksToUndoFileBytePos = new List<long>();
            List<bool> blocksToUndoConflictedWithCurrentState = new List<bool>();
            List<int> blocksToUndoContentAfter = new List<int>();
            List<int> blocksToUndoContentBefore = new List<int>();

            fileMutex.WaitOne();
            {
                int newUndoId = undoIdCount++;
                
                int newUndoPlayerId = -1;
                if (cmd.executor is Player)
                {
                    newUndoPlayerId = ((Player)cmd.executor).offlinePlayer.getServerWideId();
                }

                FileStream fs = new FileStream(blockLogFilePath, FileMode.Open);

                byte[] entryFormatBuff = new byte[1];
                byte[] playerIdBuff = new byte[4];
                byte[] timestampBuff = new byte[8];
                byte[] xyzBuff = new byte[3 * 4];
                byte[] contentBeforeBuff = new byte[4];
                byte[] contentAfterBuff = new byte[4];
                byte[] changeIdBuff = new byte[8];
                byte[] undoIdBuff = new byte[4];
                byte[] undoTimestampBuff = new byte[8];
                byte[] undoPlayerIdBuff = new byte[4];
                byte[] undoIdBeforeRedoBuff = new byte[4];
                byte[] redoTimestampBuff = new byte[8];

                long posInFile = -1 - 4 - 8 - 3 * 4 - 4 - 4 - 8 - 4 - 8 - 4 - 4 - 8;

                logger.log("onlyplace="+onlyPlacements+" onlyremove="+onlyRemoves);

                while (true)
                {
                    if (fillBufferCheckEnd(entryFormatBuff, fs)) break;
                    if (fillBufferCheckEnd(playerIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(timestampBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(xyzBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(contentBeforeBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(contentAfterBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(changeIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoTimestampBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoPlayerIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoIdBeforeRedoBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(redoTimestampBuff, fs)) throw new Exception("Corrupt block log file :-(");

                    posInFile += 1 + 4 + 8 + 3 * 4 + 4 + 4 + 8 + 4 + 8 + 4 + 4 + 8;

                    byte entryFormat = entryFormatBuff[0];
                    int playerId = BitConverter.ToInt32(playerIdBuff, 0);
                    long timestamp = BitConverter.ToInt64(timestampBuff, 0);
                    int xx = BitConverter.ToInt32(xyzBuff, 0);
                    int yy = BitConverter.ToInt32(xyzBuff, 4);
                    int zz = BitConverter.ToInt32(xyzBuff, 8);
                    int contentBefore = BitConverter.ToInt32(contentBeforeBuff, 0);
                    int contentAfter = BitConverter.ToInt32(contentAfterBuff, 0);
                    long changeId = BitConverter.ToInt64(changeIdBuff, 0);
                    int undoId = BitConverter.ToInt32(undoIdBuff, 0);


                    long posHash = (xx << 42) + (yy << 21) + (zz << 00);


                    // If the block change is outside the region or already undone we can skip over it
                    if (xx < minX || yy < minY || zz < minZ || xx > maxX || yy > maxY || zz > maxZ) continue;
                    if (undoId != 0) continue;

                    // If the block change does not match the parameters for undo, we should NOT try to undo
                    // actions on that same block that happened before it!
                    // For example, consider this sequence of events:
                    // 1. Player C places a block.
                    // 2. Player A removes block.
                    // 3. Player B places a block.
                    // 4. Player A removes it again.
                    // Now, if we undo all actions of player A, we DO NOT want to undo number 2 and number 4,
                    // we ONLY want to undo number 4!!!

                    int index = blocksToUndoXYZhashes.IndexOf(posHash);
                    if ((onlyPlacements && contentAfter == 0) ||
                        (onlyRemoves && contentAfter != 0) ||
                        timestamp < sinceTimestamp ||
                        playerId != targetPlayerId)
                    {
                        if (index != -1) // Here we remove change 2 in the example from the list of changes to undo
                        {
                            blocksToUndoXs.RemoveAt(index);
                            blocksToUndoYs.RemoveAt(index);
                            blocksToUndoZs.RemoveAt(index);
                            blocksToUndoXYZhashes.RemoveAt(index);
                            blocksToUndoFileBytePos.RemoveAt(index);
                            blocksToUndoConflictedWithCurrentState.RemoveAt(index);
                            blocksToUndoContentAfter.RemoveAt(index);
                            blocksToUndoContentBefore.RemoveAt(index);
                            changesUndone--;
                            if (blocksToUndoXYZhashes.IndexOf(posHash) == -1) blocksUndone--;
                        }
                        continue;
                    }

                    if (index != -1)
                    {
                        blocksToUndoXs.Insert(index, xx);
                        blocksToUndoYs.Insert(index, yy);
                        blocksToUndoZs.Insert(index, zz);
                        blocksToUndoXYZhashes.Insert(index, posHash);
                        blocksToUndoFileBytePos.Insert(index, posInFile);
                        blocksToUndoConflictedWithCurrentState.Insert(index, contentBefore != blocksToUndoContentAfter[index]);
                        blocksToUndoContentAfter.Insert(index, contentAfter);
                        blocksToUndoContentBefore.Insert(index, contentBefore);
                        changesUndone++;
                    }
                    else
                    {
                        blocksToUndoXs.Add(xx);
                        blocksToUndoYs.Add(yy);
                        blocksToUndoZs.Add(zz);
                        blocksToUndoXYZhashes.Add(posHash);
                        blocksToUndoFileBytePos.Add(posInFile);
                        blocksToUndoConflictedWithCurrentState.Add(map.getBlockContent(xx, yy, zz) != contentAfter);
                        blocksToUndoContentAfter.Add(contentAfter);
                        blocksToUndoContentBefore.Add(contentBefore);
                        blocksUndone++;
                        changesUndone++;
                    }
                }

                if (changesUndone == 0 || blocksUndone == 0)
                {
                    cmd.executor.commandOutput("&4Nothing to undo with those parameters!");
                    goto cancel;
                }

                bool forceThroughConflicts = false;
                int conflictCount = 0;
                for (int i = 0; i < blocksToUndoFileBytePos.Count; i++)
                {
                    if (blocksToUndoConflictedWithCurrentState[i])
                    {
                        conflictCount++;
                    }
                }
                if (conflictCount > 0)
                {
                    cmd.executor.commandOutput("&4There are " + conflictCount + " inconsistencies!");
                    cmd.executor.commandOutput("&4These are caused when block changes were not logged.");
                    cmd.executor.commandOutput("&4Undoing past unlogged changes may cause unexpected results!");
                    forceThroughConflicts = cmd.getConfirmation(cmd.executor, "&4Would you like to force through the inconsistencies?");
                }

                if (cmd.executor is Player)
                {
                    Player p = cmd.executor as Player;
                    int increment = Math.Max(1, blocksToUndoContentAfter.Count / 50);
                    for (int i = 0; i < blocksToUndoFileBytePos.Count; i+=increment)
                    {
                        p.flashBlock(map.getBlock(blocksToUndoXs[i], blocksToUndoYs[i], blocksToUndoZs[i]), true, blocksToUndoContentBefore[i]);
                    }
                    if (increment==1) cmd.executor.commandOutput("&bThe flashing blocks are the blocks to undo!");
                    else cmd.executor.commandOutput("&bThe flashing blocks are some of the blocks to undo!");
                }

                bool confirm = cmd.getConfirmation(cmd.executor, "&bAre you sure: undo &4"+changesUndone+"&b on &4"+blocksUndone+"&b blocks??");
                if (cmd.executor is Player)
                {
                    Player p = cmd.executor as Player;
                    p.clearFlashingBlocks();
                }
                if (!confirm)
                {
                    blocksUndone = 0;
                    changesUndone = 0;
                    goto cancel;
                }


                fs.Seek(0, SeekOrigin.Begin);
                posInFile = -1;// <!--- don' t use anymore form this point on

                int conflictsSkipped = 0;

                long newUndoTimestamp = getTimestampNow();

                for (int i = 0; i < blocksToUndoFileBytePos.Count; i++)
                {
                    if (!forceThroughConflicts)
                    {
                        if (blocksToUndoConflictedWithCurrentState[i])
                        {
                            if (i + 1 >= blocksToUndoXs.Count) break;
                            for (int j = i+1 ; j < blocksToUndoFileBytePos.Count; j++)
                            {
                                if (blocksToUndoXYZhashes[j] != blocksToUndoXYZhashes[i]) break;
                                else
                                {
                                    i++;
                                    conflictsSkipped++;
                                }
                            }
                            continue;
                        }
                    }

                    fs.Seek(blocksToUndoFileBytePos[i], SeekOrigin.Begin);

                    fillBufferCheckEnd(entryFormatBuff, fs);
                    fillBufferCheckEnd(playerIdBuff, fs);
                    fillBufferCheckEnd(timestampBuff, fs);
                    fillBufferCheckEnd(xyzBuff, fs);
                    fillBufferCheckEnd(contentBeforeBuff, fs);
                    fillBufferCheckEnd(contentAfterBuff, fs);
                    fillBufferCheckEnd(changeIdBuff, fs);
                    fillBufferCheckEnd(undoIdBuff, fs);
                    fillBufferCheckEnd(undoTimestampBuff, fs);
                    fillBufferCheckEnd(undoPlayerIdBuff, fs);
                    fillBufferCheckEnd(undoIdBeforeRedoBuff, fs);
                    fillBufferCheckEnd(redoTimestampBuff, fs);

                    byte entryFormat = entryFormatBuff[0];
                    int playerId = BitConverter.ToInt32(playerIdBuff, 0);
                    long timestamp = BitConverter.ToInt64(timestampBuff, 0);
                    int xx = BitConverter.ToInt32(xyzBuff, 0);
                    int yy = BitConverter.ToInt32(xyzBuff, 4);
                    int zz = BitConverter.ToInt32(xyzBuff, 8);
                    int contentBefore = BitConverter.ToInt32(contentBeforeBuff, 0);
                    int contentAfter = BitConverter.ToInt32(contentAfterBuff, 0);
                    long changeId = BitConverter.ToInt64(changeIdBuff, 0);
                    int undoId = BitConverter.ToInt32(undoIdBuff, 0);

                    map.setBlockContent(xx, yy, zz, contentBefore, true);

                    fs.Seek(-8-4-4-8-4, SeekOrigin.Current);

                    fs.Write(BitConverter.GetBytes(newUndoId), 0, 4);
                    fs.Write(BitConverter.GetBytes(newUndoTimestamp), 0, 8);
                    fs.Write(BitConverter.GetBytes(newUndoPlayerId), 0, 4);
                }

                if (conflictsSkipped>0) cmd.executor.commandOutput("&c" + conflictsSkipped + "&b changes have been skipped because of conflicts!");

            cancel:
                fs.Flush();
                fs.Close();
            }
            fileMutex.ReleaseMutex();
            return new int[] { changesUndone, blocksUndone };
        }

        /*private int[] redoAll(command.Command cmd, int targetPlayerId, int untilNsecondsAgo)
        {

        }*/
        private int[] redoByUndoId(bool force, command.Command cmd, int targetUndoId, int untilNsecondsAgo)
        {
            return redoByUndoId(force, cmd, targetUndoId, untilNsecondsAgo, -1, -1, -1, map.getWidth() - 1, map.getHeight() - 1, map.getDepth() - 1);
        }
        private int[] redoByUndoId(bool force, command.Command cmd, int targetUndoId, int untilNsecondsAgo, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            return redo(force, cmd, -1, targetUndoId, untilNsecondsAgo, minX, minY, minZ, maxX, maxY, maxZ);
        }

        private int[] redoByPlayerId(bool force, command.Command cmd, int targetPlayerId, int untilNsecondsAgo)
        {
            return redoByPlayerId(force, cmd, targetPlayerId, untilNsecondsAgo, -1, -1, -1, map.getWidth() - 1, map.getHeight() - 1, map.getDepth() - 1);
        }
        private int[] redoByPlayerId(bool force, command.Command cmd, int targetPlayerId, int untilNsecondsAgo, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            return redo(force, cmd, targetPlayerId, -1, untilNsecondsAgo, minX, minY, minZ, maxX, maxY, maxZ);
        }
        private int[] redo(bool force, command.Command cmd, int targetPlayerId, int targetUndoId, int untilNsecondsAgo, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            long beforeTimestamp = getTimestampNow() - untilNsecondsAgo * 1000;
            int blocksRedone = 0;
            int changesRedone = 0;

            List<int> blocksToRedoXs = new List<int>();
            List<int> blocksToRedoYs = new List<int>();
            List<int> blocksToRedoZs = new List<int>();
            List<long> blocksToRedoXYZhashes = new List<long>();
            List<long> blocksToRedoFileBytePos = new List<long>();
            List<bool> blocksToRedoConflictedWithNextChange = new List<bool>();
            List<int> blocksToRedoContentAfter = new List<int>();

            fileMutex.WaitOne();
            {
                FileStream fs = new FileStream(blockLogFilePath, FileMode.Open);

                byte[] entryFormatBuff = new byte[1];
                byte[] playerIdBuff = new byte[4];
                byte[] timestampBuff = new byte[8];
                byte[] xyzBuff = new byte[3 * 4];
                byte[] contentBeforeBuff = new byte[4];
                byte[] contentAfterBuff = new byte[4];
                byte[] changeIdBuff = new byte[8];
                byte[] undoIdBuff = new byte[4];
                byte[] undoTimestampBuff = new byte[8];
                byte[] undoPlayerIdBuff = new byte[4];
                byte[] undoIdBeforeRedoBuff = new byte[4];
                byte[] redoTimestampBuff = new byte[8];

                long posInFile = -1 - 4 - 8 - 3 * 4 - 4 - 4 - 8 - 4 - 8 - 4 - 4 - 8;

                int conflictCount = 0;

                while (true)
                {
                    if (fillBufferCheckEnd(entryFormatBuff, fs)) break;
                    if (fillBufferCheckEnd(playerIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(timestampBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(xyzBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(contentBeforeBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(contentAfterBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(changeIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoTimestampBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoPlayerIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoIdBeforeRedoBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(redoTimestampBuff, fs)) throw new Exception("Corrupt block log file :-(");

                    posInFile += 1 + 4 + 8 + 3 * 4 + 4 + 4 + 8 + 4 + 8 + 4 + 4 + 8;

                    byte entryFormat = entryFormatBuff[0];
                    int playerId = BitConverter.ToInt32(playerIdBuff, 0);
                    long timestamp = BitConverter.ToInt64(timestampBuff, 0);
                    int xx = BitConverter.ToInt32(xyzBuff, 0);
                    int yy = BitConverter.ToInt32(xyzBuff, 4);
                    int zz = BitConverter.ToInt32(xyzBuff, 8);
                    int contentBefore = BitConverter.ToInt32(contentBeforeBuff, 0);
                    int contentAfter = BitConverter.ToInt32(contentAfterBuff, 0);
                    long changeId = BitConverter.ToInt64(changeIdBuff, 0);
                    int undoId = BitConverter.ToInt32(undoIdBuff, 0);

                    long posHash = (xx << 42) + (yy << 21) + (zz << 00);

                    // If the block change is outside the region we can skip over it
                    // without worrying about anything.
                    if (xx < minX || yy < minY || zz < minZ || xx > maxX || yy > maxY || zz > maxZ) continue;

                    int lastIndex = blocksToRedoXYZhashes.LastIndexOf(posHash);

                    // If the block change is not the right player, or not undone,
                    // or not the right undo is,
                    // we should not try to redo anything before it!
                    if ((playerId != targetPlayerId && targetPlayerId != -1) ||
                        undoId == 0 ||
                        (undoId != targetUndoId && targetUndoId != -1))
                    {
                        if (!force)
                        {
                            if (lastIndex != -1) blocksRedone--;
                            while (blocksToRedoXYZhashes.Contains(posHash))
                            {
                                int index = blocksToRedoXYZhashes.IndexOf(posHash);
                                blocksToRedoXs.RemoveAt(index);
                                blocksToRedoYs.RemoveAt(index);
                                blocksToRedoZs.RemoveAt(index);
                                blocksToRedoXYZhashes.RemoveAt(index);
                                blocksToRedoFileBytePos.RemoveAt(index);
                                blocksToRedoConflictedWithNextChange.RemoveAt(index);
                                blocksToRedoContentAfter.RemoveAt(index);
                                changesRedone--;
                            }
                        }
                        continue;
                    }
                    
                    // If the block change is the right player and undone, but too late,
                    // we can just skip it.
                    if (timestamp > beforeTimestamp) continue;

                    int destIndex = 0;

                    if (lastIndex != -1)
                    {
                        destIndex = lastIndex + 1;
                    }

                    // Add the change to the redo list.
                    blocksToRedoXs.Insert(destIndex, xx);
                    blocksToRedoYs.Insert(destIndex, yy);
                    blocksToRedoZs.Insert(destIndex, zz);
                    blocksToRedoXYZhashes.Insert(destIndex, posHash);
                    blocksToRedoFileBytePos.Insert(destIndex, posInFile);
                    if (lastIndex != -1)
                    {
                        blocksToRedoConflictedWithNextChange.Insert(destIndex, blocksToRedoContentAfter[lastIndex] != contentBefore);
                    }
                    else
                    {
                        blocksToRedoConflictedWithNextChange.Insert(destIndex, false);
                        blocksRedone++;
                    }
                    blocksToRedoContentAfter.Insert(destIndex, contentAfter);
                    changesRedone++;
                }

                if (changesRedone == 0 || blocksRedone == 0)
                {
                    cmd.executor.commandOutput("&4Nothing to redo with those parameters!");
                    goto cancel;
                }

                bool forceThroughConflicts = false;

                if (conflictCount > 0)
                {
                    cmd.executor.commandOutput("&4There are " + conflictCount + " inconsistencies!");
                    cmd.executor.commandOutput("&4These can be caused when block changes were not logged.");
                    cmd.executor.commandOutput("&4Redoing through inconsistencies may cause unexpected results!");
                    forceThroughConflicts = cmd.getConfirmation(cmd.executor, "&4Would you like to force through the inconsistencies?");
                }

                if (cmd.executor is Player)
                {
                    Player p = cmd.executor as Player;
                    int increment = Math.Max(1, blocksToRedoContentAfter.Count / 50);
                    for (int i = 0; i < blocksToRedoFileBytePos.Count; i += increment)
                    {
                        p.flashBlock(map.getBlock(blocksToRedoXs[i], blocksToRedoYs[i], blocksToRedoZs[i]), true, blocksToRedoContentAfter[i]);
                    }
                    if (increment == 1) cmd.executor.commandOutput("&bThe flashing blocks are the blocks to redo!");
                    else cmd.executor.commandOutput("&bThe flashing blocks are some of the blocks to redo!");
                }

                if (force)
                {
                    bool confirmForce = cmd.getConfirmation(cmd.executor, "&4Are you sure you want to&0 force&4 a redo?\n&4This might overwrite blocks placed later!\n&4"+blocksRedone+" blocks are selected!");
                    if (!confirmForce)
                    {
                        if (cmd.executor is Player)
                        {
                            Player p = cmd.executor as Player;
                            p.clearFlashingBlocks();
                        }
                        blocksRedone = 0;
                        changesRedone = 0;
                        goto cancel;
                    }
                }

                bool confirm = cmd.getConfirmation(cmd.executor, "&bAre you sure: redo &4" + changesRedone + "&b on &4" + blocksRedone + "&b blocks??");
                if (cmd.executor is Player)
                {
                    Player p = cmd.executor as Player;
                    p.clearFlashingBlocks();
                }
                if (!confirm)
                {
                    blocksRedone = 0;
                    changesRedone = 0;
                    goto cancel;
                }


                fs.Seek(0, SeekOrigin.Begin);
                posInFile = -1;// <!--- don' t use anymore form this point on

                int conflictsSkipped = 0;

                long newRedoTimestamp = getTimestampNow();

                for (int i = 0; i < blocksToRedoFileBytePos.Count; i++)
                {
                    if (!forceThroughConflicts)
                    {
                        if (blocksToRedoConflictedWithNextChange[i])
                        {
                            if (i + 1 >= blocksToRedoXs.Count) break;
                            for (int j = i + 1; j < blocksToRedoFileBytePos.Count; j++)
                            {
                                if (blocksToRedoXYZhashes[j] != blocksToRedoXYZhashes[i]) break;
                                else
                                {
                                    i++;
                                    conflictsSkipped++;
                                }
                            }
                            continue;
                        }
                    }

                    fs.Seek(blocksToRedoFileBytePos[i], SeekOrigin.Begin);

                    fillBufferCheckEnd(entryFormatBuff, fs);
                    fillBufferCheckEnd(playerIdBuff, fs);
                    fillBufferCheckEnd(timestampBuff, fs);
                    fillBufferCheckEnd(xyzBuff, fs);
                    fillBufferCheckEnd(contentBeforeBuff, fs);
                    fillBufferCheckEnd(contentAfterBuff, fs);
                    fillBufferCheckEnd(changeIdBuff, fs);
                    fillBufferCheckEnd(undoIdBuff, fs);
                    fillBufferCheckEnd(undoTimestampBuff, fs);
                    fillBufferCheckEnd(undoPlayerIdBuff, fs);
                    fillBufferCheckEnd(undoIdBeforeRedoBuff, fs);
                    fillBufferCheckEnd(redoTimestampBuff, fs);

                    byte entryFormat = entryFormatBuff[0];
                    int playerId = BitConverter.ToInt32(playerIdBuff, 0);
                    long timestamp = BitConverter.ToInt64(timestampBuff, 0);
                    int xx = BitConverter.ToInt32(xyzBuff, 0);
                    int yy = BitConverter.ToInt32(xyzBuff, 4);
                    int zz = BitConverter.ToInt32(xyzBuff, 8);
                    int contentBefore = BitConverter.ToInt32(contentBeforeBuff, 0);
                    int contentAfter = BitConverter.ToInt32(contentAfterBuff, 0);
                    long changeId = BitConverter.ToInt64(changeIdBuff, 0);
                    int undoId = BitConverter.ToInt32(undoIdBuff, 0);

                    map.setBlockContent(xx, yy, zz, contentAfter, true);

                    fs.Seek(-8-4-4-8-4, SeekOrigin.Current);

                    fs.Write(BitConverter.GetBytes((int)0), 0, 4); // write undoId = 0
                    fs.Seek(8, SeekOrigin.Current); // skip the undoTimestamp
                    fs.Seek(4, SeekOrigin.Current); // skip the undoPlayerId
                    fs.Write(BitConverter.GetBytes((int)undoId), 0, 4); // write undoIdBeforeRedo = undoId
                    fs.Write(BitConverter.GetBytes((long)newRedoTimestamp), 0, 8); // write redoTimestamp
                }

                if (conflictsSkipped>0) cmd.executor.commandOutput("&c" + conflictsSkipped + "&b changes have been skipped because of conflicts!");

            cancel:
                try { fs.Flush(); }
                catch { }
                try { fs.Close(); }
                catch { }
            }
            fileMutex.ReleaseMutex();
            return new int[] { changesRedone, blocksRedone };
        }

        private void listUndoIds(command.Command cmd, int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
        {
            List<int> undoIds = new List<int>();
            List<List<int>> undoPlayerIds = new List<List<int>>();
            List<int> undoChangeCounts = new List<int>();
            List<long> undoTimestamps = new List<long>();
            List<int> undoUndoers = new List<int>();

            fileMutex.WaitOne();
            {
                FileStream fs = new FileStream(blockLogFilePath, FileMode.Open);

                byte[] entryFormatBuff = new byte[1];
                byte[] playerIdBuff = new byte[4];
                byte[] timestampBuff = new byte[8];
                byte[] xyzBuff = new byte[3 * 4];
                byte[] contentBeforeBuff = new byte[4];
                byte[] contentAfterBuff = new byte[4];
                byte[] changeIdBuff = new byte[8];
                byte[] undoIdBuff = new byte[4];
                byte[] undoTimestampBuff = new byte[8];
                byte[] undoPlayerIdBuff = new byte[4];
                byte[] undoIdBeforeRedoBuff = new byte[4];
                byte[] redoTimestampBuff = new byte[8];

                long posInFile = -1 - 4 - 8 - 3 * 4 - 4 - 4 - 8 - 4 - 8 - 4 - 4 - 8;

                while (true)
                {
                    if (fillBufferCheckEnd(entryFormatBuff, fs)) break;
                    if (fillBufferCheckEnd(playerIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(timestampBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(xyzBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(contentBeforeBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(contentAfterBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(changeIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoTimestampBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoPlayerIdBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(undoIdBeforeRedoBuff, fs)) throw new Exception("Corrupt block log file :-(");
                    if (fillBufferCheckEnd(redoTimestampBuff, fs)) throw new Exception("Corrupt block log file :-(");

                    posInFile += 1 + 4 + 8 + 3 * 4 + 4 + 4 + 8 + 4 + 8 + 4 + 4 + 8;

                    byte entryFormat = entryFormatBuff[0];
                    int playerId = BitConverter.ToInt32(playerIdBuff, 0);
                    long timestamp = BitConverter.ToInt64(timestampBuff, 0);
                    int xx = BitConverter.ToInt32(xyzBuff, 0);
                    int yy = BitConverter.ToInt32(xyzBuff, 4);
                    int zz = BitConverter.ToInt32(xyzBuff, 8);
                    int contentBefore = BitConverter.ToInt32(contentBeforeBuff, 0);
                    int contentAfter = BitConverter.ToInt32(contentAfterBuff, 0);
                    long changeId = BitConverter.ToInt64(changeIdBuff, 0);
                    int undoId = BitConverter.ToInt32(undoIdBuff, 0);
                    long undoTimestamp = BitConverter.ToInt64(undoTimestampBuff, 0);
                    int undoPlayerId = BitConverter.ToInt32(undoPlayerIdBuff, 0);
                    int undoIdBeforeRedo = BitConverter.ToInt32(undoIdBeforeRedoBuff, 0);
                    long redoTimestamp = BitConverter.ToInt64(redoTimestampBuff, 0);

                    if (xx < minX || yy < minY || zz < minZ || xx > maxX || yy > maxY || zz > maxZ) continue;

                    if (undoId == 0) continue;

                    int index = undoIds.IndexOf(undoId);

                    if (index == -1)
                    {
                        undoIds.Add(undoId);
                        List<int> list = new List<int>();
                        list.Add(playerId);
                        undoPlayerIds.Add(list);
                        undoChangeCounts.Add(1);
                        undoTimestamps.Add(undoTimestamp);
                        undoUndoers.Add(undoPlayerId);
                    }
                    else
                    {
                        if (!undoPlayerIds[index].Contains(playerId)) undoPlayerIds[index].Add(playerId);
                        undoChangeCounts[index]++;
                    }
                }

                if (minX == -1) cmd.executor.commandOutput("&3Undoes found on this map:");
                else cmd.executor.commandOutput("&3Undoes found in the selected region:");
                for (int i = 0; i < undoIds.Count; i++)
                {
                    string msg = "&bu" + undoIds[i] + "&a: &b" + undoChangeCounts[i] + "&a changes of:";
                    for (int j = 0; j < undoPlayerIds[i].Count; j++)
                    {
                        msg += " " + OfflinePlayer.getOfflinePlayerById(undoPlayerIds[i][j]).getNameWithRankColor();
                    }
                    cmd.executor.commandOutput(msg);
                }
                cmd.executor.commandOutput("&3Use &a/redo uCode&3 to redo a full undo");

                try { fs.Close(); }
                catch { }
            }
            fileMutex.ReleaseMutex();
        }
        public class Entry
        {
            public readonly byte entryFormat;
            public readonly int playerId;
            public readonly long timestamp;
            public readonly int x, y, z;
            public readonly int oldContent;
            public readonly int newContent;
            public readonly long changeId;
            public readonly int undoId;

            public readonly long undoTimestamp;
            public readonly int undoPlayerId;
            public readonly int undoIdBeforeRedo;
            public readonly long redoTimestamp;
            public Entry(byte entryFormat, int playerId, long timestamp, int x, int y, int z, int oldContent, int newContent, long changeId, int undoId,
                long undoTimestamp, int undoPlayerId, int undoIdBeforeRedo, long redoTimestamp)
            {
                this.entryFormat = entryFormat;
                this.playerId = playerId;
                this.timestamp = timestamp;
                this.x = x;
                this.y = y;
                this.z = z;
                this.oldContent = oldContent;
                this.newContent = newContent;
                this.changeId = changeId;
                this.undoId = undoId;
                this.undoTimestamp = undoTimestamp;
                this.undoPlayerId = undoPlayerId;
                this.undoIdBeforeRedo = undoIdBeforeRedo;
                this.redoTimestamp = redoTimestamp;
            }
        }

        ////// .jbl - Jagg Block Log - File format:

        // struct entry
        // {
        // byte entryFormat
        // int playerId
        // long timestamp
        // int x
        // int y
        // int z
        // int contentBefore
        // int contentAfter
        // long changeId
        // int undoId
        // long undoTimestamp
        // int undoPlayerId
        // int undoIdBeforeRedo
        // long redoTimestamp
        // }

        public static void fillBuffer(byte[] buff, BinaryReader stream)
        {
            int ptr = 0;
            while (ptr < buff.Length)
            {
                int amount = stream.Read(buff, ptr, buff.Length - ptr);
                if (amount <= 0)
                {
                    Thread.Sleep(1);
                }
                else
                {
                    ptr += amount;
                }
            }
        }
        public static bool fillBufferCheckEnd(byte[] buff, FileStream stream)
        {
            int sleepCount = 0;
            int ptr = 0;
            while (ptr < buff.Length)
            {
                int amount = stream.Read(buff, ptr, buff.Length - ptr);
                if (amount == 0) return true;
                if (amount <= 0)
                {
                    Thread.Sleep(1);
                    if (sleepCount++ > 5000) throw new Exception("fillBufferCheckEnd took too long!");
                }
                else
                {
                    ptr += amount;
                }
            }
            return false;
        }

        public static long getTimestampNow()
        {
            return (long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        public static string timestampToString(long timestamp)
        {
            long msAgo = getTimestampNow() - timestamp;
            DateTime theTime = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(timestamp);
            if (msAgo < 1000) return "" + (((double)msAgo) / 1000.0) + " sec ago";
            if (msAgo < 60 * 1000) return "" + (msAgo / 1000) + " secs ago";
            if (msAgo < 60 * 60 * 1000) return "" + (msAgo / 1000 / 60) + " mins ago";
            if (msAgo < 48 * 60 * 60 * 1000) return "" + (msAgo / 1000 / 60 / 60) + " hours ago";
            return theTime.ToString("yyyy-MM-dd at HH:mm:ss");
        }
    }
}
