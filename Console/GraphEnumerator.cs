﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BitLevelGeneration;
using Choosability;

namespace Console
{
    public class GraphEnumerator : IDisposable
    {
        const string Graph6FileRoot = @"C:\Graph6\graph";

        StreamWriter Writer { get; set; }
        string WinnersFile { get; set; }
        string LastTriedFile { get { return "last tried " + WinnersFile; } }
        int MinVertices { get; set; }
        int MaxVertices { get; set; }
        List<Graph> PreviousWinners { get; set; }
        Graph Last { get; set; }

        public GraphEnumerator(string winnersFile, int minVertices, int maxVertices)
        {
            WinnersFile = winnersFile;
            MinVertices = minVertices;
            MaxVertices = maxVertices;

            Initialize();
        }

        void Initialize()
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("Loading previous winners...");
            PreviousWinners = LoadPreviousWinners();
            System.Console.WriteLine("Found " + PreviousWinners.Count + " previous winners.");

            Writer = new StreamWriter(WinnersFile);

            foreach (var g in PreviousWinners)
            {
                WriteGraph(Writer, g);
                Last = g;
            }

            if (File.Exists(LastTriedFile))
            {
                using (var sr = new StreamReader(LastTriedFile))
                {
                    var ew = sr.ReadToEnd().GetEdgeWeights();
                    Last = new Graph(ew);
                    System.Console.WriteLine("last tried graph: " + ew.ToGraph6());
                }
            }
            else if (Last != null)
                System.Console.WriteLine("last tried graph: " + Last.GetEdgeWeights().ToGraph6());
        }

        List<Graph> LoadPreviousWinners()
        {
            var winners = new List<Graph>();
            try
            {
                try
                {
                    File.Copy(WinnersFile, "previous " + WinnersFile);
                }
                catch { }

                using (var sr = new StreamReader("previous " + WinnersFile))
                {
                    while (true)
                    {
                        var line = sr.ReadLine();
                        if (line == null)
                            break;

                        var parts = line.Split(' ');
                        var edgeWeights = parts.Where(p => !p.StartsWith("[")).Select(x => int.Parse(x)).ToList();

                        List<int> vertexWeights = null;
                        var vwp = parts.FirstOrDefault(p => p.StartsWith("["));
                        if (vwp != null)
                            vertexWeights = vwp.Trim('[').Trim(']').Split(',').Select(x => int.Parse(x)).ToList();

                        var g = new Graph(edgeWeights, vertexWeights);
                        winners.Add(g);
                    }
                }
            }
            catch { }

            return winners;
        }

        public void AddWinner(Graph g, Graph output = null)
        {
            PreviousWinners.Add(g);
            output = output ?? g;
            WriteGraph(Writer, output);
        }

        public IEnumerable<Graph> EnumerateGraph6File(Func<Graph, IEnumerable<Graph>> secondaryEnumerator = null)
        {
            var min = MinVertices;
            string lastGraph6 = null;
            var foundPreviousLast = true;
            if (Last != null)
            {
                min = Last.N;
                lastGraph6 = Last.GetEdgeWeights().ToGraph6();
                foundPreviousLast = false;
            }

            for (int N = min; N <= MaxVertices; N++)
            {
                System.Console.WriteLine("Checking " + N + " vertex graphs...");
                var file = Graph6FileRoot + N + ".txt";
                if (!File.Exists(file))
                {
                    System.Console.WriteLine(file + " does not exist, skipping.");
                    continue;
                }

                using (var sr = new StreamReader(file))
                {
                    while (true)
                    {
                        var line = sr.ReadLine();
                        if (line == null)
                            goto next;

                        if (!foundPreviousLast)
                        {
                            if (line.StartsWith(lastGraph6))
                                foundPreviousLast = true;
                            continue;
                        }

                        var ew = line.GetEdgeWeights();
                        var g = new Graph(ew);
                        if (secondaryEnumerator != null)
                        {
                            foreach (var gg in secondaryEnumerator(g))
                            {
                                if (PreviousWinners.All(h => !gg.ContainsInducedWithoutLargerWeight(h)))
                                    yield return gg;
                                else
                                    System.Console.WriteLine("skipping supergraph " + gg.ToGraph6());
                            }
                        }
                        else
                        {
                            if (PreviousWinners.All(h => !g.ContainsInduced(h)))
                                yield return g;
                            else
                                System.Console.WriteLine("skipping supergraph " + g.ToGraph6());
                        }
                        

                        using (var sw = new StreamWriter(LastTriedFile))
                            sw.WriteLine(ew.ToGraph6());
                    }
                }

            next: ;
            }
        }

        static void WriteGraph(StreamWriter sw, Graph g)
        {
            var edgeWeights = string.Join(" ", g.GetEdgeWeights().Select(x => x.ToString()));
            var vertexWeights = "";
            if (g.VertexWeight != null)
                vertexWeights = " [" + string.Join(",", g.VertexWeight) + "]";

            sw.WriteLine(edgeWeights + vertexWeights);
            sw.Flush();
        }

        public void Dispose()
        {
            Writer.Dispose();
        }
    }
}