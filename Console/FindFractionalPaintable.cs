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
    public static class FindFractionalPaintable
    {
        const int MinVertices = 4;
        const int MaxVertices = 16;
        const int MinRingSize = 4;
        const int MaxRingSize = 15;
        const int MinDegree = 5;
        const int MaxDegree = 6;
        const int Fold = 2;

        static List<Graph> Winners = new List<Graph>();

        static readonly string WinnersFile = "Folded " + Fold + " " + ("ring size " + MinRingSize + " -- " + MaxRingSize) + ("degrees " + MinDegree + " -- " + MaxDegree) + ("planar triangulation") + string.Format("winners.txt");
        public static void Go()
        {
            File.Delete(WinnersFile);

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

                    foreach (var h in path.EnumerateWeightedGraphs())
                    {
                        foreach (var g in EnumerateWeightings(h))
                        {
                            if (g.IsOnlineFGChoosable(v => 5 * Fold - 1 - Fold * (g.VertexWeight[v] + 5 - g.Degree(v)), v => Fold))
                            {
                                Winners.Add(g);
                                g.AppendWeightStringToFile(WinnersFile);
                                System.Console.ForegroundColor = ConsoleColor.Green;
                                System.Console.WriteLine(string.Format("found {0} paintable graph{1} so far", Winners.Count, Winners.Count > 1 ? "s" : ""));
                                System.Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                    }
                }
            }
        }

        static IEnumerable<Graph> EnumerateWeightings(Graph g)
        {
            var space = g.Vertices.Select(v => Enumerable.Range(MinDegree - 5, Math.Min(g.VertexWeight[v], MaxDegree) - MinDegree + 1).Reverse()).CartesianProduct();
            foreach (var weighting in space)
            {
                var www = weighting.ToList();

                if (g.Vertices.Any(v => g.Degree(v) > www[v] + 5))
                    continue;

                if (g.Vertices.Any(v => www[v] >= g.Degree(v)))
                    continue;

                var gg = g.Clone();
                gg.VertexWeight = www;

                if (Winners.Any(h => gg.Contains(h, true, IsomorphRemover.WeightConditionDown)))
                    continue;

                yield return gg;
            }
        }
    }
}

