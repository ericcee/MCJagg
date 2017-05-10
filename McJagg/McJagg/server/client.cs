using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.IO.Compression;
using McJagg.map;
using System.IO;

namespace McJagg.server
{
    public class client
    {
        public double lastX = -100, lastY = -100, lastZ = -100;
        public short lastRawX = -1, lastRawY = -1, lastRawZ = -1;
        public byte lastYaw = 0, lastPitch = 0;

        public Player playerObject = null;

        public string name = string.Empty;
        public string key = string.Empty;
        public byte protocolVersion = 0;
        public int id = 0;
        private bool disconnected = false;
        private bool isAlive = true;
        private long secMove = 0;
        private string motd = null;

        public delegate void MessageIncomeHandler(object sender, EventArgs e);
        public event MessageIncomeHandler MessageIncome;
        
        public Thread pingTh = null;
        public Thread ServerThread = null;
        server serv = null;
        public Map currentMap = null;

        public TcpClient tclient=null;
        private NetworkStream stream=null;

        private bool loadingMap = false;

        private Mutex writeMutex = new Mutex();

        public bool isLoadingMap {
            get {
                return loadingMap;
            }
        }

        public bool isBanned
        {
            get
            {
                return false; // TODO: Config
            }
        }

        public void updateUserdataStream() {
            protocolVersion = readByte();
            name = readString();
            key = readString();
            byte unused = readByte();
        }

        public client(TcpClient tcl,server _serv, int _id)
        {
            tclient = tcl;
            serv = _serv;
            stream = tclient.GetStream();
            id = _id;

            if (readByte() == 0)
            {
                updateUserdataStream();
            }
            else
            {
                logger.debugLog("ERROR ERROR ERROR ALARM FIRST BYTE != 0");
            }

        }

        public void startReadThread()
        {
            ServerThread = new Thread(new ThreadStart(delegate
             {
                 try
                 {
                     while (isAlive&&stream.CanRead)
                     {
                         if (stream.DataAvailable)
                         {
                             byte packetId = readByte();

                             if (packetId == 0x00)
                             {
                                 updateUserdataStream();
                             }
                             else if (packetId == 0x05)
                             {
                                 // Set block
                                 byte[] buff2 = new byte[2];
                                 fillBuffer(buff2, stream);
                                 short x = BitConverter.ToInt16(reverseEndian(buff2), 0);
                                 fillBuffer(buff2, stream);
                                 short y = BitConverter.ToInt16(reverseEndian(buff2), 0);
                                 fillBuffer(buff2, stream);
                                 short z = BitConverter.ToInt16(reverseEndian(buff2), 0);

                                 byte mode = readByte();
                                 byte blockType = readByte();

                                 // Not needed, these are integer block coordinates... silly me
                                 //double xx = ((double)x) / 32.0;
                                 //double yy = ((double)y) / 32.0;
                                 //double zz = ((double)z) / 32.0;

                                 receivedSetBlock(x, y, z, mode, blockType);
                             }
                             else if (packetId == 0x08)
                             {
                                 // Position update

                                 byte playerId = readByte();
                                 // This should always be 0xFF

                                 if (playerId != 0xFF)
                                 {
                                     throw new Exception("postiion packet player id != 0xFF");
                                 }

                                 byte[] buff2 = new byte[2];
                                 fillBuffer(buff2, stream);
                                 short x = BitConverter.ToInt16(reverseEndian(buff2), 0);
                                 fillBuffer(buff2, stream);
                                 short y = BitConverter.ToInt16(reverseEndian(buff2), 0);
                                 fillBuffer(buff2, stream);
                                 short z = BitConverter.ToInt16(reverseEndian(buff2), 0);

                                 byte yaw = readByte();
                                 byte pitch = readByte();

                                 // Not needed, these are integer block coordinates... silly me
                                 double xx = ((double)x) / 32.0;
                                 double yy = ((double)y) / 32.0;
                                 double zz = ((double)z) / 32.0;

                                 if (receivedPosition(xx, yy, zz, yaw, pitch, x, y, z))
                                 {
                                     lastRawX = x;
                                     lastRawY = y;
                                     lastRawZ = z;
                                 }
                             }
                             else if (packetId == 0x0d)
                             {
                                 // Chat message
                                 byte unused = readByte();
                                 string msg = readString();

                                 receivedChatMessage(msg);
                             }
                             else
                             {
                                 Console.WriteLine("ERROR: Unknown packet id: " + packetId);
                                 throw new Exception("Unknown packet id " + packetId);
                             }
                         }
                         else {
                             try
                             {
                                 if (playerObject.lastMovement + Config.server.afkSeconds * 1000 < playerObject.loginTime && !playerObject.AFK)
                                 {
                                     playerObject.AFK = true;
                                 }
                             }
                             catch { }
                         }
                     }
                 }
                 catch (IOException e)
                 {
                     logger.log("Player disconnected!");
                     serv.players.Remove(this);
                     Player.players.Remove(playerObject);
                 }
                 catch (Exception e)
                 {
                     Console.WriteLine(e.Message);
                     Console.WriteLine(e.StackTrace);
                     disconnect("Something went wrong " + e);
                 }
                 logger.debugLog("Can no longer read stream of player");
                 onDisconnected();
             }));
            ServerThread.Start();
        }
        public void setMotd(string Motd)
        {
            this.motd = Motd;
        }
        public void serve()
        {
            pingTh = new Thread(new ThreadStart(delegate{
                while (true) {
                    try{
                        if (!isLoadingMap)
                        {
                            writeMutex.WaitOne();
                            {
                                writeByte(0x01);
                                //logger.debugLog("Ping");
                            }
                            writeMutex.ReleaseMutex();
                        }
                    }
                    catch {
                        logger.log("Player "+ name + " left the server.");
                        //onDisconnected();
                    }
                    Thread.Sleep(1000);
                }
            }));
            pingTh.Start();
        }

