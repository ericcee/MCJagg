using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace McJagg.command
{
    class CmdCuboid : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }

        public override string getName()
        {
            return "cuboid";
        }
        public override string[] getAliases() { return new string[] { "z", "replace", "sphere", "spheroid", "circle", "oval" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{ ParamType.STRING_WITH_SPACES, },
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 200;
        }
        private static string parseFillType(string str, out bool success)
        {
            str = str.ToLower();
            if (str == "fill" || str == "filled") str = "solid";
            if (str == "wire" || str == "wireframe") str = "grid";
            if (str == "solid" || str == "hollow" || str=="grid" || str=="walls")
            {
                success = true;
                return str;
            }
            else
            {
                success = false;
                return null;
            }
        }

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            string[] args = arguments[0].stringValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (alias.ToLower().StartsWith("spher"))
            {
                spheroid(executor, args);
                return;
            }
            if (alias.ToLower().StartsWith("circle"))
            {
                circle(executor, args);
                return;
            }
            if (alias.ToLower().StartsWith("oval"))
            {
                oval(executor, args);
                return;
            }

            Player p = executor as Player;

            List<int> blockTypes = new List<int>();
            List<int> byBlockTypes = new List<int>();
            bool insideBy = false;
            string fillType = null;
            bool slow = false, verySlow = false;

            bool modulo = true;

            bool sb, sf;
            foreach (string arggg in args)
            {
                string arg = arggg.ToLower();
                int multiplier = 1;
                if (arg == "and" || arg == "or" || arg == "using") continue;
                if (arg == "increment" || arg == "incremental" || arg == "incrementally") { modulo = false; continue; }
                if (arg == "modulo" || arg == "modulus") { modulo = true; continue; }
                if (arg == "slow") { slow = true; continue; }
                if (arg == "veryslow") { verySlow = true; continue; }
                if (arg.Contains("*"))
                {
                    string[] parts = arg.Split(new char[] { '*' });
                    int i1, i2;
                    if (int.TryParse(parts[0], out i1))
                    {
                        multiplier = i1;
                        arg = parts[1];
                    }
                    else if (int.TryParse(parts[1], out i2))
                    {
                        multiplier = i2;
                        arg = parts[0];
                    }
                    else
                    {
                        executor.commandOutput("&4Invalid argument: " + arggg);
                        return;
                    }
                }
                if (alias=="replace" && (arg == "by" || arg == "with"))
                {
                    insideBy = true;
                    continue;
                }
                fillType = parseFillType(arg, out sf) ?? fillType;
                if (!sf)
                {
                    if (!insideBy) blockTypes.AddIfNotNull((int)map.Block.parse(arg, out sb), multiplier);
                    else byBlockTypes.AddIfNotNull((int)map.Block.parse(arg, out sb), multiplier);
                }
                else sb = false;
                if (!sb && !sf)
                {
                    executor.commandOutput("&4Argument &a" + arggg + "&4 is invalid!");
                    return;
                }
            }

            if (alias == "replace")
            {
                if (!insideBy)
                {
                    if (blockTypes.Count == 2)
                    {
                        byBlockTypes.Add(blockTypes[1]);
                        blockTypes.RemoveAt(1);
                        insideBy = true;
                    }
                    else
                    {
                        executor.commandOutput("&4Need &a'&bwith&a'&4 or &a'&bby&a'&4 clause between the block types!");
                        return;
                    }
                }
                if (fillType == null) fillType = "solid";
            }
            else if (fillType == null) fillType = "solid";

            if (blockTypes.Count == 0) blockTypes.Add((int)map.Block.Type.STONE);

            map.Block b1 = getBlockInput(executor, "Please select the first corner");
            if (b1 == null)
            {
                executor.commandOutput("&a" + alias + " has been cancelled!");
                return;
            }
            if (p != null) p.flashBlock(b1);
            map.Block b2 = getBlockInput(executor, "Please select the second corner");
            if (b2 == null)
            {
                executor.commandOutput("&a" + alias + " has been cancelled!");
                return;
            }
            if (p != null) p.flashBlock(b2);

            if (b1.getMap() != b2.getMap())
            {
                executor.commandOutput("&4Error: Those blocks are not in the same map!");
                p.removeFlashingBlock(b1);
                p.removeFlashingBlock(b2);
                return;
            }

            bool solid = fillType == "solid";
            bool walls = fillType == "walls";
            bool grid = fillType == "grid";
            bool hollow = fillType == "hollow";
            
            int minX = Math.Min(b1.getX(), b2.getX());
            int maxX = Math.Max(b1.getX(), b2.getX());
            int minY = Math.Min(b1.getY(), b2.getY());
            int maxY = Math.Max(b1.getY(), b2.getY());
            int minZ = Math.Min(b1.getZ(), b2.getZ());
            int maxZ = Math.Max(b1.getZ(), b2.getZ());

            int setCount = 0;
            int actuallyModifiedCount = 0;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        if (solid ||
                            (
                                hollow
                                &&
                                (x == minX || x == maxX || y == minY || y == maxY || z == minZ || z == maxZ)
                            ) ||
                            (
                                walls
                                &&
                                (x == minX || x == maxX || z == minZ || z == maxZ)
                            ) ||
                            (
                                grid
                                &&
                                ((x == minX ? 1 : 0) + (x == maxX ? 1 : 0) + (y == minY ? 1 : 0) + (y == maxY ? 1 : 0) + (z == minZ ? 1 : 0) + (z == maxZ ? 1 : 0) >= 2)
                            ))
                        {

                            int currentContent = b1.getMap().getBlockContent(x, y, z);

                            if (insideBy)
                            {
                                if (!blockTypes.Contains(currentContent))
                                {
                                    continue;
                                }
                            }


                            int newType;
                            if (!insideBy)
                            {
                                if (modulo) newType = blockTypes[(x + y + z) % blockTypes.Count];
                                else newType = blockTypes[setCount % blockTypes.Count];
                            }
                            else
                            {
                                if (modulo) newType = byBlockTypes[(x + y + z) % byBlockTypes.Count];
                                else newType = byBlockTypes[setCount % byBlockTypes.Count];
                            }
                            if (currentContent != newType)
                            {
                                if (slow || verySlow) Thread.Sleep((slow?5:0)+(verySlow?25:0));
                                else if (actuallyModifiedCount%50 == 0) Thread.Sleep(1);
                                b1.getMap().setBlockContent(x, y, z, newType, true);
                                actuallyModifiedCount++;
                            }
                            setCount++;
                        }
                    }
                }
            }
            p.removeFlashingBlock(b1);
            p.removeFlashingBlock(b2);
        }
        private void oval(CommandExecutor executor, string[] args)
        {

        }
        private void spheroid(CommandExecutor executor, string[] args)
        {

        }
        private void circle(CommandExecutor executor, string[] args)
        {
            Player p = executor as Player;

            bool thinLine = true;
            bool angled = false;
            bool haveRadius = false;
            int radius = -1;
            foreach (string arggg in args)
            {
                string arg = arggg.ToLower();
                if (arg == "thin" || arg == "thinline") thinLine = true;
                else if (arg == "thick" || arg == "thickline") thinLine = false;
                else if (arg == "angled" || arg == "diagonal") angled = true;
                else if (int.TryParse(arg, out radius)) haveRadius = true;
                else
                {
                    executor.commandOutput("&4Invalid argument:&b "+arggg);
                    return;
                }
            }
            if (args.Length == 0)
            {
                
            }
            else if (args.Length == 1)
            {

            }
            else
            {
                executor.commandOutput("&4Invalid arguments!");
                executor.commandOutput("&bUsage:&a /circle [optional: radius]");
                return;
            }
            if (haveRadius && angled)
            {
                executor.commandOutput("&4You cannot both supply a radius and parameter '&bangled&4'");
                return;
            }
            else if (haveRadius && !angled)
            {
                if (!int.TryParse(args[0], out radius))
                {
                    executor.commandOutput("&4Invalid argument: &a" + args[0]);
                    return;
                }
                executor.commandOutput("&aDrawing circle with radius " + radius);
                map.Block center = getBlockInput(executor, "Please select the center block of the circle");
                if (center == null) return;
                if (p != null) p.flashBlock(center);

                while (true)
                {
                    map.Block plane1 = getBlockInput(executor, "&3Please select two random blocks within your circle");
                    if (plane1 == null) return;
                    if (p != null) p.flashBlock(plane1);
                    map.Block plane2 = getBlockInput(executor, "&3Please select one more block in your circle, not in a line with the other two");
                    if (plane2 == null) return;
                    if (p != null) p.flashBlock(plane2);

                    bool sameX = plane1.getX() == plane2.getX() && plane2.getX() == center.getX();
                    bool sameY = plane1.getY() == plane2.getY() && plane2.getY() == center.getY();
                    bool sameZ = plane1.getZ() == plane2.getZ() && plane2.getZ() == center.getZ();

                    if (center.getMap() != plane1.getMap() || plane1.getMap() != plane2.getMap())
                    {
                        executor.commandOutput("&4Error: The blocks you selected are not on the same map!");
                        if (p!=null) p.clearFlashingBlocks();
                        return;
                    }

                    if (plane1 == plane2 || plane1 == center || center == plane2)
                    {
                        executor.commandOutput("&4You selected the same block twice. Please don't.");
                        if (p != null)
                        {
                            p.removeFlashingBlock(plane1);
                            p.removeFlashingBlock(plane2);
                        }
                        continue;
                    }

                    if ((sameX && sameY) || (sameY && sameZ) || (sameX && sameZ))
                    {
                        executor.commandOutput("&4You selected 3 blocks in a straight line. Please don't,");
                        executor.commandOutput("&4otherwise we don't know if you want a horizontal or");
                        executor.commandOutput("&4vertical circle!");
                        if (p != null)
                        {
                            p.removeFlashingBlock(plane1);
                            p.removeFlashingBlock(plane2);
                        }
                        continue;
                    }

                    if (!sameX && !sameY && !sameZ)
                    {
                        executor.commandOutput("&4You didn't select blocks in the same plane!");
                        if (p != null)
                        {
                            p.removeFlashingBlock(plane1);
                            p.removeFlashingBlock(plane2);
                        }
                        continue;
                    }

                    float step = 1f / (2f * (float)Math.PI * radius * (thinLine?1:2));

                    for (float t = 0; t < 2 * Math.PI; t += step)
                    {
                        int x = -1, y = -1, z = -1;
                        if (sameX)
                        {
                            x = center.getX();
                            y = (int)(Math.Sin(t) * radius + center.getY() + 0.5f);
                            z = (int)(Math.Cos(t) * radius + center.getZ() + 0.5f);
                        }
                        if (sameY)
                        {
                            y = center.getY();
                            x = (int)(Math.Sin(t) * radius + center.getX() + 0.5f);
                            z = (int)(Math.Cos(t) * radius + center.getZ() + 0.5f);
                        }
                        if (sameZ)
                        {
                            z = center.getX();
                            y = (int)(Math.Sin(t) * radius + center.getY() + 0.5f);
                            x = (int)(Math.Cos(t) * radius + center.getX() + 0.5f);
                        }
                        try { center.getMap().setBlockContent(x, y, z, 1, true); }
                        catch { }
                    }
                    break;
                }
                if (p != null) p.clearFlashingBlocks();
            }
            else if (!haveRadius)
            {
                map.Block a = getBlockInput(executor, "Select three blocks on your circle");
                if (a == null) return;
                if (p != null) p.flashBlock(a);
                map.Block b = getBlockInput(executor, "Select the second block on your circle");
                if (b == null) return;
                if (p != null) p.flashBlock(b);
                map.Block c = getBlockInput(executor, "Select the third block on your circle");
                if (c == null) return;
                if (p != null) p.flashBlock(c);

                if (a.getMap() != b.getMap() || b.getMap() != c.getMap())
                {
                    executor.commandOutput("&4The blocks you selected are not on the same map!");
                    return;
                }

                bool sameX = a.getX() == b.getX() && b.getX() == c.getX();
                bool sameY = a.getY() == b.getY() && b.getY() == c.getY();
                bool sameZ = a.getZ() == b.getZ() && b.getZ() == c.getZ();

                if (sameX || sameY || sameZ)
                {
                    executor.commandOutput("&4Error: Cannot draw circle through three blocks in a straight line!");
                    return;
                }

                
                /*Vector dAB = new Vector(b) - new Vector(a);
                Vector betweenAB = new Vector(a) + dAB / 2;

                Vector dBC = new Vector(c) - new Vector(b);
                Vector betweenBC = new Vector(b) + dBC / 2;*/

                // Looking at it along X axis, Z becomes X temporarily

                Vector2 circleCenter1 = circleCenter(new Vector(a).ZY(), new Vector(b).ZY(), new Vector(c).ZY());

            }
            else
            {
                // this can nay happen
            }
        }
        Vector2 circleCenter(Vector2 A, Vector2 B, Vector2 C)
        {
            double yDelta_a = B.y - A.y;
            double xDelta_a = B.x - A.x;

            double yDelta_b = C.y - B.y;
            double xDelta_b = C.x - B.x;

            double aSlope = yDelta_a / xDelta_a;
            double bSlope = yDelta_b / xDelta_b;
            double centerX = (aSlope*bSlope * (A.y - C.y) + bSlope * (A.x + B.x) - aSlope * (B.x + C.x)) / (2*(bSlope - aSlope));
            double centerY = -1 * (centerX - (A.x + B.x) / 2) / aSlope + (A.y + B.y) / 2;
            return new Vector2(centerX, centerY);
        }
    }
}
