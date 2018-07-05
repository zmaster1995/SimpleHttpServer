using SimpleHttpServer.Log;
using SimpleHttpServer.Server.Request;
using SimpleHttpServer.Server.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleHttpServer.Server.Thread
{
    public class ServerThread
    {
        private Socket client;
        private byte[] buffer;

        public ServerThread(Socket client)
        {
            this.client = client;
            buffer = new byte[1024];
        }

        public void Start()
        {
            new System.Threading.Thread(new ThreadStart(DoWork)).Start();
        }

        private void DoWork()
        {
            try
            {
                SimplifiedClientRequest request = null;

                do
                {
                    string requestString = ReadRequest();

                    if (!String.IsNullOrEmpty(requestString))
                    {
                        request = SimplifiedClientRequest.ParseRequest(requestString);

                        SimplifiedServerResponse response = GenerateResponse(request);
                        WriteResponse(response);
                        
                        Logger.WriteMessage(String.Format("{0} {1} - Status: {2}", request.RequestType, request.Url, response.ResponseStatus), LogType.SUCCESS);
                    }
                    else
                    {
                        request = null;
                    }
                }
                while (request!=null && request.Connection == ConnectionType.KEEP_ALIVE);
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex);
            }
            finally
            {
                client.Close();
            }
        }

        private void WriteResponse(SimplifiedServerResponse response)
        {
            byte[] responseBytes = Encoding.ASCII.GetBytes(response.ToString());

            client.Send(responseBytes);

            if(response.Body!=null)
            {
                client.Send(response.Body);
            }
        }

        private SimplifiedServerResponse GenerateResponse(SimplifiedClientRequest request)
        {
            switch (request.RequestType)
            {
                case ClientRequestType.GET:
                    return GenerateGetResponse(request);
                case ClientRequestType.POST:
                    return GeneratePostResponse(request);
                case ClientRequestType.PUT:
                    return GeneratePutResponse(request);
                case ClientRequestType.HEAD:
                    return GenerateHeadResponse(request);
                case ClientRequestType.DELETE:
                    return GenerateDeleteResponse(request);
                case ClientRequestType.CONNECT:
                case ClientRequestType.OPTIONS:
                case ClientRequestType.PATCH:
                case ClientRequestType.TRACE:
                    return GenerateNotImplementedResponse(request);
                default:
                    throw new Exception("Unknown request type");
            }
        }

        private SimplifiedServerResponse GenerateNotImplementedResponse(SimplifiedClientRequest request)
        {
            SimplifiedServerResponse response = GenerateHeadResponse(request);

            response.ResponseStatus = 501;

            return response;
        }

        private SimplifiedServerResponse GeneratePostResponse(SimplifiedClientRequest request)
        {
            SimplifiedServerResponse response = GenerateHeadResponse(request);

            if (ServerFileHelper.FileExists(request.Url))
            {
                response.Body = ServerFileHelper.GetFileContent(request.Url);
            }

            return response;
        }

        private SimplifiedServerResponse GeneratePutResponse(SimplifiedClientRequest request)
        {
            SimplifiedServerResponse response = new SimplifiedServerResponse()
            {
                HttpVersion = request.HttpVersion,
                ContentLocation = request.Url,
                Connection = request.Connection,
                Cookies = request.Cookies
            };

            if (ServerFileHelper.FileExists(request.Url))
            {
                response.ResponseStatus = 204;
            }
            else
            {
                response.ResponseStatus = 201;
                ServerFileHelper.WriteFileContent(request.Url, request.MessageBody);
            }

            return response;
        }

        private SimplifiedServerResponse GenerateDeleteResponse(SimplifiedClientRequest request)
        {
            SimplifiedServerResponse response = GenerateHeadResponse(request);

            if (ServerFileHelper.FileExists(request.Url))
            {
                response.ResponseStatus = 202;
                ServerFileHelper.DeleteFile(request.Url);
            }

            return response;
        }

        private SimplifiedServerResponse GenerateGetResponse(SimplifiedClientRequest request)
        {
            SimplifiedServerResponse response = GenerateHeadResponse(request);

            if(ServerFileHelper.FileExists(request.Url))
            {
                response.Body = ServerFileHelper.GetFileContent(request.Url);
            }

            return response;
        }

        private SimplifiedServerResponse GenerateHeadResponse(SimplifiedClientRequest request)
        {
            SimplifiedServerResponse response = new SimplifiedServerResponse()
            {
                HttpVersion = request.HttpVersion,
                Connection = request.Connection,
                Cookies = request.Cookies
            };

            if(ServerFileHelper.FileExists(request.Url))
            {
                response.ResponseStatus = 200;
                response.ContentType = ServerFileHelper.GetContentType(request.Url);
                response.LastModified = ServerFileHelper.GetModifyDate(request.Url);
            }
            else
            {
                response.ResponseStatus = 404;
            }

            return response;
        }

        private string ReadRequest()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                int readed;

                while(client.Available>0)
                { 
                    readed = client.Receive(buffer);
                    ms.Write(buffer, 0, readed);
                }

                return Encoding.ASCII.GetString(ms.ToArray());
            }
        }
    }
}
