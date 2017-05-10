using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace McJagg.map
{
    public class Map
    {
        /* .jgm file format:
         * 
         * TYPE         CONTENT
         * int         file format version
         * 
         * int         width  (size in x direction)
         * int         height (size in y direction)
         * int         depth  (size in z direction)
         * 
         * int[][][]    block contents in this order:
         * [x=0,y=0,z=0]
         * [x=0,y=0,z=1]
         * [x=0,y=0,z=2]
         * ...
         * [x=0,y=1,z=1]
         * [x=0,y=1,z=2]
         * ...
         * [x=1,y=0,z=0]
         * 
         * 
         * 
         * map configuration ideas:
         * 
         * int          
         * int          rank needed to visit
         * int          rank needed to place/edit blocks
         * int          rank needed to delete blocks
         * 
         */


        public static List<Map> loadedMaps = new List<Map>();

        private static List<Map> loadingMaps = new List<Map>();

        private bool loaded = false;

        public static bool exists(string name)
        {
            return File.Exists(getJgmPath(name));
        }

        public static Map getMap(string name)
        {
            if (name.Contains("/")) return null;
            for (int i=0; i<loadingMaps.Count; i++)
            {
                Map m = loadingMaps[i];
                if (m.name.ToLower() == name.ToLower())
                {
                    while (!m.loaded) { Thread.Sleep(2); }
                    return m;
                }
            }
            for (int i=0; i<loadedMaps.Count; i++)
            {
                if (loadedMaps[i].name.ToLower() == name.ToLower())
                {
                    return loadedMaps[i];
                }
            }
            try
            {
                if (!Directory.Exists("maps/" + name))
                {
                    // The map doesn't exist
                    return null;
                }
            }
            catch
            {
                // There's an error in the path name, so the map can't exist
                return null;
            }

            if (!File.Exists(getJgmPath(name)))
            {
                return null;
            }


            FileStream s = File.OpenRead(getJgmPath(name));

            Map map = new Map();
            map.name = name;
            loadingMaps.Add(map);

            int fileFormatVersion = readInt(s);
            map.width = readInt(s);
            map.height = readInt(s);
            map.depth = readInt(s);
            map.contents = new int[map.width, map.height, map.depth];
            map.blockObjects = new Block[map.width, map.height, map.depth];

            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    for (int z = 0; z < map.depth; z++)
                    {
                        map.contents[x, y, z] = readInt(s);
                    }
                }
            }

            if (fileFormatVersion == 2)
            {
                // In the next file format version more data can be read at the end
            }

            s.Close();

            map.config = Config.loadMapConfig(map);

            loadingMaps.Remove(map);
            loadedMaps.Add(map);
            return map;
        }

        public static Map create(string name, int width, int height, int depth)
        {
            Map map = new Map();
            map.name = name;
            loadingMaps.Add(map);
            
            map.contents = new int[width, height, depth];
            map.blockObjects = new Block[width, height, depth];
            map.width = width;
            map.height = height;
            map.depth = depth;

            // Generate a beautiful map 
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (y < ((int)(height / 2))) map.contents[x, y, z] = (int)Block.Type.DIRT;
                        else if (y == ((int)(height / 2)))
                        {
                            map.contents[x, y, z] = (int)Block.Type.GRASS;
                        }
                        else
                        {
                            if (y == ((int)(height / 2)) + 1 && ((x % 5) + (z % 7)) == 0)
                            {
                                map.contents[x, y, z] = (int)Block.Type.SAPLING;
                            }
                            else
                            {
                                map.contents[x, y, z] = (int)Block.Type.AIR;
                            }
                        }
                    }
                }
            }

            map.mapHasUnsavedChanges = true;
            map.settingsHasUnsavedChanges = true;
            
            map.saveMap(true);
            map.saveSettings();
            
            loadingMaps.Remove(map);
            loadedMaps.Add(map);
            if (map.config.physicsEnabled) map.initializePhysics();
            return map;
        }

        private int width, height, depth;
        private int[,,] contents = null;
        private Block[,,] blockObjects = null;
        private string name = null;
        private bool mapHasUnsavedChanges = false;
        private bool settingsHasUnsavedChanges = false;
        public void saveConfig()
        {
            Config.saveMapConfig(config, this);
        }
        public void reloadConfig()
        {
            config = Config.loadMapConfig(this);
        }
        public IEnumerable<Player> Players
        {
            get {
                return Player.getPlayersInMap(this);
            }
        }

        private Config.Map config = null;

        public Physics physics = null;

        public enum IncreaseSizeDirection
        {
            NEGATIVE,
            POSITIVE,
            CENTER,
        }

        public bool increaseSize(
            int newWidth,
            int newHeight,
            int newDepth,
            IncreaseSizeDirection widthDirection = IncreaseSizeDirection.CENTER,
            IncreaseSizeDirection heightDirection = IncreaseSizeDirection.CENTER,
            IncreaseSizeDirection depthDirection = IncreaseSizeDirection.CENTER)
        {
            if (newWidth < width) return false;
            if (newHeight < height) return false;
            if (newDepth < depth) return false;

            loadingMaps.Add(this);
            loadedMaps.Remove(this);
            saveMap();
            saveConfig();

            int[,,] newContents = new int[newWidth, newHeight, newDepth];
            int grassY;
            if (heightDirection == IncreaseSizeDirection.NEGATIVE) grassY = ((int)(height / 2)) + (newHeight - height);
            else if (heightDirection == IncreaseSizeDirection.CENTER) grassY = ((int)(height / 2)) + (newHeight - height)/2;
            else grassY = ((int)(height / 2));
            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    for (int z = 0; z < newDepth; z++)
                    {
                        if (y < grassY) newContents[x, y, z] = (int)Block.Type.DIRT;
                        else if (y == grassY)
                        {
                            newContents[x, y, z] = (int)Block.Type.GRASS;
                        }
                        else
                        {
                            if (y == grassY + 1 && ((x % 5) + (z % 7)) == 0)
                            {
                                newContents[x, y, z] = (int)Block.Type.SAPLING;
                            }
                            else
                            {
                                newContents[x, y, z] = (int)Block.Type.AIR;
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        int destX = x;
                        int destY = y;
                        int destZ = z;
                        if (widthDirection == IncreaseSizeDirection.NEGATIVE) destX += newWidth - width;
                        if (widthDirection == IncreaseSizeDirection.CENTER) destX += (newWidth - width) / 2;
                        if (heightDirection == IncreaseSizeDirection.NEGATIVE) destY += newHeight - height;
                        if (heightDirection == IncreaseSizeDirection.CENTER) destY += (newHeight - height) / 2;
                        if (depthDirection == IncreaseSizeDirection.NEGATIVE) destZ += newDepth - depth;
                        if (depthDirection == IncreaseSizeDirection.CENTER) destZ += (newDepth - depth) / 2;

                        newContents[destX, destY, destZ] = contents[x, y, z];
                    }
                }
            }
            int dx = 0;
            int dy = 0;
            int dz = 0;
            if (widthDirection == IncreaseSizeDirection.NEGATIVE) dx += newWidth - width;
            if (widthDirection == IncreaseSizeDirection.CENTER) dx += (newWidth - width) / 2;
            if (heightDirection == IncreaseSizeDirection.NEGATIVE) dy += newHeight - height;
            if (heightDirection == IncreaseSizeDirection.CENTER) dy += (newHeight - height) / 2;
            if (depthDirection == IncreaseSizeDirection.NEGATIVE) dz += newDepth - depth;
            if (depthDirection == IncreaseSizeDirection.CENTER) dz += (newDepth - depth) / 2;
            BlockLog.shiftAllByOffset(this, dx, dy, dz);
            width = newWidth;
            height = newHeight;
            depth = newDepth;
            contents = newContents;
            blockObjects = new Block[newWidth, newHeight, newDepth];
            saveMap();

            loadedMaps.Add(this);
            loadingMaps.Remove(this);
            foreach (Player p in Players)
            {
                p.client.joinWorld(this);
            }
            return true;
        }

        public string getName() { return name; }
        public int getWidth() { return width; }
        public int getHeight() { return height; }
        public int getDepth() { return depth; }

        public Block getSpawnLocationBlock() { return getBlock((int)config.spawnLocation.x, (int)config.spawnLocation.y, (int)config.spawnLocation.z); }
        public double getSpawnLocationX() { return config.spawnLocation.x; }
        public double getSpawnLocationY() { return config.spawnLocation.y; }
        public double getSpawnLocationZ() { return config.spawnLocation.z; }
        public double getSpawnLocationYaw() { return config.spawnLocation.yaw; }
        public double getSpawnLocationPitch() { return config.spawnLocation.pitch; }

        public int getPhysicsSpeed() { return config.physicsSpeed; }
        public bool isPhysicsEnabled() { return config.physicsEnabled; }

        public void setPhysicsSpeed(int newSpeed) { config.physicsSpeed = newSpeed; saveSettings(); }
        public void setPhysicsEnabled(bool newValue) { config.physicsEnabled = newValue; saveSettings(); }
        public void enablePhysics() { setPhysicsEnabled(true); }
        public void disablePhysics() { setPhysicsEnabled(false); }

        public Location getSpawnLocation() { return new Location(this, config.spawnLocation.x, config.spawnLocation.y, config.spawnLocation.z, config.spawnLocation.yaw, config.spawnLocation.pitch); }
        public void setSpawnLocation(LocationWithinMap loc)
        {
            setSpawnLocation(loc.getX(), loc.getY(), loc.getZ(), loc.getYaw(), loc.getPitch());
        }

        public int getVisitPermissionLevel() { return config.visitPermissionLevel; }
        public int getPlaceBlockPermissionLevel() { return config.placeBlockPermissionLevel; }
        public int getRemoveBlockPermissionLevel() { return config.removeBlockPermissionLevel; }
        public int getEditCommandPermissionLevel() { return config.editCommandPermissionLevel; }

        public bool getHacksAllowed() { return config.hacksAllowed; }

        public void setHacksAllowed(bool newValue) { config.hacksAllowed = newValue; saveSettings(); }

        public void setVisitPermissionLevel(int newValue)
        {
            config.visitPermissionLevel = newValue;
            saveSettings();
            // TODO remove current players that don't have permission from this map
        }
        public void setPlaceBlockPermissionLevel(int newValue) { config.placeBlockPermissionLevel = newValue; saveSettings(); }
        public void setRemoveBlockPermissionLevel(int newValue) { config.removeBlockPermissionLevel = newValue; saveSettings(); }
        public void setEditCommandPermissionLevel(int newValue) { config.editCommandPermissionLevel = newValue; saveSettings(); }

        /*public void setSpawnLocation(Block location)
        {
            setSpawnLocation(location.getX(), location.getY(), location.getZ(), 0, 0);
        }*/
        public void setSpawnLocation(double x, double y, double z, byte yaw, byte pitch)
        {
            config.spawnLocation.x = x;
            config.spawnLocation.y = y;
            config.spawnLocation.z = z;
            config.spawnLocation.yaw = yaw;
            config.spawnLocation.pitch = pitch;
            Config.saveMapConfig(config, this);
        }
        public void setBlockContent(Block x, Block.Type newContent, bool sendToClients) {
            setBlockContent(x.getX(), x.getY(), x.getZ(), (int)newContent, sendToClients);
        }
        public void setBlockContent(int x, int y, int z, Block.Type newContent, bool sendToClients)
        {
            setBlockContent(x, y, z, (int)newContent, sendToClients);
        }
        public void setBlockContent(int x, int y, int z, int newContent, bool sendToClients)
        {
            if (getHeight() < y || getWidth() < x || getDepth() < z) return;
            if (contents[x, y, z] != newContent)
            {
                contents[x, y, z] = newContent;
                mapHasUnsavedChanges = true;
            }
            for (int i = 0; i < server.server.serv.players.Count; i++)
            {
                if (server.server.serv.players[i].currentMap == this)
                {
                    server.server.serv.players[i].sendSetBlock(x, y, z, newContent);
                }
            }
            if (true) {
                //logger.log("set physics");
                if (physics != null) {
                    physics.checkBlock(this.getBlock(x, y, z));
                }
            }
        }

        public void unloadPhysics() {
            if (physics != null) {
                physics.Unload();
                physics = null;
            }
            
        }

        public int getBlockContent(int x, int y, int z)
        {
            if (getHeight() < y || getWidth() < x || getDepth() < z) return 0;
            return contents[x, y, z];
        }

        public void fillArea3D(Block corner1, Block corner2, int newContent)
        {
            if (corner1.getMap() != corner2.getMap())
            {
                throw new Exception("Those two blocks are not on the same map!");
            }
            else corner1.getMap().fillArea3D(corner1.getX(), corner1.getY(), corner1.getZ(), corner2.getX(), corner2.getY(), corner2.getZ(), newContent);
        }
        public void fillArea3D(int x1, int y1, int z1, int x2, int y2, int z2, int newContent)
        {
            if (x1 > x2)
            {
                int prevX1 = x1;
                x1 = x2;
                x2 = prevX1;
            }
            if (y1 > y2)
            {
                int prevY1 = y1;
                y1 = y2;
                y2 = prevY1;
            }
            if (z1 > z2)
            {
                int prevZ1 = z1;
                z1 = z2;
                z2 = prevZ1;
            }
            for (int x=x1; x <= x2; x++)
            {
                for (int y = y1; y <= y2; y++)
                {
                    for (int z = z1; z <= z2; z++)
                    {
                        contents[x, y, z] = newContent;
                    }
                }
            }
        }

        public Block getBlock(int x, int y, int z)
        {
            if (x < 0 || y < 0 || z < 0 || x >= width || y >= height || z >= depth) return null;
            if (blockObjects[x, y, z] == null)
            {
                return blockObjects[x, y, z] = new Block(this, x, y, z);
            }
            else
            {
                return blockObjects[x, y, z];
            }
        }
        public Block getBlockAt(LocationWithinMap loc)
        {
            return getBlock((int)loc.getX(), (int)loc.getY(), (int)loc.getZ());
        }

        public void unload()
        {
            physics.Unload();
            loadedMaps.Remove(this);
            loadingMaps.Remove(this);
            
        }

        static void writeInt(Stream stream, int i)
        {
            stream.Write(BitConverter.GetBytes(i), 0, 4);
        }
        static int readInt(Stream stream)
        {
            byte[] buff = new byte[4];
            server.client.fillBuffer(buff, stream);
            return BitConverter.ToInt32(buff, 0);
        }

        public void initializePhysics() {
            if (physics == null)
            {
                physics = new Physics(this);
                physics.Load();
            }
        }

        void saveMap(bool force=false)
        {
            if (mapHasUnsavedChanges || force)
            {
                mapHasUnsavedChanges = false;
                if (!Directory.Exists("maps")) Directory.CreateDirectory("maps");
                if (!Directory.Exists("maps/"+name)) Directory.CreateDirectory("maps/"+name);
                FileStream s;
                if (!File.Exists(getJgmPath()))
                {
                    s = File.Create(getJgmPath());
                }
                else
                {
                    s = File.OpenWrite(getJgmPath());
                }

                writeInt(s, 1);
                writeInt(s, width);
                writeInt(s, height);
                writeInt(s, depth);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int z = 0; z < depth; z++)
                        {
                            writeInt(s, contents[x, y, z]);
                        }
                    }
                }

                s.Flush();
                s.Close();
            }
        }
        void saveSettings()//bool force = true)
        {
            //if (settingsHasUnsavedChanges || force)
            {
                settingsHasUnsavedChanges = false;
                if (config == null) config = Config.loadMapConfig(this);
                Config.saveMapConfig(config, this);
            }
        }

        private string getJgmPath()
        {
            return getJgmPath(name);
        }
        private static string getJgmPath(string name)
        {
            return "maps/" + name + "/blocks.jgm";
        }


        public byte[] getRawBytesForClient()
        {
            // First 4 bytes = width * height * depth
            // then width*height*depth bytes containing the blocks ID's as byte[][][]
            byte[] buffer = new byte[4 + width * height * depth];
            bigEndianInt(width * height * depth).CopyTo(buffer, 0);
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        buffer[4 + i] = (byte)map.Block.getClientDisplayType(contents[x, y, z]);
                        i++;
                    }
                }
            }
            return buffer;
        }
        public static byte[] bigEndianUshort(ushort us)
        {
            byte[] bytes = BitConverter.GetBytes(us);
            Array.Reverse(bytes);
            return bytes;
        }
        public static byte[] bigEndianInt(int i)
        {
            byte[] bytes = BitConverter.GetBytes(i);
            Array.Reverse(bytes);
            return bytes;
        }

        private static Thread saveThread = null;
        public static void startSaveThread()
        {
            (saveThread=new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    Thread.Sleep(10000);
                    for (int i=0; i<loadedMaps.Count; i++)
                    {
                        Map m = loadedMaps[i];
                        if (m.mapHasUnsavedChanges)
                        {
                            logger.log("Saving changes on map " + m.getName());
                            m.saveMap();
                        }
                        Thread.Sleep(50);
                    }
                }
            }))).Start();
        }
    }
}
