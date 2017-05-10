using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdConfig : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "config";
        }

        public override string[] getAliases() { return new string[] { "loadranks", "reloadranks", "readranks", "saveranks", "writeranks" }; }

        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
                new ParamType[]{ParamType.STRING_WITH_SPACES},
            };
        }

        public override int getDefaultPermissionLevel()
        {
            return 1000;
        }

        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            if (alias == "config")
            {
                executor.commandOutput(getHelp());
                return;
            }
            string[] parts = arguments[0].stringValue.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
            if (alias == "loadranks" || alias == "reloadranks" || alias == "readranks")
            {
                Config.loadRanksConfig();
                executor.commandOutput("Ranks loaded from ranks.json!");
                return;
            }
            else if (alias == "saveranks" || alias == "writeranks")
            {
                Config.loadRanksConfig();
                executor.commandOutput("Ranks written to ranks.json!");
                return;
            }
            else if (alias == "config")
            {
                if (parts[0].ToLower() == "reload" || parts[0].ToLower() == "read" || parts[0].ToLower() == "load")
                {
                    if (parts.Length >= 2)
                    {
                        if (parts[1].ToLower() == "ranks" || parts[1].ToLower() == "ranks.json")
                        {
                            Config.loadRanksConfig();
                            executor.commandOutput("Ranks loaded from ranks.json!");
                        }
                        else if (parts[1].ToLower() == "players" || parts[1].ToLower() == "players.json")
                        {
                            Config.reloadOfflinePlayers(); // TODO Does this even work correctly?
                            executor.commandOutput("Players loaded from players.json!");
                        }
                        else if (parts[1].ToLower() == "server" || parts[1].ToLower() == "serverconfig" || parts[1].ToLower() == "server.json")
                        {
                            Config.loadServerConfig();
                            executor.commandOutput("Server config loaded from server.json!");
                        }
                        else if (parts[1].ToLower() == "map")
                        {
                            if (parts.Length >= 3)
                            {
                                map.Map m = map.Map.getMap(parts[2]);
                                if (m == null)
                                {
                                    executor.commandOutput("&4Map named&a " + parts[2] + "&4 does not exist!");
                                }
                                else
                                {
                                    m.reloadConfig();
                                    executor.commandOutput("Map config loaded from maps/" + m.getName() + "/config.json!");
                                }
                            }
                            else
                            {
                                executor.commandOutput("&4Usage:&b /config&a reload map [&bmapname&a]");
                            }
                        }
                        else
                        {
                            executor.commandOutput("&4You can only reload:&b ranks&4,&b server&4,&b map");
                        }
                    }
                    else
                    {
                        executor.commandOutput("&4Usages:");
                        executor.commandOutput("-&a /config load ranks");
                        executor.commandOutput("-&a /config load players");
                        executor.commandOutput("-&a /config load server");
                        executor.commandOutput("-&a /config load map [mapName]");
                    }
                }
                else if (parts[0].ToLower() == "save" || parts[0].ToLower() == "write")
                {
                    if (parts.Length >= 2)
                    {
                        if (parts[1].ToLower() == "ranks" || parts[1].ToLower() == "ranks.json")
                        {
                            Config.saveRanksConfig();
                            executor.commandOutput("Ranks saved to ranks.json!");
                        }
                        else if (parts[1].ToLower() == "players" || parts[1].ToLower() == "players.json")
                        {
                            Config.saveOfflinePlayers();
                            executor.commandOutput("Players saved to players.json!");
                        }
                        else if (parts[1].ToLower() == "server" || parts[1].ToLower() == "serverconfig" || parts[1].ToLower() == "server.json")
                        {
                            Config.saveServerConfig();
                            executor.commandOutput("Server config saved to server.json!");
                        }
                        else if (parts[1].ToLower() == "map")
                        {
                            if (parts.Length == 3)
                            {
                                map.Map m = map.Map.getMap(parts[2]);
                                if (m == null)
                                {
                                    executor.commandOutput("&4Map named&a " + parts[2] + "&4 does not exist!");
                                }
                                else
                                {
                                    m.saveConfig();
                                    executor.commandOutput("Map config saved to maps/" + m.getName() + "/config.json!");
                                }
                            }
                            else
                            {
                                executor.commandOutput("&4Usage:&b /config&a save map [&bmapname&a]");
                            }
                        }
                        else
                        {
                            executor.commandOutput("&4You can only save:&b ranks&4,&b server&4,&b map");
                        }
                    }
                    else
                    {
                        executor.commandOutput("&4Usages:");
                        executor.commandOutput("-&a /config save ranks");
                        executor.commandOutput("-&a /config save players");
                        executor.commandOutput("-&a /config save server");
                        executor.commandOutput("-&a /config save map [mapName]");
                    }
                }
                else if (parts[0].ToLower() == "set")
                {
                    if (parts.Length < 2 || (parts[1].ToLower() != "map" || parts[1].ToLower() != "server"))
                    {
                        executor.commandOutput("&4Usages:");
                        executor.commandOutput("-&a /config set map [configValue] [newValue]");
                        executor.commandOutput("-&a /config set map [mapName] [configValue] [newValue]");
                        executor.commandOutput("-&a /config set server [configValue] [newValue]");
                    }
                    else if (parts[1].ToLower() == "map")
                    {
                        map.Map m = null;
                        if (parts.Length >= 3 && (m=map.Map.getMap(parts[2])) == null)
                        {
                            executor.commandOutput("&4Cannot find map&b " + parts[2] + "&4!!");
                        }
                        else if (parts.Length >= 5 && parts[3].ToLower() == "hacksallowed")
                        {
                            m.setHacksAllowed(parts[4].ToLower() == "true");
                            executor.commandOutput("&bhacksAllowed&a was set to&b " + m.getHacksAllowed());
                        }
                        else if (parts.Length >= 5 && parts[3].ToLower().StartsWith("visitpermission"))
                        {
                            // TODO
                        }
                        else if (parts.Length >= 5 && parts[3].ToLower().StartsWith("buildpermission"))
                        {
                            // TODO
                        }
                        else if (parts.Length >= 5 && parts[3].ToLower().StartsWith("placeblockpermission"))
                        {
                            // TODO
                        }
                        else if (parts.Length >= 5 && parts[3].ToLower().StartsWith("removeblockpermission"))
                        {
                            // TODO
                        }
                        else
                        {
                            executor.commandOutput("&4Usages:");
                            executor.commandOutput("-&a /config set map [configValue] [newValue]");
                        }
                    }
                    else if (parts[1].ToLower() == "server")
                    {

                    }
                }
                else
                {
                    executor.commandOutput("&4Usages:");
                    executor.commandOutput("-&a /config set ...");
                    executor.commandOutput("-&a /config reload ...");
                    executor.commandOutput("-&a /config save ...");
                }
            }
        }
    }
}
