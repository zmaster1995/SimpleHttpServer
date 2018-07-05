using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer.Server.Request
{
    class SimplifiedClientRequest
    {
        public ClientRequestType RequestType { get; set; }
        public string Url { get; set; }
        public string HttpVersion { get; set; }
 
        public int ContentLength { get; set; }
        public ConnectionType Connection { get; set; }
        public string[] Cookies { get; set; }

        public string MessageBody { get; set; }

        public static SimplifiedClientRequest ParseRequest(string requestString)
        {
            SimplifiedClientRequest request = new SimplifiedClientRequest();

            string[] requestLines = requestString.Split('\n');

            ParseRequestLine(requestLines[0], request);

            int i;
            for (i=1; i < requestLines.Length; i++)
            {
                if(!String.IsNullOrEmpty(requestLines[i]))
                {
                    ParseHeaderLine(requestLines[i], request);
                }
                else
                {
                    break;
                }
            }

            if(request.ContentLength>0)
            {
                StringBuilder sb = new StringBuilder();

                for (; i < requestLines.Length; i++)
                {
                    sb.AppendLine(requestLines[i]);
                }

                request.MessageBody = sb.ToString();
            }

            return request;
        }

        private static void ParseHeaderLine(string line, SimplifiedClientRequest request)
        {
            string[] parts = line.Split(':').Select(x=>x.Trim()).ToArray();

            switch(parts[0].ToUpper())
            {
                case "CONTENT-LENGHT":
                    request.ContentLength = Int32.Parse(parts[1]);
                    break;
                case "CONNECTION":
                    request.Connection = ParseRequestConnectionType(parts[1]);
                    break;
                case "COOKIE":
                    request.Cookies = parts[1].Split(',').Select(x => x.Trim()).ToArray();
                    break;
            }
        }

        private static ConnectionType ParseRequestConnectionType(string value)
        {
            return value.ToUpper() == "KEEP-ALIVE" ? ConnectionType.KEEP_ALIVE : ConnectionType.CLOSE;
        }

        private static void ParseRequestLine(string line, SimplifiedClientRequest request)
        {
            string[] parts = line.Split();

            request.RequestType = (ClientRequestType) Enum.Parse(typeof(ClientRequestType), parts[0], true);
            request.Url = parts[1];
            request.HttpVersion = parts[2];
        } 
    }
}
