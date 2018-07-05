using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer.Log
{
    class Logger
    {
        private static object lockObj = new object();

        public static void WriteMessage(string message, LogType type = LogType.DEFAULT)
        {
            lock(lockObj)
            {
                switch (type)
                {
                    case LogType.SUCCESS:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogType.ERROR:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.WriteLine(String.Format("{0} - {1}", DateTime.Now, message));
            }
        }

        public static void WriteException(Exception ex)
        {
            WriteMessage(ex.Message, LogType.ERROR);
        }
    }
}
