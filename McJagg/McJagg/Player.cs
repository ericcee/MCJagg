using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace McJagg
{
    public class Player : command.CommandExecutor
    {

        public bool frozen { get { return offlinePlayer.frozen; } }

        public bool AFK {
            get { return afk; }
            set {
                if (value) {
                    server.server.serv.broadcastMessage(this.getName() + " is AFK");
                }
                else {
                    server.server.serv.broadcastMessage(this.getName() + " is no longer AFK");
                }
                afk = value;
            }
        }

        public bool isSwiming
        {
            get
            {
                if (getMap().getBlock((int)getX(),(int) getY(), (int) getZ()).getContent() == (int)map.Block.Type.WATER) return true;
                if (getMap().getBlock((int)getX(), (int)getY()-1, (int)getZ()).getContent() == (int)map.Block.Type.WATER) return true;
                if (getMap().getBlock((int)getX(), (int)getY()-2, (int)getZ()).getContent() == (int)map.Block.Type.WATER) return true;
                if (getMap().getBlock((int)getX(), (int)getY(), (int)getZ()).getContent() == (int)map.Block.Type.LAVA_FLOW) return true;
                if (getMap().getBlock((int)getX(), (int)getY() - 1, (int)getZ()).getContent() == (int)map.Block.Type.LAVA_FLOW) return true;
                if (getMap().getBlock((int)getX(), (int)getY() - 2, (int)getZ()).getContent() == (int)map.Block.Type.LAVA_FLOW) return true;
                return false;
            }
        }
        

        private bool afk = false;
        public bool probablyFalling = false;
        public Vector lastFallingPos = null;
        public static List<Player> players = new List<Player>();
        private double currSpeed = 0;
        public Vector lastFlyingDistance = null;

        private DateTime thistm = new DateTime();

        public double currSecSpeed {
            get
            {
                if (lastSecondPos != null)
                {
                    Vector thvec = (new Vector(this)) - lastSecondPos;
                    return (Math.Sqrt(Math.Pow(thvec.x, 2) + Math.Pow(thvec.z, 2))) - currSpeed;
                }
                return 0;
            }
        }
        public double fullSpeed = 0;

        public long lastMovement = 0; // for afk needs
        public Vector lastSecondPos; // determine speed

        public long loginTime
        {
            get {
                return (DateTime.Now-thistm).Ticks/TimeSpan.TicksPerMillisecond;
            }
        }

        public readonly OfflinePlayer offlinePlayer;
        public server.client client;


        public Player(server.client client, string name)
        {
            thistm = DateTime.Now;
            offlinePlayer = OfflinePlayer.getOfflinePlayer(name);
            this.client = client;
            players.Add(this);
        }

        public int getServerWideId() { return offlinePlayer.getServerWideId(); }

        public Location getLocation()
        {
            return new Location(getMap(), getX(), getY(), getZ(), getYaw(), getPitch());
        }

        public string getName()
        {
            return offlinePlayer.getName();
        }
        public string getNameAndRank()
        {
            return "&f[" + offlinePlayer.getRank().getColoredName() + "&f] &" + offlinePlayer.getRank().getColor() + offlinePlayer.getName();
        }
        public string getColoredName()
        {
            return "&" + offlinePlayer.getRank().getColor() + offlinePlayer.getName();
        }
        public Rank getRank()
        {
            return offlinePlayer.getRank();
        }
        public int getPermissionLevel()
        {
            return offlinePlayer.getPermissionLevel();
        }

        public map.Map getMap()
        {
            return client.currentMap;
        }

        public double getX()
        {
            return client.lastX;
        }

        public double getY()
        {
            return client.lastY;
        }

        public double getZ()
        {
            return client.lastZ;
        }

        public byte getYaw()
        {
            return client.lastYaw;
        }

        public byte getPitch()
        {
            return client.lastPitch;
        }

        private int roundBlockPos(double x)
        {
            

            return (int)x;
        }

        public int getPlayerGroundDistance()
        {
            int y=0;
            while (getMap().getBlock(roundBlockPos(this.getX()), ((int)this.getY()) - y - 2, roundBlockPos(this.getZ())).getContent() == 0)
            {
                y++;
            }
            return y;
        }

        public void teleportTo(double x, double y, double z)
        {
            teleportTo(x, y, z, getYaw(), getPitch());
        }
        public void teleportTo(double x, double y, double z, byte yaw, byte pitch)
        {
            client.Teleport((short)(x * 32), (short)(y * 32), (short)(z * 32), yaw, pitch);
        }
        public void teleportWithinMap(LocationWithinMap loc)
        {
            teleportTo(loc.getX(), loc.getY(), loc.getZ(), loc.getYaw(), loc.getPitch());
        }
        public void teleportTo(Location loc, bool overridePermission)
        {
            if (getMap() == loc.getMap()) teleportWithinMap(loc);
            else
            {
                sendToMap(loc.getMap(), loc, overridePermission);
            }
        }

        public bool sendToMap(map.Map destination, Location loc, bool overridePermission)
        {
            if (!overridePermission)
            {
                if (getPermissionLevel() < destination.getVisitPermissionLevel())
                {
                    return false;
                }
            }
            client.joinWorld(destination, loc);
            return true;
        }
        public bool sendToMap(map.Map destination, bool overridePermission)
        {
            if (!overridePermission)
            {
                if (getPermissionLevel() < destination.getVisitPermissionLevel())
                {
                    return false;
                }
            }
            client.joinWorld(destination);
            return true;
        }

        public void teleportTo(map.Block destination, bool overridePermission)
        {
            if (getMap() != destination.getMap())
            {
                sendToMap(destination.getMap(), overridePermission);
                while (client.isLoadingMap) { Thread.Sleep(5); }
            }
            // TODO client.Teleport(destination.getX(), destination.getY(), destination.getZ());
        }


        public static Player getPlayer(string name)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].getName().ToLower() == name.ToLower())
                {
                    return players[i];
                }
            }
            return null;
        }
        public static Player getPlayerStartingWith(string name, out bool multipleOnlineLikeThat)
        {
            Player p = getPlayer(name);
            if (p != null)
            {
                multipleOnlineLikeThat = false;
                return p;
            }
            List<Player> onlines = new List<Player>();
            Player shortest = null;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].getName().ToLower().StartsWith(name.ToLower()))
                {
                    onlines.Add(players[i]);
                    if (shortest == null || shortest.getName().Length > players[i].getName().Length) shortest = players[i];
                }
            }
            if (onlines.Count >= 2)
            {
                multipleOnlineLikeThat = true;
                return shortest;
            }
            else
            {
                multipleOnlineLikeThat = false;
                return shortest;
            }
        }
        public override void commandOutput(string message)
        {
            Console.WriteLine("Command message to player " + getName() + ": " + message);
            client.sendMessage("&a# &b" + message, 0xFF);
        }
        public static IEnumerable<Player> getPlayersInMap(map.Map map)
        {
            return players.Where((x) => (x.getMap() == map));
        }
        public override string name()
        {
            return getName();
        }







        private List<map.FlashingBlock> flashingBlocks = new List<map.FlashingBlock>();
        private Thread flashThread = null;
        public void clearFlashingBlocks()
        {
            flashThread = null;
            for (int i=0; i<flashingBlocks.Count; i++)
            {
                map.FlashingBlock fb = flashingBlocks[i];
                removeFlashingBlock(fb);
                i--;
            }
            flashingBlocks.Clear();
        }
        public void removeFlashingBlock(map.Block block)
        {
            for (int i=0; i<flashingBlocks.Count; i++)
            {
                map.FlashingBlock fb = flashingBlocks[i];
                if (fb.block == block)
                {
                    removeFlashingBlock(fb);
                    return;
                }
            }
        }
        public void removeFlashingBlock(map.FlashingBlock fb)
        {
            //logger.debugLog("removeFlashingBlock(" + fb.block.getX() + ", " + fb.block.getY() + ", " + fb.block.getZ() + ") removing from list...");
            flashingBlocks.Remove(fb);
            //logger.debugLog("removeFlashingBlock("+fb.block.getX()+", "+fb.block.getY()+", "+fb.block.getZ()+") sending setBlock packet...");
            if (flashingBlocks.Count == 0) flashThread = null;
            client.sendSetBlock(fb.block.getX(), fb.block.getY(), fb.block.getZ(), map.Block.getClientDisplayType(fb.block.getContent()));
            //logger.debugLog("removeFlashingBlock(" + fb.block.getX() + ", " + fb.block.getY() + ", " + fb.block.getZ() + ") packet sent!");
        }

        public void flashBlock(map.Block block, bool overridePrevious=true, int flashToBlockType = -1)
        {
            bool found = false;
            for (int i = 0; i < flashingBlocks.Count; i++)
            {
                if (flashingBlocks[i].block == block)
                {
                    if (overridePrevious)
                    {
                        flashingBlocks[i] = new map.FlashingBlock(block, flashToBlockType, 10);
                    }
                    else return;
                }
            }
            if (!found) flashingBlocks.Add(new map.FlashingBlock(block, flashToBlockType, 10));
            if (flashThread == null)
            {
                flashThread = new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        while (flashingBlocks.Count > 0)
                        {
                            for (int i = 0; i < flashingBlocks.Count; i++)
                            {
                                map.FlashingBlock fb = flashingBlocks[i];
                                //logger.debugLog("Flash thread got (" + fb.block.getX() + ", " + fb.block.getY() + ", " + fb.block.getZ() + ") from list");
                                //logger.debugLog("flashstate=" + fb.flashState0to16+" time="+fb.flashOnTime0to16);
                                int flashColor;
                                if (fb.flashToBlockType != -1 && fb.flashToBlockType != map.Block.getClientDisplayType(fb.block.getContent()))
                                {
                                    flashColor = fb.flashToBlockType;
                                }
                                else if (map.Block.getClientDisplayType(fb.block.getContent()) == (int)map.Block.Type.RED)
                                {
                                    flashColor = (int)map.Block.Type.GREEN;
                                }
                                else
                                {
                                    flashColor = (int)map.Block.Type.RED;
                                }

                                //logger.debugLog("Flash thread(" + fb.block.getX() + ", " + fb.block.getY() + ", " + fb.block.getZ() + ") sending setBlock packet...");
                                if (fb.flashState0to16 == fb.flashOnTime0to16)
                                {
                                    client.sendSetBlock(fb.block.getX(), fb.block.getY(), fb.block.getZ(), map.Block.getClientDisplayType(fb.block.getContent()));
                                }
                                else if (fb.flashState0to16 == 0)
                                {
                                    client.sendSetBlock(fb.block.getX(), fb.block.getY(), fb.block.getZ(), flashColor);
                                }
                                fb.flashState0to16 = (fb.flashState0to16 + 1) % 16;
                            }
                            //logger.debugLog("Flash thread starting 500 ms sleep...");
                            Thread.Sleep(100);
                            //logger.debugLog("Flash thread has awoken!");
                        }
                    }
                    catch 
                    {
                    }
                    flashThread = null;
                }));
                flashThread.Start();
            }
        }

    }
}
