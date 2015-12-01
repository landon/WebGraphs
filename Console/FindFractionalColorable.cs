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
    public static class FindFractionalColorable
    {
        public const int MinVertices = 5;
        public const int MaxVertices = 16;
        public const int MinRingSize = 4;
        public const int MaxRingSize = 12;
        public const int C = 4;
        public const int Fold = 1;
        public const bool SkipLookup = true;
        public const int InsideLowerLimit = 1;
        public const int InsideUpperLimit = int.MaxValue;

        static readonly string WinnersFile = Fold + "-fold " + C + "-coloring" + ("ring size " + MinRingSize + " -- " + MaxRingSize) + ("planar triangulation") + string.Format("winners.txt");
        static readonly string LookupPath = @"C:\Lookup\assignment_lookup" + C + "_" + Fold;
        public static void Go()
        {
            File.Delete(WinnersFile);

            var allColors = Enumerable.Range(0, C).To_long();
            var w = 0;

            var assignmentLookup = LoadLookup();
            for (var R = MinRingSize; R <= 9; R++)
            {
                System.Console.WriteLine("generating assignments for ring size " + R + "...");

                var partitionFile = @"C:\Users\landon\Google Drive\research\Graph6\noncrossing\non_crossing" + R + ".txt";
                var possibleAssociations = NonCrossing.Load(partitionFile);
                var contractionsPile = possibleAssociations.Select(ll => ll.Where(l => l.Count >= 2).Select(l => l.ToUInt64()).ToList()).Where(ll => ll.Count > 1).ToList();
                foreach (var contraction in contractionsPile)
                {
                    var key = new Key(R, contraction);
                    List<List<long>> assignments;
                    if (!assignmentLookup.TryGetValue(key, out assignments))
                    {
                        assignments = RingAssignmentGenerator.GenerateAssignments(R, C, Fold, contraction);
                        assignmentLookup[key] = assignments;
                    }
                }
            }

            SaveLookup(assignmentLookup);
            
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

                    var partitionFile = @"C:\Users\landon\Google Drive\research\Graph6\noncrossing\non_crossing" + R + ".txt";
                    var possibleAssociations = NonCrossing.Load(partitionFile);
                    var contractionsPile = possibleAssociations.Select(ll => ll.Where(l => l.Count >= 2).Select(l => l.ToUInt64()).ToList()).Where(ll => ll.Count > 1).ToList();

                    var lookupCount = assignmentLookup.Count;
                    foreach (var g in path.EnumerateWeightedGraphs())
                    {
                        var ring = g.Vertices.Where(v => g.VertexWeight[v] == 99).ToList();
                        var inside = g.Vertices.Except(ring).ToList();
                        var cyclicRingOrdering = GetCyclicOrdering(g, ring);

                        var allGood = false;
                        foreach (var contraction in contractionsPile)
                        {
                            var key = new Key(R, contraction);
                            List<List<long>> assignments;
                            if (!assignmentLookup.TryGetValue(key, out assignments))
                            {
                                assignments = RingAssignmentGenerator.GenerateAssignments(R, C, Fold, contraction);
                                assignmentLookup[key] = assignments;
                                //if (assignmentLookup.Count - lookupCount > 50)
                                //    SaveLookup(assignmentLookup);
                            }

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
                                allGood = true;
                                break;
                            }
                        }

                        if (allGood)
                        {
                            var gh = g.Clone();
                            gh.VertexWeight = g.VertexWeight.Select(ww => ww == 99 ? 0 : ww).ToList();
                            gh.AppendWeightStringToFile(WinnersFile);

                            w++;
                            System.Console.ForegroundColor = ConsoleColor.Green;
                            System.Console.WriteLine(string.Format("found {0} reducible graph{1} so far", w, w > 1 ? "s" : ""));
                            System.Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    if (lookupCount < assignmentLookup.Count)
                        SaveLookup(assignmentLookup);
                }
            }
        }

        public static void SaveLookup(Dictionary<Key, List<List<long>>> lookup)
        {
            if (SkipLookup)
                return;

            if (File.Exists(LookupPath))
                File.Copy(LookupPath, LookupPath + ".backup", true);

            using (var fs = new FileStream(LookupPath, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write(lookup.Count);
                foreach (var kvp in lookup)
                {
                    bw.Write(kvp.Key.R);
                    bw.Write(kvp.Key.Contractions.Count);
                    foreach (var u in kvp.Key.Contractions)
                        bw.Write(u);

                    bw.Write(kvp.Value.Count);
                    foreach (var l in kvp.Value)
                    {
                        bw.Write(l.Count);
                        foreach (var u in l)
                            bw.Write(u);
                    }
                }
            }
        }

        public static Dictionary<Key, List<List<long>>> LoadLookup()
        {
            var lookup = new Dictionary<Key, List<List<long>>>();
            if (!SkipLookup)
            {
                try
                {
                    if (File.Exists(LookupPath + ".backup"))
                    {
                        if (!File.Exists(LookupPath) || new FileInfo(LookupPath).Length < new FileInfo(LookupPath + ".backup").Length)
                        {
                            System.Console.WriteLine("Backup is better, using it.");
                            File.Copy(LookupPath + ".backup", LookupPath, true);
                        }
                    }

                    System.Console.Write("Loading lookup table...");
                    using (var fs = new FileStream(LookupPath, FileMode.Open, FileAccess.Read))
                    using (var br = new BinaryReader(fs))
                    {
                        var kvpCount = br.ReadInt32();
                        for (int k = 0; k < kvpCount; k++)
                        {
                            var r = br.ReadInt32();
                            var count = br.ReadInt32();
                            var contractions = new List<ulong>(count);
                            for (int i = 0; i < count; i++)
                                contractions.Add(br.ReadUInt64());

                            var key = new Key(r, contractions);

                            var vcount = br.ReadInt32();
                            var value = new List<List<long>>(vcount);
                            for (int i = 0; i < vcount; i++)
                            {
                                var lcount = br.ReadInt32();
                                var l = new List<long>(lcount);
                                for (int j = 0; j < lcount; j++)
                                    l.Add(br.ReadInt64());
                                value.Add(l);
                            }

                            lookup[key] = value;
                        }
                    }
                }
                catch { }
            }

            System.Console.WriteLine(" found " + lookup.Count);
            return lookup;
        }

        public static List<int> GetCyclicOrdering(Graph g, List<int> ring)
        {
            var cyclic = new List<int>(ring.Count);

            var unplaced = ring.ToList();
            var last = -1;
            while (unplaced.Count > 0)
            {
                last = last < 0 ? unplaced.First() : g.Neighbors[last].Intersect(unplaced).First();
                unplaced.Remove(last);
                cyclic.Add(last);
            }

            return cyclic;
        }

        public class Key
        {
            public int R;
            public List<ulong> Contractions;
            int _hashCode;

            public Key(int r, List<ulong> contractions)
            {
                R = r;
                Contractions = contractions;

                unchecked
                {
                    _hashCode = 19;
                    foreach (var x in contractions)
                        _hashCode = (int)(_hashCode * 31 + (long)x);

                    _hashCode = (int)(_hashCode * 31 + R);
                }
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }

            public override bool Equals(object obj)
            {
                return Equals((Key)obj);
            }

            public bool Equals(Key other)
            {
                return other.R == R && other.Contractions.SequenceEqual(Contractions);
            }
        }
    }
}

