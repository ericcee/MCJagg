using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace McJagg.command
{
    public abstract class Command
    {
        public enum InputType
        {
            BLOCK_SELECTION,
            YES_CANCEL_CONFIRMATION,
            PLAYER_LOCATION_SELECTION,
        }
        public enum ParamType
        {
            BLOCK_LOCATION,
            PLAYER_LOCATION,
            OFFLINE_PLAYER,
            ONLINE_PLAYER_NULL,
            ONLINE_PLAYER_NOT_NULL,
            MAP,
            STRING_WITHOUT_SPACES,
            STRING_WITH_SPACES,
            INTEGER,
            FLOAT,
        }

        //public static List<Command> all = new List<Command>();

        public CommandExecutor executor = null;

        public static List<Type> allCmdTypes = new List<Type>();
        public static List<string> allCmdNames = new List<string>();
        public static List<string[]> allCmdAliases = new List<string[]>();

        public static void loadCommands()
        {
            Type[] commandClasses = Assembly.GetExecutingAssembly().GetTypes().Where(t => (t.BaseType == typeof(Command))).ToArray();
            foreach (Type t in commandClasses)
            {
                allCmdTypes.Add(t);
                Command cmd = (Command)Activator.CreateInstance(t);
                allCmdNames.Add(cmd.getName());
                allCmdAliases.Add(cmd.getAliases());
            }
        }

        public abstract string getName();
        public abstract string getHelp();
        public virtual string[] getAliases() { return null; }
        public abstract ParamType[][] getParamTypes();
        public abstract int getDefaultPermissionLevel();
        public abstract void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias);

        public string getDefaultHelp()
        {
            string help = "&bUsage of&a /"+getName()+"&b:\n";
            foreach (ParamType[] pars in getParamTypes())
            {
                help += "&a/"+getName()+"&b";
                foreach (ParamType par in pars)
                {
                    if (par==ParamType.BLOCK_LOCATION) help += " [blockX] [blockY] [blockZ]";
                    if (par==ParamType.PLAYER_LOCATION) help += " [playerX] [playerY] [playerZ]";
                    if (par==ParamType.OFFLINE_PLAYER) help += " [player]";
                    if (par==ParamType.ONLINE_PLAYER_NULL) help += " [player]";
                    if (par==ParamType.ONLINE_PLAYER_NOT_NULL) help += " [player]";
                    if (par==ParamType.MAP) help += " [map]";
                    if (par==ParamType.STRING_WITHOUT_SPACES) help += " [string]"; 
                    if (par==ParamType.STRING_WITH_SPACES) help += " [big string]";
                    if (par==ParamType.INTEGER) help += " [int]";
                    if (par==ParamType.FLOAT) help += " [float]";
                }
                help += "\n";
            }
            return help.Trim();
        }

        public int getPermissionLevel()
        {
            if (Config.ranks.commandPermissions.ContainsKey(getName().ToLower()))
            {
                return Config.ranks.commandPermissions[getName().ToLower()];
            }
            return getDefaultPermissionLevel();
        }

        public map.Block getBlockInput(CommandExecutor executor, string message)
        {
            if (executor.commandInputer != null)
            {
                executor.commandOutput("&4Something went wrong: already waiting for input?!?");
                return null;
            }
            executor.commandOutput("&a" + message);
            executor.commandOutput("&aClick or place a block to select it or type &b/cancel&a to cancel");
            executor.commandInputer = new CommandInputer(this, executor, InputType.BLOCK_SELECTION);
            return executor.commandInputer.getBlock();
        }

        public map.Block getLocationInput(CommandExecutor executor, string message)
        {
            if (executor.commandInputer != null)
            {
                executor.commandOutput("&4Something went wrong: already waiting for input?!?");
                return null;
            }
            executor.commandOutput("&a" + message);
            executor.commandOutput("&aType &b/locate&a to select your location or &b/cancel&a to cancel");
            executor.commandInputer = new CommandInputer(this, executor, InputType.PLAYER_LOCATION_SELECTION);
            return executor.commandInputer.getBlock();
        }
        public bool getConfirmation(CommandExecutor executor, string message)
        {
            if (executor.commandInputer != null)
            {
                executor.commandOutput("&4Something went wrong: already waiting for input?!?");
                return false;
            }
            executor.commandOutput("&a" + message);
            executor.commandOutput("&aType &b/yes&a to confirm or &b/cancel&a to cancel");
            executor.commandInputer = new CommandInputer(this, executor, InputType.YES_CANCEL_CONFIRMATION);
            return executor.commandInputer.isYes();
        }

        public static List<ParamType> getArgumentPotentials(string arg)
        {
            Console.WriteLine("getArgumentPotentials(" + arg + ")");
            List<ParamType> potentials = new List<ParamType>();
            float dummy1;
            int dummy2;
            if (int.TryParse(arg, out dummy2))
            {
                potentials.Add(ParamType.INTEGER);
                potentials.Add(ParamType.BLOCK_LOCATION);
                Console.WriteLine("-int");
            }
            if (float.TryParse(arg, out dummy1))
            {
                potentials.Add(ParamType.FLOAT);
                potentials.Add(ParamType.PLAYER_LOCATION);
                Console.WriteLine("-float");
            }
            if (map.Map.exists(arg))
            {
                potentials.Add(ParamType.MAP);
                Console.WriteLine("-map");
            }
            potentials.Add(ParamType.STRING_WITHOUT_SPACES);
            potentials.Add(ParamType.STRING_WITH_SPACES);
            potentials.Add(ParamType.ONLINE_PLAYER_NULL);
            potentials.Add(ParamType.ONLINE_PLAYER_NOT_NULL);
            potentials.Add(ParamType.OFFLINE_PLAYER);
            potentials.Add(ParamType.BLOCK_LOCATION);
            potentials.Add(ParamType.PLAYER_LOCATION);
            return potentials;
        }

        public static CommandArgument parseArgumentAsType(string arg, ParamType type, out string error)
        {
            error = null;
            if (type == ParamType.INTEGER)
            {
                return CommandArgument.makeInt(int.Parse(arg));
            }
            if (type == ParamType.FLOAT)
            {
                return CommandArgument.makeFloat(float.Parse(arg));
            }
            if (type == ParamType.MAP)
            {
                return CommandArgument.makeMap(map.Map.getMap(arg));
            }
            if (type == ParamType.OFFLINE_PLAYER)
            {
                return CommandArgument.makeOfflinePlayer(OfflinePlayer.getOfflinePlayer(arg));
            }
            if (type == ParamType.ONLINE_PLAYER_NULL)
            {
                bool multipleLikeThatOnline;
                Player p = Player.getPlayerStartingWith(arg, out multipleLikeThatOnline);
                if (multipleLikeThatOnline)
                {
                    error = "&4Multiple players starting with &b" + arg + "&4 are online!";
                    return null;
                }
                else return CommandArgument.makeOnlinePlayer(p);
            }
            if (type == ParamType.ONLINE_PLAYER_NOT_NULL)
            {
                bool multipleLikeThatOnline;
                Player p = Player.getPlayerStartingWith(arg, out multipleLikeThatOnline);
                if (multipleLikeThatOnline)
                {
                    error = "&4Multiple players starting with &b" + arg + "&4 are online!";
                    return null;
                }
                else if (p != null) return CommandArgument.makeOnlinePlayer(p);
                else
                {
                    error = "&4No player starting with &b"+arg+"&4 is online!";
                    return null;
                }
            }
            if (type == ParamType.STRING_WITH_SPACES)
            {
                return CommandArgument.makeString(arg, type);
            }
            if (type == ParamType.STRING_WITHOUT_SPACES)
            {
                return CommandArgument.makeString(arg, type);
            }
            Console.WriteLine("fail in parseArgumentType: unknown type " + (int)type);
            Console.ReadLine();
            throw new Exception("fail in parseArgumentType: unknown type");
            return null;
        }
        public static CommandArgument parseArgumentAsType(string arg1, string arg2, string arg3, ParamType type)
        {
            return parseArgumentAsType(double.Parse(arg1), double.Parse(arg2), double.Parse(arg3), type);
        }
        public static CommandArgument parseArgumentAsType(double arg1, double arg2, double arg3, ParamType type)
        {
            return parseArgumentAsType(arg1, arg2, arg3, 0, 0, type);
        }
        public static CommandArgument parseArgumentAsType(double arg1, double arg2, double arg3, byte arg4, byte arg5, ParamType type)
        {
            if (type == ParamType.BLOCK_LOCATION)
            {
                return CommandArgument.makeBlockLocation((int)arg1, (int)arg2, (int)arg3);
            }
            if (type == ParamType.PLAYER_LOCATION)
            {
                return CommandArgument.makePlayerLocation(arg1, arg2, arg3, arg4, arg5);
            }

            Console.WriteLine("fail in parseArgumentType 2: unknown type " + (int)type);
            Console.ReadLine();
            throw new Exception("fail in parseArgumentType 2: unknown type");
            return null;
        }

        public static bool execute(string chatMessage, CommandExecutor executor)
        {
            //Console.WriteLine("chatMessage="+chatMessage);
            chatMessage += " ";
            string commandName = null;
            List<string> arguments = new List<string>();
            List<int> argumentStartIndexes = new List<int>();
            List<List<ParamType>> argumentPotentials = new List<List<ParamType>>();
            Command theCommand = null;
            int previousSpaceIndex = -1;
            for (int i=0; i<chatMessage.Length; i++)
            {
                char c = chatMessage[i];
                //Console.WriteLine("c="+c);
                if (i == 0)
                {
                    if (c != '/')
                    {
                        throw new Exception("Command.execute() was called on a string that does not start with a slash:\r\n\"" + chatMessage + "\"");
                    }
                    //Console.WriteLine("found slash");
                    continue;
                }
                /*if (i==1)
                {
                    if (c == ' ')
                    {
                        return false;
                    }
                    continue;
                }*/
                if (previousSpaceIndex == i-1 && c == ' ')
                {
                    previousSpaceIndex = i;
                    //Console.WriteLine("skipping repeated space");
                    continue;
                }
                if (theCommand == null && c == ' ')
                {
                    previousSpaceIndex = i;
                    commandName = chatMessage.Substring(1, i - 1).ToLower();
                    //Console.WriteLine("command name=" + commandName + "=");
                    for (int j=0; j<allCmdNames.Count; j++)
                    {
                        if (allCmdNames[j].ToLower() == commandName.ToLower())
                        {
                            theCommand = (Command)Activator.CreateInstance(allCmdTypes[j]);
                            goto foundCommand;
                        }
                        if (allCmdAliases[j] != null)
                        {
                            foreach (string alias in allCmdAliases[j])
                            {
                                if (alias.ToLower() == commandName.ToLower())
                                {
                                    theCommand = (Command)Activator.CreateInstance(allCmdTypes[j]);
                                    goto foundCommand;
                                }
                            }
                        }
                    }
                    foundCommand:
                    if (theCommand == null)
                    {
                        executor.commandOutput("That command does not exist!");
                        return false;
                    }

                    Player player = executor as Player;
                    if (player != null &&
                        player.getPermissionLevel() < theCommand.getPermissionLevel())
                    {
                        executor.commandOutput("You don't have permission to use this command.");
                        executor.commandOutput("Your permission: " + player.getPermissionLevel());
                        executor.commandOutput("Permission needed: " + theCommand.getPermissionLevel());
                        return false;
                    }

                    continue;
                }
                if (theCommand!=null && c == ' ' && previousSpaceIndex < i-1)
                {
                    string str = chatMessage.Substring(previousSpaceIndex + 1, i - previousSpaceIndex - 1);
                    Console.WriteLine("arg=" + str + "=");
                    arguments.Add(str);
                    argumentStartIndexes.Add(previousSpaceIndex + 1);
                    argumentPotentials.Add(getArgumentPotentials(str));
                    previousSpaceIndex = i;

                    continue;
                }
            }

            if (theCommand == null) return false;

            List<int> potentialOverloads = new List<int>();
            List<int> potentialOverloadsParamIndex = new List<int>();
            List<int> potentialOverloadsParamPartsFullfilled = new List<int>();
            List<int> potentialOverloadsContextPlayerUsedParamIndex = new List<int>();
            List<bool> potentialOverloadsInsideStringWithSpaces = new List<bool>();

            for (int i = 0; i < theCommand.getParamTypes().Length; i++)
            {
                potentialOverloads.Add(i);
                potentialOverloadsParamIndex.Add(0);
                potentialOverloadsParamPartsFullfilled.Add(0);
                potentialOverloadsContextPlayerUsedParamIndex.Add(-1);
                potentialOverloadsInsideStringWithSpaces.Add(false);
            }

            // Loop over the arguments
            for (int i = 0; i < arguments.Count; i++)
            {
                //Console.WriteLine("Current argument="+arguments[i]);

                // Loop over the parameter overloads
                for (int j = 0; j < potentialOverloads.Count; j++)
                {
                    // If the current overload type does not have enough parameters,
                    // we can eliminate it
                    if (potentialOverloadsParamIndex[j] >= theCommand.getParamTypes()[potentialOverloads[j]].Length)
                    {
                        //Console.WriteLine("Eliminated "+j+", not enough params");
                        potentialOverloads.RemoveAt(j);
                        potentialOverloadsParamIndex.RemoveAt(j);
                        potentialOverloadsParamPartsFullfilled.RemoveAt(j);
                        potentialOverloadsContextPlayerUsedParamIndex.RemoveAt(j);
                        potentialOverloadsInsideStringWithSpaces.RemoveAt(j);
                        j--;
                        continue;
                    }

                    // Get the current overload's current parameter type
                    ParamType pt = theCommand.getParamTypes()[potentialOverloads[j]][potentialOverloadsParamIndex[j]];

                    // If the current parameter type does not match the current argument,
                    // we can eliminate this overload
                    if (!argumentPotentials[i].Contains(pt))
                    {
                        //Console.WriteLine("potentials of current arg don't contain paramtype " + (int)pt);
                        if ((pt == ParamType.OFFLINE_PLAYER ||
                            pt == ParamType.ONLINE_PLAYER_NOT_NULL ||
                            pt == ParamType.ONLINE_PLAYER_NULL ||
                            pt == ParamType.BLOCK_LOCATION ||
                            pt == ParamType.PLAYER_LOCATION) &&
                            potentialOverloadsContextPlayerUsedParamIndex[j] == -1 &&
                            potentialOverloadsParamPartsFullfilled[j] == 0 &&
                            (executor is Player))
                        {
                            // A parameter is not present, but we can use the context!
                            //Console.WriteLine("Does not match, but context was assumed!");
                            potentialOverloadsContextPlayerUsedParamIndex[j] = potentialOverloadsParamIndex[j];
                            potentialOverloadsParamIndex[j]++;
                        }
                        else
                        {
                            //Console.WriteLine("Eliminated " + potentialOverloads[j] + ", does not match");
                            potentialOverloads.RemoveAt(j);
                            potentialOverloadsParamIndex.RemoveAt(j);
                            potentialOverloadsParamPartsFullfilled.RemoveAt(j);
                            potentialOverloadsContextPlayerUsedParamIndex.RemoveAt(j);
                            potentialOverloadsInsideStringWithSpaces.RemoveAt(j);
                            j--;
                            continue;
                        }
                    }
                    else
                    {
                        double dummy;
                        if (pt == ParamType.BLOCK_LOCATION || pt == ParamType.PLAYER_LOCATION)
                        {
                            //Console.WriteLine("Matches, it's part of a block or player location!");
                            if (!double.TryParse(arguments[i], out dummy))
                            {
                                //Console.WriteLine("a player, only one!");
                                potentialOverloadsParamIndex[j]++;
                            }
                            else
                            {
                                //Console.WriteLine("a number part, one of three!");
                                potentialOverloadsParamPartsFullfilled[j]++;
                                if (potentialOverloadsParamPartsFullfilled[j] == 3)
                                {
                                    potentialOverloadsParamPartsFullfilled[j] = 0;
                                    potentialOverloadsParamIndex[j]++;
                                }
                            }
                        }
                        else if (pt == ParamType.STRING_WITH_SPACES)
                        {
                            //Console.WriteLine("Matches, it's part of a string with spaces!");
                            potentialOverloadsInsideStringWithSpaces[j] = true;
                            //potentialOverloadsParamIndex[j]++;
                        }
                        else if ((pt == ParamType.ONLINE_PLAYER_NULL ||
                            pt == ParamType.ONLINE_PLAYER_NOT_NULL) &&
                            double.TryParse(arguments[i], out dummy) &&
                            Player.getPlayer(arguments[i]) == null)
                        {
                            //Console.WriteLine("Does not match because player " + arguments[i] + " is not online, but context was assumed!");
                            potentialOverloadsContextPlayerUsedParamIndex[j] = potentialOverloadsParamIndex[j];
                            potentialOverloadsParamIndex[j]++;
                            j--;
                            continue;
                        }
                        else if (pt == ParamType.OFFLINE_PLAYER &&
                            double.TryParse(arguments[i], out dummy) &&
                            OfflinePlayer.getOfflinePlayer(arguments[i]) == null)
                        {
                            //Console.WriteLine("Does not match because offline player " + arguments[i] + " is not found, but context was assumed!");
                            potentialOverloadsContextPlayerUsedParamIndex[j] = potentialOverloadsParamIndex[j];
                            potentialOverloadsParamIndex[j]++;
                            j--;
                            continue;
                        }
                        else
                        {
                            //Console.WriteLine("Matches directly!");
                            potentialOverloadsParamIndex[j]++;
                        }
                    }
                }
            }

            for (int i = 0; i < potentialOverloads.Count; i++)
            {
                //Console.WriteLine("Checking if we have enough args for " + potentialOverloads[i]);
                if (theCommand.getParamTypes()[potentialOverloads[i]].Length > potentialOverloadsParamIndex[i] &&
                    !potentialOverloadsInsideStringWithSpaces[i])
                {
                    ParamType pt = theCommand.getParamTypes()[potentialOverloads[i]][potentialOverloadsParamIndex[i]];

                    if ((pt == ParamType.OFFLINE_PLAYER ||
                                pt == ParamType.ONLINE_PLAYER_NULL ||
                                pt == ParamType.ONLINE_PLAYER_NOT_NULL ||
                                pt == ParamType.BLOCK_LOCATION ||
                                pt == ParamType.PLAYER_LOCATION) &&
                                potentialOverloadsParamIndex[i] + 1 == theCommand.getParamTypes()[potentialOverloads[i]].Length &&
                        potentialOverloadsContextPlayerUsedParamIndex[i] == -1)
                    {
                        // This overload needs one more argument, and we can get it from the context!
                        //Console.WriteLine(potentialOverloads[i] + " needed one more, got it from context!");
                        potentialOverloadsContextPlayerUsedParamIndex[i] = potentialOverloadsParamIndex[i];
                    }
                    else if (potentialOverloadsParamIndex[i] < theCommand.getParamTypes()[potentialOverloads[i]].Length)
                    {
                        //Console.WriteLine("Not enough for " + potentialOverloads[i]);
                        potentialOverloads.RemoveAt(i);
                        potentialOverloadsContextPlayerUsedParamIndex.RemoveAt(i);
                        potentialOverloadsParamIndex.RemoveAt(i);
                        potentialOverloadsParamPartsFullfilled.RemoveAt(i);
                        i--;
                    }
                }
                else
                {
                    //Console.WriteLine("It has enough");
                }
            }
            if (potentialOverloads.Count > 1)
            {
                // Try to resolve ambiguity by removing the overloads that used the player context.
                // The most vocal / direct / similar / equal possibility should take precedence.
                //Console.WriteLine("Attempting to resolve ambiguity...");
                for (int i = 0; i < potentialOverloads.Count - 1; i++)
                {
                    if ((potentialOverloadsContextPlayerUsedParamIndex[i] == -1) != (potentialOverloadsContextPlayerUsedParamIndex[i + 1] == -1))
                    {
                        if (potentialOverloadsContextPlayerUsedParamIndex[i] != -1)
                        {
                            //Console.WriteLine(potentialOverloads[i]+" removed!");
                            potentialOverloads.RemoveAt(i);
                            potentialOverloadsParamIndex.RemoveAt(i);
                            potentialOverloadsParamPartsFullfilled.RemoveAt(i);
                            potentialOverloadsContextPlayerUsedParamIndex.RemoveAt(i);
                            i--;
                            continue;
                        }
                        else 
                        {
                            //Console.WriteLine(potentialOverloads[i+1] + " removed!");
                            potentialOverloads.RemoveAt(i + 1);
                            potentialOverloadsParamIndex.RemoveAt(i + 1);
                            potentialOverloadsParamPartsFullfilled.RemoveAt(i + 1);
                            potentialOverloadsContextPlayerUsedParamIndex.RemoveAt(i + 1);
                            continue;
                        }
                    }
                }
                if (potentialOverloads.Count > 1)
                {
                    Console.WriteLine("Command is ambigious! not executing it...");
                    executor.commandOutput("Ambiguous arguments!");
                    return false;
                }
            }
            if (potentialOverloads.Count < 1)
            {
                Console.WriteLine("Command has invalid arguments! not executing it...");
                executor.commandOutput("Invalid arguments!");
                return false;
            }

            Console.WriteLine("The correct overload is " + potentialOverloads[0]);

            CommandArgument[] args = new CommandArgument[theCommand.getParamTypes()[potentialOverloads[0]].Length];

            int paramIndex = 0;

            for (int i = 0; i < arguments.Count; i++)
            {
                Console.WriteLine("arg="+arguments[i]+"=");
                ParamType type = theCommand.getParamTypes()[potentialOverloads[0]][paramIndex];
                if (type == ParamType.BLOCK_LOCATION ||
                    type == ParamType.PLAYER_LOCATION)
                {
                    if (potentialOverloadsContextPlayerUsedParamIndex[0] == paramIndex)
                    {
                        // Locatio ex contexto
                        Console.WriteLine("locatio ex contexto");
                        args[paramIndex] = parseArgumentAsType(((Player)executor).getX(), ((Player)executor).getY(), ((Player)executor).getZ(), type);
                        i--; // we don't consume this argument
                    }
                    else if (argumentPotentials[i].Contains(ParamType.BLOCK_LOCATION) ||
                        argumentPotentials[i].Contains(ParamType.PLAYER_LOCATION))
                    {
                        double dummy;
                        if (double.TryParse(arguments[i], out dummy))
                        {
                            Console.WriteLine("consuming three arguments for location...");
                            try
                            {
                                args[paramIndex] = parseArgumentAsType(arguments[i], arguments[i + 1], arguments[i + 2], type);
                                i += 2; // we consumed two extra arguments.
                            }
                            catch
                            {
                                executor.commandOutput("Invalid arguments!");
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("consuming one player for location...");
                            Player p = Player.getPlayer(arguments[i]);
                            if (p == null)
                            {
                                executor.commandOutput("Player " + arguments[i] + " cannot be found!");
                                return false;
                            }
                            args[paramIndex] = parseArgumentAsType(p.getX(), p.getY(), p.getZ(), p.getYaw(), p.getPitch(), type);
                        }
                    }
                    else
                    {
                        Console.WriteLine("consuming one player for location...");
                        Player p = Player.getPlayer(arguments[i]);
                        if (p==null)
                        {
                            executor.commandOutput("Player "+arguments[i]+" cannot be found!");
                            return false;
                        }
                        args[paramIndex] = parseArgumentAsType(p.getX(), p.getY(), p.getZ(), p.getYaw(), p.getPitch(), type);
                    }
                }
                else if (type == ParamType.STRING_WITH_SPACES)
                {
                    Console.WriteLine("Starting string with spaces");
                    string str = chatMessage.Substring(argumentStartIndexes[i], chatMessage.Length - argumentStartIndexes[i] - 1);
                    args[paramIndex] = CommandArgument.makeString(str, type);
                    break;
                }
                else if ((type == ParamType.ONLINE_PLAYER_NOT_NULL ||
                    type == ParamType.ONLINE_PLAYER_NULL ||
                    type == ParamType.OFFLINE_PLAYER) &&
                    potentialOverloadsContextPlayerUsedParamIndex[0] == paramIndex)
                {
                    // playero ex contexto
                    Console.WriteLine("playero ex contexto");
                    string errorStr;
                    args[paramIndex] = parseArgumentAsType(((Player)executor).getName(), type, out errorStr);
                    i--; // we don't consume this argument
                }
                else
                {
                    string errorStr;
                    args[paramIndex] = parseArgumentAsType(arguments[i], type, out errorStr);
                    if (errorStr != null)
                    {
                        executor.commandOutput("&4" + errorStr);
                        return false;
                    }
                }
                paramIndex++;
            }

            if (paramIndex == potentialOverloadsContextPlayerUsedParamIndex[0])
            {
                ParamType pt = theCommand.getParamTypes()[potentialOverloads[0]][potentialOverloadsContextPlayerUsedParamIndex[0]];
                Console.WriteLine("We need a context arg at the end of type "+(int)pt);
                Player p = executor as Player;
                if (p == null)
                {
                    executor.commandOutput("No player available :( :( bad problem :( cannot be found!");
                    return false;
                }
                if (pt == ParamType.BLOCK_LOCATION ||
                    pt == ParamType.PLAYER_LOCATION)
                {
                    args[paramIndex] = parseArgumentAsType(p.getX(), p.getY(), p.getZ(), p.getYaw(), p.getPitch(), pt);
                }
                else if (pt == ParamType.OFFLINE_PLAYER ||
                         pt == ParamType.ONLINE_PLAYER_NULL ||
                         pt == ParamType.ONLINE_PLAYER_NOT_NULL)
                {
                    string errorStr;
                    args[paramIndex] = parseArgumentAsType(p.getName(), pt, out errorStr);
                    if (errorStr != null)
                    {
                        executor.commandOutput("&4" + errorStr);
                        return false;
                    }
                }
                else
                {
                    throw new Exception("Shit! a bad error occurred");
                }
            }

            try
            {
                theCommand.executor = executor;
                theCommand.execute(potentialOverloads[0], executor, args, commandName);
            }
            catch (Exception e)
            {
                executor.commandOutput("ERROR: " + e.Message);
                executor.commandOutput(e.StackTrace);
            }

            return true;
        }
    }
}
