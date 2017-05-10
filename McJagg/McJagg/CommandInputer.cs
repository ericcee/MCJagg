using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace McJagg
{
    public class CommandInputer
    {
        private command.Command cmd;
        private command.Command.InputType type;
        private bool ready = false;
        private bool canceled = false;

        private map.Block blockValue = null;
        private Location locationValue = null;
        private bool yesValue = false;
        private command.CommandExecutor executor = null;

        public command.Command.InputType getInputType()
        {
            return type;
        }
        public CommandInputer(command.Command cmd, command.CommandExecutor executor, command.Command.InputType type)
        {
            this.cmd = cmd;
            this.executor = executor;
            this.type = type;
        }
        public void waitUntilReady()
        {
            while (!ready) { Thread.Sleep(300); }
        }
        public bool isYes()
        {
            waitUntilReady();
            executor.commandInputer = null;
            if (canceled) return false;
            return yesValue;
        }
        public map.Block getBlock()
        {
            waitUntilReady();
            executor.commandInputer = null;
            if (canceled) return null;
            return blockValue;
        }
        public Location getLocation()
        {
            waitUntilReady();
            executor.commandInputer = null;
            if (canceled) return null;
            return locationValue;
        }
        public void setLocationInput(Location loc)
        {
            if (this.type != command.Command.InputType.PLAYER_LOCATION_SELECTION)
            {
                throw new Exception("This CommandInputer is not intended for player location!");
            }
            this.locationValue = loc;
            this.ready = true;
        }
        public void setBlockInput(map.Block b)
        {
            if (this.type != command.Command.InputType.BLOCK_SELECTION)
            {
                throw new Exception("This CommandInputer is not intended for block input!");
            }
            this.blockValue = b;
            this.ready = true;
        }
        public void setYesCancelInput(bool yes)
        {
            if (this.type != command.Command.InputType.YES_CANCEL_CONFIRMATION)
            {
                throw new Exception("This CommandInputer is not intended for yes cancel input!");
            }
            this.yesValue = yes;
            this.ready = true;
        }
        public void cancel()
        {
            canceled = true;
            ready = true;
        }
    }
}
