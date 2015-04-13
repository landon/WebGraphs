using Choosability;
using Choosability.WordGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class FindTarpits
    {
        const int Length = 4;
        const bool RemovePermutationRedundancy = true;

        public static void Go()
        {
            System.Console.ForegroundColor = ConsoleColor.White;

            var tarpitEnumerator = new TarpitEnumerator(Length);
            var count = 0;

            var tarpitCores = new List<List<string>>();
            foreach (var tarpit in tarpitEnumerator.EnumerateMinimalTarpits())
            {
                var isClosed = tarpitEnumerator.IsPermutationClosed(tarpit);
                var core = tarpitEnumerator.RemovePermutationRedundancies(tarpit);

                if (RemovePermutationRedundancy)
                    System.Console.Write("{" + string.Join(",", core) + "}");
                else
                    System.Console.Write("{" + string.Join(",", tarpit) + "}");

                if (!isClosed)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine(" (not closed)");
                }
                else
                    System.Console.WriteLine();

                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine();
                count++;

                tarpitCores.Add(core);
            }

            System.Console.Write("found " + count + " tarpits");
            System.Console.WriteLine();
            System.Console.WriteLine("computing transversal hypergraph...");
            System.Console.WriteLine();

            var H = new Hypergraph<string>(tarpitCores);
            var transversals = H.Tr();

            foreach (var t in transversals.E)
            {
                System.Console.WriteLine("{" + string.Join(",", t) + "}");
            }
        }
    }
}
