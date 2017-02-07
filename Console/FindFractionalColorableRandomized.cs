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
using System.Runtime.Serialization.Formatters.Binary;

namespace Console
{
    public static class FindFractionalColorableRandomized
    {
        static readonly int MinVertices = FindFractionalColorable.MinVertices;
        static readonly int MaxVertices = FindFractionalColorable.MaxVertices;
        static readonly int MinRingSize = FindFractionalColorable.MinRingSize;
        static readonly int MaxRingSize = FindFractionalColorable.MaxRingSize;
        static readonly int C = FindFractionalColorable.C;
        static readonly int Fold = FindFractionalColorable.Fold;
        static readonly int InsideLowerLimit = FindFractionalColorable.InsideLowerLimit;
        static readonly int InsideUpperLimit = FindFractionalColorable.InsideUpperLimit;
        
        static readonly bool OnlyBigContractions = false;
        static readonly Func<int, int> GetMinContractionSize = R => OnlyBigContractions ? R / 2 : 0;

        static readonly string WinnersFile = "i forgot " + (OnlyBigContractions ? "only big " : "" ) + (InsideLowerLimit != int.MinValue ? "limit down " + InsideLowerLimit + " " : "") + (InsideUpperLimit != int.MaxValue ? "limit up " + InsideUpperLimit + " " : "") + "random " + Fold + "-fold " + C + "-coloring" + ("ring size " + MinRingSize + " -- " + MaxRingSize) + ("planar triangulation") + string.Format("winners.txt");
        public static void Go()
        {
            var RNG = new Random(DateTime.Now.Millisecond);

            File.Delete(WinnersFile);

            var allColors = Enumerable.Range(0, C).To_long();
            var w = 0;

            var assignmentLookup = FindFractionalColorable.LoadLookup();

            System.Console.Write("Loading graphs...");
            var graphs = Enumerable.Range(MinVertices, MaxVertices - MinVertices + 1).CartesianProduct(Enumerable.Range(MinRingSize, MaxRingSize - MinRingSize + 1)).SelectMany(tup =>
                {
                    var N = tup.Item1;
                    var R = tup.Item2;

                    var path = string.Format(@"C:\Users\landon\Google Drive\research\Graph6\triangulation\disk\triangulation{0}_{1}.g6.tri.weighted.txt", N, R);
                    if (!File.Exists(path))
                        return new Graph[] { };

                    return path.EnumerateWeightedGraphs().Where(gg => InsideLowerLimit <= gg.Vertices.Count(v => gg.VertexWeight[v] != 99) && gg.Vertices.Count(v => gg.VertexWeight[v] != 99) <= InsideUpperLimit);
                }).Reverse().ToList();

            System.Console.WriteLine(" found " + graphs.Count);

            System.Console.Write("Loading contractions...");
            var totalContractions = 0;
            var contractionsLookup = new Dictionary<int, List<List<ulong>>>();
            for (var R = MinRingSize; R <= MaxRingSize; R++)
            {
                var minHave = GetMinContractionSize(R);
                var partitionFile = @"C:\Users\landon\Google Drive\research\Graph6\noncrossing\non_crossing" + R + ".txt";
                if (File.Exists(partitionFile))
                {
                    var vv = NonCrossing.Load(partitionFile).ToList();
                    var possibleAssociations = NonCrossing.Load(partitionFile).Where(ll => ll.Any(l => l.Count >= minHave)).ToList();
                    totalContractions += possibleAssociations.Count();
                    contractionsLookup[R] = possibleAssociations.Select(ll => ll.Where(l => l.Count >= 2).Select(l => l.ToUInt64()).ToList()).Where(ll => ll.Count >= 1).ToList();
                }
            }

            System.Console.WriteLine(" found " + totalContractions);

            while (graphs.Count > 0)
            {
                System.Console.Write("selecting contractions... ");
                var count = graphs.Count;

                var lookupCount = assignmentLookup.Count;
                var randomContractionAssignments = new Dictionary<int, List<List<long>>>();
                for (var R = MinRingSize; R <= MaxRingSize; R++)
                {
                    List<List<ulong>> contractions;
                    if (contractionsLookup.TryGetValue(R, out contractions))
                    {
                        if (contractions.Count <= 0)
                            continue;

                        var q = RNG.Next(contractions.Count);
                        var contraction = contractions[q];
                        contractions.RemoveAt(q);

                        var key = new FindFractionalColorable.Key(R, contraction);
                        List<List<long>> assignments;
                        if (!assignmentLookup.TryGetValue(key, out assignments))
                        {
                            assignments = RingAssignmentGenerator.GenerateAssignments(R, C, Fold, contraction);
                            if (!FindFractionalColorable.SkipLookup)
                                assignmentLookup[key] = assignments;
                        }

                        randomContractionAssignments[R] = assignments;
                    }

                    System.Console.Write(R + ",");
                }

                if (randomContractionAssignments.Count <= 0)
                    break;

                if (assignmentLookup.Count > lookupCount)
                    FindFractionalColorable.SaveLookup(assignmentLookup);

                System.Console.WriteLine();
                System.Console.Write("checking reducibility...");
                for (int i = graphs.Count - 1; i >= 0; i--)
                {
                    var g = graphs[i];

                    var ring = g.Vertices.Where(v => g.VertexWeight[v] == 99).ToList();
                    if (!randomContractionAssignments.ContainsKey(ring.Count))
                        continue;

                    var inside = g.Vertices.Except(ring).ToList();
                    var cyclicRingOrdering = FindFractionalColorable.GetCyclicOrdering(g, ring);

                    var assignments = randomContractionAssignments[ring.Count];

                    var good = true;
                    foreach (var stacks in assignments)
                    {
                        var insideAssignment = Enumerable.Repeat<long>(0, g.N).ToList();
                        foreach (var v in inside)
                        {
                            var neighborColors = g.Neighbors[v].Intersect(ring).Aggregate(0L, (tot, z) => tot | stacks[cyclicRingOrdering.IndexOf(z)]);
                            insideAssignment[v] = allColors & ~neighborColors;
                        }

                        if (!g.IsFoldChoosable(insideAssignment, inside, Fold))
                        {
                            good = false;
                            break;
                        }
                    }

                    if (good)
                    {
                        var gh = g.Clone();
                        gh.VertexWeight = g.VertexWeight.Select(ww => ww == 99 ? 0 : ww).ToList();
                        gh.AppendWeightStringToFile(WinnersFile);

                        w++;
                        graphs.RemoveAt(i);
                    }
                }

                var reduced = count - graphs.Count;
                System.Console.WriteLine(" found " + reduced);
            }
        }
    }
}

