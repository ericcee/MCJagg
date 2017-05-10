using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.command
{
    class CmdShowHideWindow : Command
    {
        public override string getHelp()
        {
            return getDefaultHelp();
        }
        public override string getName()
        {
            return "createmap";
        }
        public override string[] getAliases() { return new string[] { "show", "hide", "showconsole", "hideconsole", "showgui", "hidegui", "closeconsole", "closegui", "openconsole", "opengui", "close", "open" }; }
        public override ParamType[][] getParamTypes()
        {
            return new ParamType[][]{
                new ParamType[]{},
                new ParamType[]{ParamType.STRING_WITHOUT_SPACES},
            };
        }
        public override int getDefaultPermissionLevel()
        {
            return 500;
        }
        private static bool isValidDimension(int dim)
        {
            return dim == 16 || dim == 32 || dim == 64 || dim == 128 || dim == 256 || dim == 512 || dim == 1024;
        }
        public override void execute(int overload, CommandExecutor executor, CommandArgument[] arguments, string alias)
        {
            bool shouldShowGui = false, shouldHideGui = false;
            bool shouldShowConsole = false, shouldHideConsole = false;
            if (alias == "show" || alias == "open")
            {
                if (arguments.Length == 0 || (arguments[0].stringValue.ToLower() != "gui" && arguments[0].stringValue.ToLower() != "console"))
                {
                    executor.commandOutput("&4Please specify &bgui &4or &bconsole");
                    return;
                }
                if (arguments[0].stringValue.ToLower() == "gui") shouldShowGui = true;
                else if (arguments[0].stringValue.ToLower() == "console") shouldShowConsole = true;
            }
            else if (alias == "close" || alias == "hide")
            {
                if (arguments.Length == 0 || (arguments[0].stringValue.ToLower() != "gui" && arguments[0].stringValue.ToLower() != "console"))
                {
                    executor.commandOutput("&4Please specify &bgui &4or &bconsole");
                    return;
                }
                if (arguments[0].stringValue.ToLower() == "gui") shouldHideGui = true;
                else if (arguments[0].stringValue.ToLower() == "console") shouldHideConsole = true;
            }
            else if (alias == "showgui" || alias == "opengui") shouldShowGui = true;
            else if (alias == "hidegui" || alias == "closegui") shouldHideGui = true;
            else if (alias == "showconsole" || alias == "openconsole") shouldShowConsole = true;
            else if (alias == "hideconsole" || alias == "closeconsole") shouldHideConsole = true;
            else
            {
                executor.commandOutput("&4Invalid arguments!");
                return;
            }
            if (shouldShowConsole)
            {
                var handle = MainWindow.GetConsoleWindow();
                MainWindow.ShowWindow(handle, MainWindow.SW_SHOW);
            }
            if (shouldHideConsole)
            {
                var handle = MainWindow.GetConsoleWindow();
                MainWindow.ShowWindow(handle, MainWindow.SW_HIDE);
            }
            if (shouldShowGui)
            {
                MainWindow.show();
            }
            if (shouldHideGui)
            {
                MainWindow.hide();
            }

        }
    }
}
