using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace PgReplication.command
{
    public static class Log
    {
        public static void WriteLog(string content)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = path + "log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + "\\"+DateTime.Now.ToString("yyyy-MM-dd")+".txt";
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            using (StreamWriter sw = new StreamWriter(path,true,Encoding.Default))
            {
                sw.WriteLine(content);
            }
        }
    }
}
