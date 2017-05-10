using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace McJagg
{
    public class logger
    {
        public static void log(string llog, bool debug=false) {
            DateTime xtime = (DateTime.Now);
            string logStr = "[" + xtime + "] " + llog;
            Console.WriteLine(logStr);//rebase
            try {
                FileStream io = File.Open("log.txt", FileMode.Append);
                io.Write(Encoding.ASCII.GetBytes(logStr + "\r\n"), 0, logStr.Length + 2);
                io.Close();
            }
            catch { }
            if (Config.server.showGui)
            {
                if (debug) MainWindow.windowDebugLog(logStr);
                else MainWindow.windowLog(logStr);
            }
        }

        public static void debugLog(string dlog) {
            log("Debug: " + dlog, true);
        }
        public static void errorLog(string elog) {
            log("Error: " + elog);
        }
    }
}
