using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis
{
    // Temp class 
    public class Debugger
    {
        public static DateTime now = DateTime.Now;
        public static string path;
        public static StreamWriter writer;

        static Debugger()
        {
            Debugger.path = @"./logs_" + now.ToString("dd_MM_yyyy_HH_mm_ss") + ".txt";

            if (!File.Exists(Debugger.path))
            {
                writer = File.CreateText(Debugger.path);
            }
        }

        public static void log(string message)
        {
            //Console.WriteLine(message);

            //Debugger.writer.WriteLine(message);
        }
    }
}
