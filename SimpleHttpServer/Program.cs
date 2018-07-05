using SimpleHttpServer.Log;
using SimpleHttpServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SimpleHttpServer
{
    class Program
    {
        private static HttpServer server;

        static void Main(string[] args)
        {
            Application.ApplicationExit += Application_ApplicationExit;

            server = new HttpServer(IPAddress.Loopback, 80);          
            server.StartServer();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            server.StopServer();
        }
    }
}
