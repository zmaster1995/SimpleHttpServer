using SimpleHttpServer.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimpleHttpServer.Server
{
    class ServerFileHelper
    {
        private const string INITIAL_PATH = "./data";

        private static string GetPath(string url)
        {
            int lastDot = url.LastIndexOf('.');
            int lastSlash = url.LastIndexOf('/');

            if (lastDot>lastSlash)
            {
                return Path.Combine(INITIAL_PATH + url);
            }
            else
            {
                return Path.Combine(INITIAL_PATH + url, "index.html");
            }
        }

        public static bool FileExists(string url)
        {
            return File.Exists(GetPath(url));
        }

        public static DateTime GetModifyDate(string url)
        {
            return File.GetLastWriteTime(GetPath(url));
        }

        public static string GetContentType(string url)
        {
            return MimeMapping.GetMimeMapping(GetPath(url));
        }

        public static byte[] GetFileContent(string url)
        {
            return File.ReadAllBytes(GetPath(url));
        }

        public static void DeleteFile(string url)
        {
            File.Delete(GetPath(url));
        }

        public static void WriteFileContent(string url, string messageBody)
        {
            string path = GetPath(url);
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter sw = new StreamWriter(File.Create(path)))
            {
                sw.Write(messageBody);
            }
        }
    }
}
