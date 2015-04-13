using Choosability;
using Choosability.WordGame;
using Choosability.WordGame.Optimized;
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
        static int Length = 5;
        static bool Fast = true;

        static StreamWriter _sw;

        public static void Go()
        {
            using (_sw = new StreamWriter("tarpits" + Length + ".txt"))
            {
                _sw.AutoFlush = true;
                System.Console.ForegroundColor = ConsoleColor.White;

                TarpitEnumerator tarpitEnumerator;
                if (Fast)
                    tarpitEnumerator = new FastTarpitEnumerator(Length);
                else
                    tarpitEnumerator = new ReferenceTarpitEnumerator(Length);

                var count = 0;

                var tarpitCores = new List<List<string>>();
                foreach (var tarpit in tarpitEnumerator.EnumerateMinimalTarpits())
                {
                    var isClosed = TarpitEnumerator.IsPermutationClosed(tarpit);
                    var core = TarpitEnumerator.RemovePermutationRedundancies(tarpit);

                    Write("{" + string.Join(",", tarpit) + "}  :  ");
                    Write("{" + string.Join(",", core) + "}");

                    if (false && !isClosed)
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