        private byte[] reverseEndian(byte[] endian) {
            Array.Reverse(endian);
            return endian;
        }

        public void Teleport(short x, short y, short z, byte yaw, byte pitch)
        {
            writeMutex.WaitOne();
            {
                try
                {
                    writeByte(0x08); // Package teleport
                    writeByte(0xFF); //ID

                    writeShort(x);
                    writeShort(y);
                    writeShort(z);
                    writeByte(yaw);
                    writeByte(pitch);
                }
                catch { }
            }
            writeMutex.ReleaseMutex();
        }
        public void sendRawPositionChange(sbyte dx, sbyte dy, sbyte dz)
        {
            writeMutex.WaitOne();
            {
                try
                {
                    writeByte(0x0a); // Package id
                    writeByte(0xFF); //ID

                    writeShort(dx);
                    writeShort(dy);
                    writeShort(dz);
                }
                catch { }
            }
            writeMutex.ReleaseMutex();
        }

        private void writeShort(short x) {
            try
            {
                stream.Write(reverseEndian(BitConverter.GetBytes(x)), 0, 2);
            }
            catch { }
        }

        public void sendSpawn(byte _id, string player, double x, double y, double z, byte yaw, byte pitch)
        {
            sendSpawnRaw(_id, player, (short)(x * 32), (short)(y * 32), (short)(z * 32), yaw, pitch);
        }
        public void sendSpawnRaw(byte _id, string player, short x, short y, short z, byte yaw, byte pitch)
        {
            writeMutex.WaitOne();
            {
                writeByte(0x07); // Spawn Player

                writeByte(_id);
                writeString(player);
                writeShort(x);
                writeShort(y);
                writeShort(z);
                writeByte(yaw);
                writeByte(pitch);
            }
            writeMutex.ReleaseMutex();
        }

        //public void Teleport

        public void sendUserTypeUpdate()
        {
            writeMutex.WaitOne();
            {
                writeByte(0x0f);
                if (!playerObject.getRank().canDestroyBedrock)
                {
                    writeByte(0x00);
                }
                else
                {
                    writeByte(0x64);
                }
            }
            writeMutex.ReleaseMutex();
        }

        public void sendServerDetails()
        {
            writeMutex.WaitOne();
            {
                writeByte(0x00); // Handshake package
                writeByte(0xFF); // user id

                writeString(Config.server.serverName ?? "An awesome new MCJagg server!");
                writeString(this.motd ?? "Welcome to this new MCJagg server!");

                OfflinePlayer op = OfflinePlayer.getOfflinePlayer(name);

                if (op == null || !op.getRank().canDestroyBedrock)
                {
                    writeByte(0x00);
                }
                else
                {
                    writeByte(0x64);
                }
            }
            writeMutex.ReleaseMutex();
        }

        private void writeByte(byte x)
        {
            try
            {
                stream.Write((new byte[] { x }), 0, 1);
            }
            catch {
                onDisconnected();
            }
        }

