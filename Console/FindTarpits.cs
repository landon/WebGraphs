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
        const int Length = 6;

        public static void Go()
        {
            System.Console.ForegroundColor = ConsoleColor.White;

            var tarpitEnumerator = new TarpitEnumerator(Length);
            var count = 0;
            foreach (var tarpit in tarpitEnumerator.EnumerateMinimalTarpits())
            {
                System.Console.ForegroundColor = ConsoleColor.White;
                var isClosed = tarpitEnumerator.IsPermutationClosed(tarpit);
               
                System.Console.Write("{" + string.Join(",", tarpit) + "}");
               
                if (isClosed)
                    System.Console.ForegroundColor = ConsoleColor.Blue;
                else
                    System.Console.ForegroundColor = ConsoleColor.Red;

                System.Console.WriteLine(isClosed ? " (closed)" : " (not closed)");
            
                System.Console.WriteLine();
                count++;
            }

            System.Console.Write("found " + count + " things");
        }
    }
}
