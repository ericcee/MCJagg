using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg
{
    public class Location : LocationWithinMap
    {
        private map.Map map;
        //private map.Block block;

        public Location(map.Map map, double x, double y, double z, byte yaw, byte pitch)
            : base(x, y, z, yaw, pitch)
        {
            this.map = map;
            //this.block = map.getBlockAt(this);
        }
        public Location(map.Map map, LocationWithinMap loc)
            : base(loc)
        {
            this.map = map;
            //this.block = map.getBlockAt(loc);
        }
        public LocationWithinMap getLocationWithinMap()
        {
            return this;
        }
        public Location(map.Block block)
            : base(block.getX(), block.getY(), block.getZ(), 0, 0)
        {
            this.map = block.getMap();
            //this.block = block;
        }
        public map.Block getBlock() { return map.getBlockAt(this); }
        public map.Map getMap() { return map; }
    }
}
