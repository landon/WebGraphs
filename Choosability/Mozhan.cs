using Choosability.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Choosability
{
    public class Mozhan
    {
        const int DefaultPauseTime_ms = 1000;

        public event Action<int, List<List<int>>> OnGreedyColored;
        public event Action<List<int>, List<List<int>>> OnKempeChainPulled;
        public event Action<int, int, List<List<int>>> OnLevelSwap;

        public Graph G { get; private set; }
        public List<List<int>> π { get; private set; }
        public List<int> BigClique { get; private set; }
        public bool Success { get; private set; }
        int Δ { get; set; }
        List<int> Colors { get; set; }
        Stack<int> UncoloredVertices { get; set; }
        List<List<int>> ColorPartition { get; set; }

        AutoResetEvent PauseEvent { get; set; }
        public volatile int PauseTime_ms;

        public Mozhan(Graph g)
        {
            G = g;
            Δ = g.MaxDegree;
            π = Enumerable.Range(0, Δ - 1).Select(i => new List<int>())
                                          .ToList();
            Colors = Enumerable.Range(0, Δ - 1).ToList();
            UncoloredVertices = new Stack<int>(Enumerable.Range(0, G.N));
            CreateColorPartition();

            PauseEvent = new AutoResetEvent(false);
            PauseTime_ms = int.MaxValue;
        }

        public bool TryColor()
        {
            Success = false;

            var tabooPart = -1;
            bool[] built = null;

            while (UncoloredVertices.Count > 0)
            {
                Pause();

                var v = UncoloredVertices.Pop();

                var colorNeighbors = TryGreedyColor(v);
                if (colorNeighbors == null)
                {
                    tabooPart = -1;
                    built = null;

                    if (OnGreedyColored != null)
                        OnGreedyColored(v, π);
                    continue;
                }

                int w;
                if (TryKempeChain(v, colorNeighbors, out w))
                {
                    if (w >= 0)
                        UncoloredVertices.Push(w);

                    tabooPart = -1;
                    built = null;
                    continue;
                }

                if (built == null)
                    built = new bool[G.N];

                w = DoLevelSwap(v, colorNeighbors, built, ref tabooPart);
                if (w >= 0)
                {
                    UncoloredVertices.Push(w);

                    if (OnLevelSwap != null)
                        OnLevelSwap(v, w, π);
                    continue;
                }

                BigClique = GetClubgroup(v, colorNeighbors);
                return false;
            }

            Success = true;
            return true;
        }

        public void PlayPause(bool play)
        {
            if (play)
            {
                PauseTime_ms = DefaultPauseTime_ms;
                PauseEvent.Set();
            }
            else
            {
                PauseTime_ms = int.MaxValue;
            }

        }
        public void Faster()
        {
            PauseTime_ms = Math.Max(1, PauseTime_ms / 2);
        }
        public void Slower()
        {
            PauseTime_ms = Math.Min(int.MaxValue, PauseTime_ms * 2);
        }
        public void Step()
        {
            PauseEvent.Set();
        }

        public void Pause()
        {
            if (PauseTime_ms < 0)
                return;

            if (PauseTime_ms < int.MaxValue)
                PauseEvent.WaitOne(PauseTime_ms);
            else
                PauseEvent.WaitOne();
        }

        List<int> GetClubgroup(int v, List<List<int>> colorNeighbors)
        {
            var hardPart = ColorPartition.FirstIndex(part => part.Any(c => colorNeighbors[c].Count > 1));

            var X = ColorPartition.Where((part, i) => i != hardPart)
                                  .SelectMany(part => part.SelectMany(c => colorNeighbors[c]))
                                  .Union(v.EnList())
                                  .ToList();

            if (hardPart < 0)
                return X;

            var Y = ColorPartition[hardPart].SelectMany(c => colorNeighbors[c])
                                            .ToList()
                                            .EnumerateSublists()
                                            .OrderByDescending(l => l.Count)
                                            .FirstOrDefault(B => AreSubsetsJoined(X, B));
            return X.Union(Y)
                    .ToList();
        }

        int DoLevelSwap(int v, List<List<int>> colorNeighbors, bool[] built, ref int tabooPart)
        {
            for (int i = 0; i < ColorPartition.Count; i++)
            {
                if (i == tabooPart)
                    continue;

                if (ColorPartition[i].Any(c => colorNeighbors[c].Count != 1))
                    continue;

                if (ArePartsJoined(i, tabooPart, colorNeighbors))
                    continue;

                var swapColor = ColorPartition[i].First(c => !built[colorNeighbors[c].First()]);
                var w = colorNeighbors[swapColor].First();

                π[swapColor].Remove(w);
                π[swapColor].Add(v);
                built[v] = true;
                tabooPart = i;

                return w;
            }

            return -1;
        }

        bool ArePartsJoined(int i, int j, List<List<int>> colorNeighbors)
        {
            if (i < 0 || j < 0)
                return false;

            var X = ColorPartition[i].SelectMany(c => colorNeighbors[c])
                                     .ToList();
            var Y = ColorPartition[j].SelectMany(c => colorNeighbors[c])
                                     .ToList();

            return AreSubsetsJoined(X, Y);
        }

        bool AreSubsetsJoined(List<int> X, List<int> Y)
        {
            foreach (var x in X)
                foreach (var y in Y)
                    if (!G[x, y]) return false;

            return true;
        }

        bool TryKempeChain(int v, List<List<int>> colorNeighbors, out int w)
        {
            foreach (var part in ColorPartition)
            {
                if (part.Any(c => colorNeighbors[c].Count != 1))
                    continue;

                var component = FindComponentInPart(v, part, colorNeighbors);
                var H = G.InducedSubgraph(component);

                var high = H.Vertices.Where(z => H.Degree(z) > part.Count)
                                     .ToList();
                var superLow = H.Vertices.Where(z => H.Degree(z) < part.Count)
                                         .ToList();
                if (high.Count <= 0 && superLow.Count <= 0)
                    continue;

                var vIndex = component.IndexOf(v);

                int[,] distance;
                int[,] next;
                H.FloydWarshall(out distance, out next);

                if (high.Count > 0)
                {
                    var min = high.Min(z => distance[vIndex, z]);
                    var t = high.First(z => distance[vIndex, z] == min);
                    w = component[t];

                    var path = H.FloydWarshallShortestPath(vIndex, t, distance, next)
                                .Select(q => component[q])
                                .ToList();
                    PullKempeChain(path, part, -1);
                }
                else
                {
                    var min = superLow.Min(z => distance[vIndex, z]);
                    var t = superLow.First(z => distance[vIndex, z] == min);
                    w = -1;

                    var path = H.FloydWarshallShortestPath(vIndex, t, distance, next)
                                .Select(q => component[q])
                                .ToList();
                    var finalColor = part.First(c => !π[c].Contains(component[t]) && GetColorNeighbors(component[t], c).Count <= 0);

                    PullKempeChain(path, part, finalColor);
                }

                return true;
            }

            w = -1;
            return false;
        }

        void PullKempeChain(List<int> path, List<int> part, int finalColor)
        {
            var colors = path.Select(v => FindColor(v, part))
                             .ToList();
            colors[0] = finalColor;

            for (int i = 0; i < path.Count; i++)
            {
                if (colors[i] >= 0)
                    π[colors[i]].Remove(path[i]);

                var next = colors[(i + 1) % path.Count];
                if (next >= 0)
                    π[next].Add(path[i]);
            }

            if (OnKempeChainPulled != null)
                OnKempeChainPulled(path, π);
        }

        int FindColor(int v, List<int> possibles)
        {
            foreach(var c in possibles)
            {
                if (π[c].Contains(v))
                    return c;
            }

            return -1;
        }

        List<int> FindComponentInPart(int v, List<int> part, List<List<int>> colorNeighbors)
        {
            var verticesInPart = part.SelectMany(c => π[c]).Union(new[] { v })
                                     .ToList();

            return G.InducedSubgraph(verticesInPart)
                    .FindComponents().GetEquivalenceClass(verticesInPart.IndexOf(v))
                    .Select(z => verticesInPart[z])
                    .ToList();
        }

        List<List<int>> TryGreedyColor(int v)
        {
            var colorNeighbors = new List<List<int>>(Δ - 1);

            foreach (var c in Colors)
            {
                var neighbors = GetColorNeighbors(v, c);
                if (neighbors.Count <= 0)
                {
                    π[c].Add(v);
                    return null;
                }

                colorNeighbors.Add(neighbors);
            }

            return colorNeighbors;
        }

        List<int> GetColorNeighbors(int v, int c)
        {
            return ListUtility.Intersection(π[c], G.Neighbors[v]);
        }

        void CreateColorPartition()
        {
            ColorPartition = new List<List<int>>();

            int k;
            for (k = 0; k <= Δ - 10; k += 3)
                ColorPartition.Add(Enumerable.Range(k, 3).ToList());

            var leftover = Δ - 1 - k;
            ColorPartition.Add(Enumerable.Range(k, leftover / 2).ToList());
            ColorPartition.Add(Enumerable.Range(k + leftover / 2, leftover - leftover / 2)
                          .ToList());
        }
    }
}
