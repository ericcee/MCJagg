using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg
{
    public class Vector
    {
        public readonly double x, y, z;
        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector(map.Block block)
        {
            this.x = block.getX();
            this.y = block.getY();
            this.z = block.getZ();
        }
        public Vector(Player pl)
        {
            this.x = pl.getX();
            this.y = pl.getY();
            this.z = pl.getZ();
        }
        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }
        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        public static Vector operator *(Vector v, double d)
        {
            return new Vector(v.x * d, v.y * d, v.z * d);
        }
        public static Vector operator /(Vector v, double d)
        {
            return new Vector(v.x / d, v.y / d, v.z / d);
        }
        public double length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }
        public Vector normal()
        {
            double len = length();
            return new Vector(x / len, y / len, z / len);
        }
        public Vector rotate()
        {
            return new Vector(y, z, x);
        }
        public Vector2 ZY()
        {
            return new Vector2(z, y);
        }
    }
    public class Vector2
    {
        public readonly double x, y;
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }
        public static Vector2 operator *(Vector2 v, double d)
        {
            return new Vector2(v.x * d, v.y * d);
        }
        public static Vector2 operator /(Vector2 v, double d)
        {
            return new Vector2(v.x / d, v.y / d);
        }
    }
}
