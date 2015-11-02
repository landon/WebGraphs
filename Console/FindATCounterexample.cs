using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitLevelGeneration;
using Choosability;
using Choosability.Polynomials;
using Choosability.Utility;
using System.Threading;

namespace Console
{
    static class FindATCounterexample
    {
        public static void Go()
        {
            var output = "AT_CE_test.txt";
            File.Delete(output);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            for (int N = 2; N <= 16; N++)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                System.Console.WriteLine("Checking " + N + " vertex graphs...");
                System.Console.ForegroundColor = ConsoleColor.White;

                for (var R = 4; R <= 16; R++)
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkGray;
                    System.Console.WriteLine("Checking ring size " + R + "...");
                    System.Console.ForegroundColor = ConsoleColor.White;

                    var path = string.Format(@"C:\Users\landon\Google Drive\research\Graph6\triangulation\disk\triangulation{0}_{1}.g6", N, R);
                    if (!File.Exists(path))
                        continue;

                    foreach (var g in path.EnumerateGraph6())
                    {
                        var result = g.HasFOrientationSkipPaint(v => 4);
                        if (result == null)
                        {
                            g.AppendWeightStringToFile(output);
                            System.Console.ForegroundColor = ConsoleColor.Green;
                            System.Console.WriteLine(g.ToGraph6());
                            System.Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {

                        }
                    }
                }
            }
        }
    }
}
