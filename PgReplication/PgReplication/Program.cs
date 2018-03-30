using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PgReplication.command;
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
                    Console.WriteLine("Enter parameters:");
                    var input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        switch (input)
                        {
                            case "1":new Kernel().ToString();break;
                            default:
                                break;
                        }
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
