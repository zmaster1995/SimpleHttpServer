using SimpleHttpServer.Log;
using SimpleHttpServer.Server.Thread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer.Server
{
    class HttpServer
    {
        private IPAddress ip;
        private int port;
        private const int MAX_CLIENTS = 100;

        private Socket serverSocket;

        public HttpServer(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        public void StartServer()
        {
            try
            {
                serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                serverSocket.Bind(new IPEndPoint(ip, port));
                serverSocket.Listen(MAX_CLIENTS);

                Logger.WriteMessage(String.Format("Server started listening on {0}:{1}", ip, port), LogType.SUCCESS);

                while (true)
                {
                    try
                    {
                        Socket clientSocket = serverSocket.Accept();

                        Logger.WriteMessage("Client accepted", LogType.SUCCESS);

                        new ServerThread(clientSocket).Start();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteException(ex);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.WriteException(ex);
            }
        }

        public void StopServer()
        {
            serverSocket.Close();
        }
    }
}
