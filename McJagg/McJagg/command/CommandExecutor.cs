using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    public abstract class CommandExecutor
    {
        public CommandInputer commandInputer = null;
        public abstract void commandOutput(string message);
        public abstract string name();
    }
}
