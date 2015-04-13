using Choosability;
using Choosability.WordGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class FindTarpits
    {
        const int Length = 5;
        
        static StreamWriter _sw;

        public static void Go()
        {
            using(_sw =  new StreamWriter("tarpits" + Length + ".txt"))
            {
                System.Console.ForegroundColor = ConsoleColor.White;

                var tarpitEnumerator = new TarpitEnumerator(Length);
                var count = 0;

                var tarpitCores = new List<List<string>>();
                foreach (var tarpit in tarpitEnumerator.EnumerateMinimalTarpits())
                {
                    var isClosed = tarpitEnumerator.IsPermutationClosed(tarpit);
                    var core = tarpitEnumerator.RemovePermutationRedundancies(tarpit);

                    Write("{" + string.Join(",", tarpit) + "}  :  ");    
                    Write("{" + string.Join(",", core) + "}");

                    if (!isClosed)
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        WriteLine(" (not closed)");
                    }
                    else
                        WriteLine();

                    System.Console.ForegroundColor = ConsoleColor.White;
                    WriteLine();
                    count++;

                    tarpitCores.Add(core);
                }

                Write("found " + count + " tarpits");
                WriteLine();
                WriteLine("computing transversal hypergraph...");
                WriteLine();

                var H = new Hypergraph<string>(tarpitCores);
                var transversals = H.Tr();

                foreach (var t in transversals.E)
                    WriteLine("{" + string.Join(",", t) + "}");
            }
        }

        static void Write(string s)
        {
            System.Console.Write(s);
            _sw.Write(s);
        }

        static void WriteLine(string s)
        {
            System.Console.WriteLine(s);
            _sw.WriteLine(s);
        }

        static void WriteLine()
        {
            System.Console.WriteLine();
            _sw.WriteLine();
        }
    }
}
