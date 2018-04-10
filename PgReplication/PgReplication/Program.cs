using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PgReplication.command;
using System.Diagnostics;

namespace PgReplication
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("1---Copy to the empty database.");
                    Console.Write("Enter parameters:");
                    var input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        Stopwatch sw = new Stopwatch();
                        Console.WriteLine("task start>");
                        sw.Start();
                        switch (input)
                        {
                            case "1":new Kernel().CopyToEmptyDataBase();break;
                            default:
                                break;
                        }
                        sw.Stop();
                        Console.WriteLine($"<task end,spend time:{sw.ElapsedMilliseconds}ms");
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
