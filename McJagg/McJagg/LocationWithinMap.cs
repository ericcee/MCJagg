using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg
{
    public class LocationWithinMap
    {
        private double x, y, z;
        private byte yaw, pitch;
        public LocationWithinMap(LocationWithinMap loc)
            : this(loc.x, loc.y, loc.z, loc.yaw, loc.pitch)
        {
        }
        public LocationWithinMap(double x, double y, double z, byte yaw, byte pitch)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.yaw = yaw;
            this.pitch = pitch;
        }
        public Location getLocationOnMap(map.Map map)
        {
            return new Location(map, this);
        }
        public double getX() { return x; }
        public double getY() { return y; }
        public double getZ() { return z; }
        public byte getYaw() { return yaw; }
        public byte getPitch() { return pitch; }
    }
}
