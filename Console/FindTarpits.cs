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
            foreach (var tarpit in tarpitEnumerator.EnumerateMinimalTarpits())
            {
                var isClosed = tarpitEnumerator.IsPermutationClosed(tarpit);

                if (RemovePermutationRedundancy)
                    System.Console.Write("{" + string.Join(",", tarpitEnumerator.RemovePermutationRedundancies(tarpit)) + "}");
                else
                    System.Console.Write("{" + string.Join(",", tarpit) + "}");
               
                if (isClosed)
                    System.Console.ForegroundColor = ConsoleColor.Blue;
                else
                    System.Console.ForegroundColor = ConsoleColor.Red;

                System.Console.WriteLine(isClosed ? " (closed)" : " (not closed)");
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine();
                count++;
            }

            System.Console.Write("found " + count + " things");
        }
    }
}
