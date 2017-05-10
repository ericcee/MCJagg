using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using McJagg.map;

namespace McJagg
{
    public class Physics
    {
        private Queue<Block> tasks = null;
        private Map thisMap=null;
        private Thread PhysicsThread = null;

        private bool physicsLoaded = false;
        private int PhysSpeed = 250;

        public Physics(Map _map) 
        {
            tasks = new Queue<Block>();
            thisMap = _map;
        }

        public Block[] getSuroundings(Block surr) {
            Block [] t = new Block[6];
            try
            {
                t[0] = thisMap.getBlock(surr.getX(), surr.getY(), surr.getZ() + 1);
                t[1] = thisMap.getBlock(surr.getX(), surr.getY(), surr.getZ() - 1);
                t[2] = thisMap.getBlock(surr.getX()+1, surr.getY(), surr.getZ());
                t[3] = thisMap.getBlock(surr.getX()-1, surr.getY(), surr.getZ());
                t[4] = thisMap.getBlock(surr.getX(), surr.getY() - 1, surr.getZ());
                t[5] = thisMap.getBlock(surr.getX(), surr.getY() + 1, surr.getZ());
            }
            catch { }
            return t;
        }

        public bool checkSuroundingsIfSponge(Block z)
        {
            if (z == null) return false;
            foreach (Block x in getSuroundings(z))
            {
                if (x == null) continue;
                if (x.getContent() == (int)Block.Type.SPONGE) return true;
            }
            return false;
        }

        public void updateBlock(Block type)
        {
            try // physics fuse
            {
                if (type.getContent() == (int)(Block.Type.SPONGE))
                {
                    foreach (Block th in getSuroundings(type)) {
                        if (th == null) continue;
                        if(th.getContent()==(int)Block.Type.X_WATER|| th.getContent() == (int)Block.Type.ACTIVE_WATER)
                                thisMap.setBlockContent(th.getX(), th.getY(), th.getZ(), Block.Type.AIR, true);
                    }
                }

                if (type.getContent() == (int)(Block.Type.X_WATER))
                {
                    if(thisMap.getBlock(type.getX(), type.getY() - 1, type.getZ()).getContent() == (int)Block.Type.X_WATER) return;

                    if (thisMap.getBlock(type.getX(), type.getY() - 1, type.getZ()).getContent() == (int)Block.Type.AIR)
                    {
                        thisMap.setBlockContent(type.getX(), type.getY() - 1, type.getZ(), Block.Type.X_WATER, true);
                    }
                    else
                    {
                        if (thisMap.getBlock(type.getX()-1, type.getY(), type.getZ()).getContent() == (int)Block.Type.AIR)
                        {
                            thisMap.setBlockContent(type.getX()-1, type.getY(), type.getZ(), Block.Type.X_WATER, true);
                        }
                        if (thisMap.getBlock(type.getX()+1, type.getY(), type.getZ()).getContent() == (int)Block.Type.AIR)
                        {
                            thisMap.setBlockContent(type.getX()+1, type.getY(), type.getZ(), Block.Type.X_WATER, true);
                        }
                        if (thisMap.getBlock(type.getX(), type.getY(), type.getZ()-1).getContent() == (int)Block.Type.AIR)
                        {
                            thisMap.setBlockContent(type.getX(), type.getY(), type.getZ()-1, Block.Type.X_WATER, true);
                        }
                        if (thisMap.getBlock(type.getX(), type.getY(), type.getZ()+1).getContent() == (int)Block.Type.AIR)
                        {
                            thisMap.setBlockContent(type.getX(), type.getY() , type.getZ() + 1, Block.Type.X_WATER, true);
                        }
                    }

                }

                if (type.getContent() == (int)(Block.Type.ACTIVE_LAVA))
                {
                    try
                    {
                        Block[] surr = getSuroundings(type);
                        for (int i = 0; i < 5; i++)
                        {
                            if (surr[i].getContent() == (int)(Block.Type.AIR))
                            {
                                if (surr[i] == null) continue;
                                thisMap.setBlockContent(surr[i], Block.Type.ACTIVE_LAVA, true);
                            }
                        }
                    }
                    catch { }
                }

                if (type.getContent() == (int)(Block.Type.ACTIVE_WATER))
                {
                    try
                    {
                        Block[] surr = getSuroundings(type);
                        for (int i =0; i < 5; i++) { 
                            if (surr[i].getContent() == (int)(Block.Type.AIR))
                            {
                                if (surr[i] == null) continue;
                                if (!checkSuroundingsIfSponge(surr[i]))
                                    thisMap.setBlockContent(surr[i]  , Block.Type.ACTIVE_WATER, true);
                            }
                        }
                    }
                    catch { }
                }

                if (type.getContent() == (int)(Block.Type.SAND))
                {
                    try
                    {
                        if (thisMap.getBlock(type.getX(), type.getY() - 1, type.getZ()).getContent() == (int)(Block.Type.AIR))
                        {
                            thisMap.setBlockContent(type.getX(), type.getY(), type.getZ(), Block.Type.AIR, true);
                            thisMap.setBlockContent(type.getX(), type.getY() - 1, type.getZ(), Block.Type.SAND, true);
                        }
                    }
                    catch { }
                }

                if (type.getContent() == (int)Block.Type.DIRT)
                {
                    if (getSuroundings(type)[5].getContent() == (int)Block.Type.AIR)
                        thisMap.setBlockContent(type, Block.Type.GRASS, true);

                    if (getSuroundings(type)[4].getContent() == (int)Block.Type.GRASS)
                        thisMap.setBlockContent(getSuroundings(type)[4], Block.Type.DIRT, true);
                }

                if (type.getContent() == (int)Block.Type.GRASS)
                {
                    if (getSuroundings(type)[5].getContent() != (int)Block.Type.AIR)
                        thisMap.setBlockContent(type, Block.Type.DIRT, true);

                    if (getSuroundings(type)[4].getContent() == (int)Block.Type.GRASS)
                        thisMap.setBlockContent(getSuroundings(type)[4], Block.Type.DIRT, true);
                }


            }
            catch { }
        }

        public void checkPos(Player p) {

        }

        public void checkBlock (Block type)
        {
            try
            {
                foreach (Block x in getSuroundings(type))
                {
                    if (x == null) continue;
                    if (x.getContent() != (int)Block.Type.AIR) tasks.Enqueue(x);
                }
                tasks.Enqueue(type);
            }
            catch (Exception e) { logger.log(e.Message+" "+ tasks.Count) ; }
        }

        private void physicsThread() 
        {
            logger.debugLog("Physics started");
            while (physicsLoaded)
            {
                if (tasks.Count == 0) {
                    Thread.Sleep(1000);
                    continue;
                }

                int cx = tasks.Count;
                Console.Title=(cx + " Physics Block changes/s");
                while(cx!=0)
                {
                    updateBlock(tasks.Dequeue());
                    cx--;
                }
                Thread.Sleep(PhysSpeed);
            }
            logger.debugLog("Physics destroyed");
        }

        public void Load() 
        {
            logger.debugLog("Physics loaded in "+thisMap.getName());
            PhysicsThread = new Thread(new ThreadStart(() => physicsThread()));
            PhysicsThread.Start();
            physicsLoaded = true;
        }

        public void Unload()
        {
            physicsLoaded = false;
        }

        public void setUpdateSpeed(int speed)
        {
            PhysSpeed = speed;
        }
    }
}
