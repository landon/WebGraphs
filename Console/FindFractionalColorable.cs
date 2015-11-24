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
    public static class FindFractionalColorable
    {
        const int MinVertices = 4;
        const int MaxVertices = 16;
        const int MinRingSize = 4;
        const int MaxRingSize = 12;
        const int C = 9;
        const int Fold = 2;

        static readonly string WinnersFile = Fold + "-fold " + C + "-coloring" + ("ring size " + MinRingSize + " -- " + MaxRingSize) + ("planar triangulation") + string.Format("winners.txt");
        public static void Go()
        {
            File.Delete(WinnersFile);

            var w = 0;
            for (int N = MinVertices; N <= MaxVertices; N++)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                System.Console.WriteLine("Checking " + N + " vertex graphs...");
                System.Console.ForegroundColor = ConsoleColor.White;

                for (var R = MinRingSize; R <= MaxRingSize; R++)
                {
                    System.Console.ForegroundColor = ConsoleColor.DarkGray;
                    System.Console.WriteLine("Checking ring size " + R + "...");
                    System.Console.ForegroundColor = ConsoleColor.White;

                    var path = string.Format(@"C:\Users\landon\Google Drive\research\Graph6\triangulation\disk\triangulation{0}_{1}.g6.tri.weighted.txt", N, R);
                    if (!File.Exists(path))
                        continue;

                    foreach (var g in path.EnumerateWeightedGraphs())
                    {
                        var ring = g.Vertices.Where(v => g.VertexWeight[v] == 99).ToList();
                        var inside = g.Vertices.Except(ring).ToList();

                        //C:\Users\landon\Google Drive\research\Graph6\noncrossing

                        if (true)
                        {
                            g.AppendWeightStringToFile(WinnersFile);

                            w++;
                            System.Console.ForegroundColor = ConsoleColor.Green;
                            System.Console.WriteLine(string.Format("found {0} reducible graph{1} so far", w, w > 1 ? "s" : ""));
                            System.Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
            }
        }
    }
}

