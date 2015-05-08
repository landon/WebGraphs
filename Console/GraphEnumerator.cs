using System;
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
        const string Graph6Root = @"C:\Graph6\";
        public static readonly string GraphFileRoot = Graph6Root + "graph";
        public static readonly string TreeFileRoot = Graph6Root + @"trees\trees";
        public static readonly string TreePlusEdgeFileRoot = Graph6Root + @"treesplusedge\treesplusedge";

        StreamWriter Writer { get; set; }
        string WinnersFile { get; set; }
        int MinVertices { get; set; }
        int MaxVertices { get; set; }
        List<Graph> PreviousWinners { get; set; }
        Graph Last { get; set; }
        public string FileRoot { get; set; }
        public Func<Graph, Graph, int, int, bool> WeightCondition = WeightConditionDown;

        public GraphEnumerator(string winnersFile, int minVertices, int maxVertices)
        {
            FileRoot = GraphFileRoot;
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

          if (Last != null)
                System.Console.WriteLine("last tried graph: " + Last.GetEdgeWeights().ToGraph6());
        }

        public static IEnumerable<Graph> EnumerateGraphFile(string path)
        {
            using (var sr = new StreamReader(path))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        break;

                    var parts = line.Split(' ');
                    var edgeWeights = parts.Where(p => !p.StartsWith("[")).Select(x => int.Parse(x)).ToList();

                    List<int> vertexWeights = null;
                    var vwp = parts.FirstOrDefault(p => p.StartsWith("["));
                    if (vwp != null)
                        vertexWeights = vwp.Trim('[').Trim(']').Split(',').Select(x => int.Parse(x)).ToList();

                    yield return new Graph(edgeWeights, vertexWeights);
                }
            }
        }

        List<Graph> LoadPreviousWinners()
        {
            try
            {
                try
                {
                    File.Copy(WinnersFile, "previous " + WinnersFile);
                }
                catch { }

                return EnumerateGraphFile("previous " + WinnersFile).ToList();
            }
            catch { }
            
            return new List<Graph>();
        }

        public void AddWinner(Graph g, Graph output = null)
        {
            PreviousWinners.Add(g);
            output = output ?? g;
            WriteGraph(Writer, output);
        }

        public IEnumerable<Graph> EnumerateGraph6File(Func<Graph, bool> filter = null, Func<Graph, IEnumerable<Graph>> secondaryEnumerator = null)
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
                var file = FileRoot + N + ".g6";
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
                        if (filter == null || filter(g))
                        {
                            if (secondaryEnumerator != null)
                            {
                                foreach (var gg in secondaryEnumerator(g))
                                {
                                    if (PreviousWinners.All(h => !gg.Contains(h, false, WeightCondition)))
                                    {
                                        yield return gg;
                                    }
                                    else
                                    {
                                        if (gg.VertexWeight != null)
                                        {
                                            System.Console.WriteLine("skipping supergraph " + gg.ToGraph6() + " with degrees [" + string.Join(",", gg.VertexWeight) + "]");
                                        }
                                        else
                                            System.Console.WriteLine("skipping supergraph " + gg.ToGraph6());
                                    }
                                }
                            }
                            else
                            {
                                if (PreviousWinners.All(h => !g.ContainsInduced(h)))
                                    yield return g;
                                else
                                    System.Console.WriteLine("skipping supergraph " + g.ToGraph6());
                            }
                        }
                    }
                }

            next: ;
            }
        }

        public static bool WeightConditionEqual(Graph self, Graph A, int selfV, int av)
        {
            return A.VertexWeight[av] == self.VertexWeight[selfV];
        }

        public static bool WeightConditionDown(Graph self, Graph A, int selfV, int av)
        {
            return A.VertexWeight[av] >= self.VertexWeight[selfV];
        }

        public static bool WeightConditionUp(Graph self, Graph A, int selfV, int av)
        {
            return A.VertexWeight[av] <= self.VertexWeight[selfV];
        }

        public static bool WeightConditionFalse(Graph self, Graph A, int selfV, int av)
        {
            return false;
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
