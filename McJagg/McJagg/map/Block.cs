using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.map
{
    public class Block
    {
        // Block objects function as references to blocks in a Map.
        // Block objects can be safely compared with ==

        private Map map;
        private int x, y, z;
        private Location loc;
        public Block(Map map, int x, int y, int z)
        {
            this.map = map;
            this.x = x;
            this.y = y;
            this.z = z;
            this.loc = new Location(this);
        }
        public int getX() { return x; }
        public int getY() { return y; }
        public int getZ() { return z; }
        public Map getMap() { return map; }
        public void set(int newContent, bool sendClientUpdates)
        {
            map.setBlockContent(x, y, z, newContent, sendClientUpdates);
        }
        public int getContent()
        {
            return map.getBlockContent(x, y, z);
        }

        public double distanceTo(Block b2)
        {
            // pythagoras
            return Math.Sqrt(Math.Pow((double)b2.getX() - (double)x, 2.0) + Math.Pow((double)b2.getY() - (double)y, 2.0) + Math.Pow((double)b2.getZ() - (double)z, 2.0));
        }

        public Location getLocation()
        {
            return loc;
        }

        public enum Type
        {
            AIR = 0,
            STONE = 1,
            SMOOTHSTONE = 1,
            SMOOTH_STONE = 1,
            GRASS = 2,
            DIRT = 3,
            MUD = 3,
            COBBLE = 4,
            COBBLESTONE = 4,
            COBBLE_STONE = 4,
            PLANK = 5,
            PLANKS = 5,
            WOOD_PLANK = 5,
            WOOD_PLANKS = 5,
            SAPLING = 6,
            ADMINIUM = 7,
            BEDROCK = 7,
            WATER_FLOW = 8,
            WATER = 8,
            FLOWING_WATER = 8,
            WATER_STATIONARY = 9,
            STATIONARY_WATER = 9,
            ACTIVE_WATER = 9,
            WATER_ACTIVE = 9,
            LAVA_FLOW = 10,
            FLOWING_LAVA = 10,
            LAVA_ACTIVE = 10,
            ACTIVE_LAVA = 10,
            LAVA_STATIONARY = 11,
            STATIONARY_LAVA = 11,
            SAND = 12,
            BEACH = 12,
            GRAVEL = 13,
            FLINT = 13,
            GOLD_ORE = 14,
            ORE_GOLD = 14,
            IRON_ORE = 15,
            ORE_IRON = 15,
            COAL_ORE = 16,
            ORE_COAL = 16,
            TREE = 17,
            TRUNK = 17,
            WOOD = 17,
            STEM = 17,
            LEAVES = 18,
            LEAVE = 18,
            SPONGE = 19,
            SPONGES = 19,
            GLASS = 20,
            RED_WOOL = 21,
            WOOL_RED = 21,
            RED = 21,
            ORANGE_WOOL = 22,
            WOOL_ORANGE = 22,
            ORANGE = 22,
            YELLOW_WOOL = 23,
            WOOL_YELLOW = 23,
            YELLOW = 23,
            LIME_WOOL = 24,
            WOOL_LIME = 24,
            LIME = 24,
            GREEN_WOOL = 25,
            WOOL_GREEN = 25,
            GREEN = 25,
            AQUA_GREEN_WOOL = 26,
            WOOL_AQUA_GREEN = 26,
            AQUA_GREEN = 26,
            CYAN_WOOL = 27,
            WOOL_CYAN = 27,
            CYAN = 27,
            BLUE_WOOL = 28,
            WOOL_BLUE = 28,
            BLUE = 28,
            PURPLE_WOOL = 29,
            WOOL_PURPLE = 29,
            PURPLE = 29,
            INDIGO_WOOL = 30,
            WOOL_INDIGO = 30,
            INDIGO = 30,
            VIOLET_WOOL = 31,
            WOOL_VIOLET = 31,
            VIOLET = 31,
            MAGENTA_WOOL = 32,
            WOOL_MAGENTA = 32,
            MAGENTA = 32,
            PINK_WOOL = 33,
            WOOL_PINK = 33,
            PINK = 33,
            BLACK_WOOL = 34,
            WOOL_BLACK = 34,
            BLACK = 34,
            GRAY_WOOL = 35,
            WOOL_GRAY = 35,
            GRAY = 35,
            WHITE_WOOL = 36,
            WOOL_WHITE = 36,
            WHITE = 36,
            WOOL = 36,
            YELLOW_FLOWER = 37,
            FLOWER_YELLOW = 37,
            RED_FLOWER = 38,
            FLOWER_RED = 38,
            BROWN_MUSHROOM = 39,
            MUSHROOM_BROWN = 39,
            RED_MUSHROOM = 40,
            MUSHROOM_RED = 40,

            GOLD = 41,
            GOLD_BLOCK = 41,
            BLOCK_GOLD = 41,
            BLOCK_OF_GOLD = 41,
            IRON = 42,
            IRON_BLOCK = 42,
            BLOCK_IRON = 42,
            BLOCK_OF_IRON = 42,
            DOUBLE_SLAB = 43,
            SLABS = 43,
            SLAB = 44,
            BRICK = 45,
            BRICKS = 45,
            TNT = 46,
            BOOM = 46,
            EXPLOSIVE = 46,
            EXPLOSIVES = 46,
            BOOKSHELF = 47,
            BOOKSHELVES = 47,
            LIBRARY = 47,
            MOSSY_COBBLE = 48,
            MOSS_COBBLE = 48,
            MOSSY_COBBLESTONE = 48,
            MOSS_COBBLESTONE = 48,
            OBSIDIAN = 49,
            BLACK_BLOCK = 49,

            MAGIC_AIR = 300,
            X_WATER=400,
            ELEVATOR=500,
            DOOR=600,
            MAGMA=700,

            __UNKNOWN = 255,
            // TODO add all of them
        }

        public static int getClientDisplayType(int blockType)
        {
            // If we add special blocks like portal, door, etc...
            // add it into this function to make the client display
            // the block you want it to.

            // By default only return the least significant byte

            if (blockType == (int)Type.MAGIC_AIR) return 0;
            if (blockType == (int)Type.X_WATER) return 8;
            if (blockType == (int)Type.MAGMA) return 10;
            return blockType & 0xFF;
        }

        public static Type parse(int blockId)
        {
            return (Type)blockId;
        }
        public static Type parse(string blockName)
        {
            bool dummy;
            return parse(blockName, out dummy);
        }
        public static Type parse(string blockName, out bool success)
        {
            blockName = blockName.ToUpper().Replace(' ', '_');
            object o = null;
            try { o = Enum.Parse(typeof(Type), blockName); }
            catch { }
            if (o != null)
            {
                success = true;
                return (Type)o;
            }
            success = false;
            return Type.__UNKNOWN;
        }
    }
}
