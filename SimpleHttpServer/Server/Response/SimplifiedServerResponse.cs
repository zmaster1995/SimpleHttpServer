using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer.Server.Response
{
    class SimplifiedServerResponse
    {
        public string HttpVersion { get; set; }
        public int ResponseStatus { get; set; }

        public ConnectionType Connection { get; set; }
        public DateTime Date { get; } = DateTime.Now;
        public string Server { get; } = "SimpleHttpServer";
        public DateTime LastModified { get; set; }
        public int? ContentLength
        {
            get
            {
                return Body?.Length;
            }
        }
        public string ContentType { get; set; }
        public string ContentLocation { get; set; }
        public string[] Cookies { get; set; }

        public byte[] Body { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format("{0} {1}", HttpVersion, ResponseStatus));

            sb.AppendLine(String.Format("Date: {0}", Date));
            sb.AppendLine(String.Format("Server: {0}", Server));
            sb.AppendLine(String.Format("Connection: {0}", Connection == ConnectionType.KEEP_ALIVE ? "keep-alive" : "close"));

            if (Body != null)
            {
                sb.AppendLine(String.Format("Last-Modified: {0}", LastModified));
                sb.AppendLine(String.Format("Content-Length: {0}", ContentLength));
                sb.AppendLine(String.Format("Content-Type: {0}", ContentType));
            }

            if(ContentLocation!=null)
            {
                sb.AppendLine(String.Format("Content-Location: {0}", ContentLocation));
            }

            if(Cookies!=null)
            {
                sb.AppendLine(String.Format("Cookies: {0}", String.Join(",", Cookies)));
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}