        public void sendMessage(string message, byte fromid=0xFF)
        {
            writeMutex.WaitOne();
            {
                string[] lines = message.Split(new char[] { '\n' });
                foreach (string line in lines)
                {
                    if (line.Length > 64)
                    {
                        string l = line;
                        while (l.Length > 64)
                        {
                            string part = l.Substring(0, 64);
                            l = l.Substring(64);
                            writeByte(0x0D); //Send massage
                            writeByte(fromid);
                            writeString(part);
                        }
                        if (l.Length > 0)
                        {
                            writeByte(0x0D); //Send massage
                            writeByte(fromid);
                            writeString(l);
                        }
                    }
                    else
                    {
                        writeByte(0x0D); //Send massage
                        writeByte(fromid);
                        writeString(line);
                    }
                }
            }
            writeMutex.ReleaseMutex();
        }

        public void sendDespawn(byte id)
        {
            writeMutex.WaitOne();
            {
                writeByte(0x0c);
                writeByte(id);
            }
            writeMutex.ReleaseMutex();
        }

        public void joinWorld(string name)
        {
            joinWorld(Map.getMap(name));
        }

        public void joinWorld(Map map, LocationWithinMap loc=null)
        {
            setMotd(Config.loadMapConfig(map).motd);
            sendServerDetails();
            if (playerObject!=null) MainWindow.updatePlayerMap(playerObject, map);
            loc = loc ?? map.getSpawnLocation();
            loadingMap = true;
            if (currentMap != null)
            {
                IEnumerable<Player> ps = Player.getPlayersInMap(currentMap);
                foreach (Player p in ps)
                {
                    if (p != this.playerObject)
                    {
                        p.client.sendDespawn((byte)this.id);
                        this.sendDespawn((byte)p.client.id);
                    }
                }
            }

            writeMutex.WaitOne();
            {
                currentMap = map;
                byte[] chunk = new byte[1024];
                byte[] rawbytes = currentMap.getRawBytesForClient();
                byte[] zipped = Compress(rawbytes.Length, rawbytes); // gzip the level

                logger.debugLog("Intialize download");
                writeByte(0x02); // intialize download map

                int parts = zipped.Length / 1024, end = zipped.Length % 1024;

                for (int i = 0; i < parts; i++)
                {
                    byte[] useless = new byte[1024];
                    Array.Copy(zipped, i * 1024, useless, 0, 1024);
                    sendLvlDataChunk(useless, 1024, (byte)(((float)i / (float)parts) * 100));
                    logger.debugLog("Sending chunk " + i + " percent complete:" + (byte)(((float)i / (float)parts) * 100) + "%");
                    Thread.Sleep(100);
                }

                byte[] useless0 = new byte[end];
                Array.Copy(zipped, parts * 1024, useless0, 0, end);
                sendLvlDataChunk(useless0, (short)end, 100);

                writeByte(0x04); // end download map

                writeShort((short)currentMap.getWidth()); // Size X   = width
                writeShort((short)currentMap.getHeight()); // Size Y  = height
                writeShort((short)currentMap.getDepth()); // Size Z  = depth
                logger.debugLog("Download complete");
                loadingMap = false;
            }
            writeMutex.ReleaseMutex();

            sendSpawn(0xFF, name, loc.getX(), loc.getY(), loc.getZ(), loc.getYaw(), loc.getPitch());

            if (currentMap != null)
            {
                IEnumerable<Player> ps = Player.getPlayersInMap(currentMap);
                foreach (Player p in ps)
                {
                    if (p != this.playerObject)
                    {
                        p.client.sendSpawn((byte)id, name, currentMap.getSpawnLocationX(), currentMap.getSpawnLocationY(), currentMap.getSpawnLocationZ(), 0, 0);
                        sendSpawn((byte)p.client.id, p.client.name, p.client.lastX, p.client.lastY, p.client.lastZ, 0, 0);
                    }
                }
            }
        }

