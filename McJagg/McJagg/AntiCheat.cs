using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using McJagg.map;
using System.Threading;

namespace McJagg
{
    public class AntiCheat
    {
        private Thread AntiCheatThread;
        private bool isLoaded = false;
        private Map thisMap = null;

        public AntiCheat(Map thMap)
        {
            thisMap = thMap;
        }

        private int getVecXZDist(Vector vec1, Vector vec2) {
            Vector thvec =vec1 - vec2;
            return (int)(Math.Sqrt(Math.Pow(thvec.x, 2) + Math.Pow(thvec.z, 2)));
        }

        private void checkPlayerMovement(Player thisplayer) {

            if (thisplayer.probablyFalling)
            {
                if (thisplayer.getPlayerGroundDistance() == 0)
                {
                    thisplayer.probablyFalling = false;
                }
                else
                {
                    if (thisplayer.lastFallingPos.y - 1 < thisplayer.getY() && thisplayer.getPlayerGroundDistance() > 1)
                    {
                        if (getVecXZDist(thisplayer.lastFallingPos, new Vector(thisplayer)) > 2|| getVecXZDist(thisplayer.lastFallingPos, new Vector(thisplayer))==0)
                        {
                            if(!thisplayer.isSwiming) thisplayer.client.sendMessage("ur flying arent u?");
                        }
                    }
                    else
                    {
                        thisplayer.lastFallingPos = new Vector(thisplayer);
                    }
                }
            }
            else
            {
                if (thisplayer.getPlayerGroundDistance() != 0 && !thisplayer.probablyFalling)
                {
                    thisplayer.lastFallingPos = new Vector(thisplayer);
                    thisplayer.probablyFalling = true;
                }
            }

            if (thisplayer.fullSpeed > 8) {
                thisplayer.client.sendMessage("Please turn of speedhacking");
            }
        }

        private void AntiCheating() {
            while (isLoaded) {
                try
                {
                    foreach (Player x in thisMap.Players)
                    {
                        checkPlayerMovement(x);
                    }
                }
                catch { }
                Thread.Sleep(250);
            }
        }

        public void initialize() {
            isLoaded = true;
            AntiCheatThread = new Thread(new ThreadStart(() => AntiCheating()));
            AntiCheatThread.Start();
        }
    }
}