        public static byte[] Compress(int blocklen,byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                CompressionMode.Compress, true))
                {
                    byte[] blocklenz = BitConverter.GetBytes(blocklen);
                    //gzip.Write(blocklenz, 0, 4);
                    gzip.Write(raw, 0, raw.Length);
                    gzip.Close();
                }
                System.GC.Collect();
                return memory.ToArray();
            }
        }

        private void sendLvlDataChunk(byte[] chunk, short size, byte prcent) {
            byte[] fullchunk = new byte[1024];
            Array.Copy(chunk, fullchunk, size);
            writeByte(0x03); // chunk data package
            writeShort(size);
            stream.Write(fullchunk, 0, fullchunk.Length);
            writeByte(prcent);
        }

        public void disconnect(string reason)
        {
            foreach (Player p in Player.getPlayersInMap(currentMap))
            {
                try
                {
                    if (p != playerObject) p.client.sendDespawn((byte)id);
                }
                catch { }
            }
            writeMutex.WaitOne();
            {
                writeByte(0x0e); // disconnect package
                writeString(reason);
                logger.log("Player " + name + " Disconnected reason: " + reason);
                stream.Close();
                tclient.Close();
                onDisconnected();
            }
            writeMutex.ReleaseMutex();
        }

        private string removeChar(string x, char z) {
            string[] all = x.Split(z);
            string ret = string.Empty;
            foreach (string f in all) {
                ret += f;
            }
            return ret;
        }

        private string readString() {
            byte[] bstr = new byte[64];
            string endres = string.Empty;
            fillBuffer(bstr, stream);

            string [] parts=ASCIIEncoding.ASCII.GetString(bstr).Split(' ');

            foreach (string trim in parts) {
                if (trim != string.Empty) endres += trim+" ";
            }

            return endres.Remove(endres.Length-1);
        }

        private void writeString(string text)
        {
            if (text.Length > 64) text = text.Substring(0, 64);
            byte[] sendmsg = new byte[64];
            for (int i = 0; i < sendmsg.Length; i++)
            {
                sendmsg[i] = 0x20;
            }

            for (int i = 0; i < text.Length; i++)
            {
                sendmsg[i] = (byte) text.ToCharArray()[i];
            }
            try
            {
                stream.Write(sendmsg, 0, sendmsg.Length);
            }
            catch { }
        }

        public bool verify()
        {
            try
            {
                if (String.Equals(removeChar(BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(serv.salt + name))),'-'), key, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
                return false;
            }
            catch { }
            return false;
        }

        public void receivedSetBlock(int x, int y, int z, int mode, int blockType)
        {
            if (loadingMap) return;
            bool canceled = false;

            //logger.debugLog("Received setBlock 0x05:");
            //logger.debugLog("x=" + x + " y=" + y + " z=" + z + " mode=" + mode + " blockType=" + blockType);

            if (mode == 1 &&
                playerObject.getPermissionLevel() < currentMap.getPlaceBlockPermissionLevel())
            {
                sendMessage("&4You do not have permission to build here!");
                canceled = true;
            }
            else if (mode == 0 &&
                playerObject.getPermissionLevel() < currentMap.getRemoveBlockPermissionLevel())
            {
                sendMessage("&4You do not have permission to remove blocks here!");
                canceled = true;
            }

            // If a command is waiting for block input...
            else if (playerObject.commandInputer != null &&
                playerObject.commandInputer.getInputType() == command.Command.InputType.BLOCK_SELECTION)
            {
                playerObject.commandInputer.setBlockInput(currentMap.getBlock(x, y, z));
                // TODO also give commands the mode & blockType ?
                canceled = true;
                sendMessage("&aBlock selected: (" + x + ", " + y + ", " + z + ")");
            }
            int originalBlockType = playerObject.getMap().getBlock(x,y,z).getContent();
            if (!playerObject.getRank().canDestroyBedrock && ((blockType == (int)Block.Type.BEDROCK) || originalBlockType==(int)Block.Type.BEDROCK)) {
                canceled = true;
                sendMessage("&4you cant place or destroy this block type");
            }

            if (!canceled)
            {
                int newBlockType = (mode == 0x00) ? (int)Block.Type.AIR : blockType;
                currentMap.setBlockContent(x, y, z, newBlockType, true);
                BlockLog.blockChanged(currentMap, playerObject, x, y, z, originalBlockType, newBlockType);
            }
            else
            {
                sendSetBlock(x, y, z, currentMap.getBlockContent(x, y, z));
            }
        }

        public void sendSetBlock(int x, int y, int z, int blockContent)
        {
            writeMutex.WaitOne();
            {
                writeByte(0x06);
                writeShort((short)x);
                writeShort((short)y);
                writeShort((short)z);
                writeByte((byte)Block.getClientDisplayType(blockContent));
            }
            writeMutex.ReleaseMutex();
        }

        public void sendPlayerMovedTeleport(sbyte playerId, double x, double y, double z, byte yaw, byte pitch)
        {
            writeMutex.WaitOne();
            {
                try
                {
                    writeByte(0x08);
                    writeByte((byte)playerId);
                    writeShort((short)(x * 32));
                    writeShort((short)(y * 32));
                    writeShort((short)(z * 32));
                    writeByte(yaw);
                    writeByte(pitch);
                }
                catch { }
            }
            writeMutex.ReleaseMutex();
        }

        public void sendPlayerMovedInterpolate(sbyte playerId, sbyte dx, sbyte dy, sbyte dz, byte yaw, byte pitch)
        {
            writeMutex.WaitOne();
            {
                try
                {
                    writeByte(0x09);
                    writeByte((byte)playerId);
                    writeByte((byte)dx);
                    writeByte((byte)dy);
                    writeByte((byte)dz);
                    writeByte(yaw);
                    writeByte(pitch);
                }
                catch { }
            }
            writeMutex.ReleaseMutex();
        }

        public bool receivedPosition(double x, double y, double z, byte yaw, byte pitch, short rawX, short rawY, short rawZ)
        {
            if (loadingMap) return false;
            bool interpolate = false;
            sbyte dx = 0, dy = 0, dz = 0;
            // TODO check if player moved maps. If they moved maps, don't interpolate!
            if (Math.Abs(lastX-x) < 4 && Math.Abs(lastZ-z) < 4 && Math.Abs(lastY-y) < 4)
            {
                interpolate = true;
                dx = (sbyte)((int)((x - lastX) * 32));
                dy = (sbyte)((int)((y - lastY) * 32));
                dz = (sbyte)((int)((z - lastZ) * 32));
            }
            lastYaw = yaw;
            lastPitch = pitch;
            if (playerObject.offlinePlayer.frozen)
            {
                //logger.debugLog("Player is frozen!");
                if (Math.Abs(rawX - lastRawX) + Math.Abs(rawY - lastRawY) + Math.Abs(rawZ - lastRawZ) > 30)
                {
                    logger.debugLog("Player is frozen and tried to move!");
                    //sendRawPositionChange((sbyte)(lastRawX - rawX), (sbyte)(lastRawY - rawY), (sbyte)(lastRawZ - rawZ));
                    Teleport(lastRawX, lastRawY, lastRawZ, lastYaw, lastPitch);
                }
                return false;
            }

            if (lastX != x || lastY != y || lastZ != z) {
                if (currentMap.physics != null) currentMap.physics.checkPos(playerObject);
                playerObject.lastMovement = playerObject.loginTime;
                if (playerObject.AFK) playerObject.AFK = false;
                if (secMove + 1000 < playerObject.loginTime)
                {
                    secMove = playerObject.loginTime;
                    playerObject.fullSpeed = playerObject.currSecSpeed;
                    playerObject.lastSecondPos = new Vector(playerObject);
                }
                
            }

            lastX = x;
            lastY = y;
            lastZ = z;


            for (int i = 0; i < serv.players.Count; i++)
            {
                if (serv.players[i] == this) continue;
                if (serv.players[i].currentMap == currentMap)
                {
                    if (interpolate) serv.players[i].sendPlayerMovedInterpolate((sbyte)id, dx, dy, dz, yaw, pitch);
                    else serv.players[i].sendPlayerMovedTeleport((sbyte)id, x, y, z, yaw, pitch);
                }
            }


            MainWindow.updatePlayerPosition(playerObject);
            

            return true;
        }

        public void receivedChatMessage(string message)
        {
            if (message.StartsWith("@"))
            {
                // TODO pm
            }
            else if (message.StartsWith("/"))
            {
                logger.log(name + ": " + message);
                new Thread(new ThreadStart(delegate
                {
                    command.Command.execute(message, playerObject);
                })).Start();
            }
            else
            {
                logger.log(name+": " + message);
                serv.broadcastMessage("&f["+playerObject.getRank().getColoredName()+"&f] &a"+name + "&b:&f " + message);
            }
        }

        static bool logBytesReceived = false;
        public static void fillBuffer(byte[] buff, Stream stream)
        {
            int ptr = 0;
            while (ptr < buff.Length)
            {
                int amount = stream.Read(buff, ptr, buff.Length - ptr);
                for (int i = ptr; i < amount + ptr; i++)
                {
                    if (logBytesReceived) Console.WriteLine("b="+buff[i]);
                }
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
        byte[] buff1 = new byte[1];
        private byte readByte()
        {
            fillBuffer(buff1, stream);
            return buff1[0];
        }

        private void onDisconnected()
        {
            MainWindow.removeOnlinePlayer(playerObject);
            if (disconnected != true)
            {
                disconnected = true;
                isAlive = false;
                serv.players.Remove(this);
                Player.players.Remove(playerObject);
                if (currentMap != null)
                {
                    IEnumerable<Player> ps = Player.getPlayersInMap(currentMap);
                    try
                    {
                        foreach (Player p in ps)
                        {
                            if (p != this.playerObject)
                            {
                                p.client.sendDespawn((byte)this.id);
                                try { this.sendDespawn((byte)p.client.id); }
                                catch { }
                            }
                        }
                    }
                    catch { logger.log("hankey"); }
                }
                serv.broadcastMessage("# Player " + name + " disconnected :-(");
                try { pingTh.Abort(); }
                catch { }
                
            }
        }
    }
}
