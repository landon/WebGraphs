using Choosability;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class EliminiteDoubleEdgeSubdivisions
    {
        public static void Go(string path)
        {
            Eliminate(path);
        }

        static void Eliminate(string path)
        {
            var eliminatedPath = path + ".eliminated.txt";
            File.Delete(eliminatedPath);
            var P4 = Choosability.Graphs.P(4);

            foreach (var g in GraphEnumerator.EnumerateGraphFile(path))
            {
                P4.VertexWeight = new List<int>() { 0, 0, 0, 0 };
                if (g.Contains(P4, true, WeightConditionEqualAndDegreeTwoInternal))
                    continue;
                P4.VertexWeight = new List<int>() { 1, 0, 0, 0 };
                if (g.Contains(P4, true, WeightConditionEqualAndDegreeTwoInternal))
                    continue;
                P4.VertexWeight = new List<int>() { 1, 0, 0, 1 };
                if (g.Contains(P4, true, WeightConditionEqualAndDegreeTwoInternal))
                    continue;

                WriteGraph(g, eliminatedPath);
            }
        }

        static void WriteGraph(Graph g, string path)
        {
            var edgeWeights = string.Join(" ", g.GetEdgeWeights().Select(x => x.ToString()));
            var vertexWeights = "";
            if (g.VertexWeight != null)
                vertexWeights = " [" + string.Join(",", g.VertexWeight) + "]";

            using (var sw = new StreamWriter(path, append: true))
                sw.WriteLine(edgeWeights + vertexWeights);
        }

        static bool WeightConditionEqualAndDegreeTwoInternal(Graph self, Graph A, int selfV, int av)
        {
            return A.VertexWeight[av] == self.VertexWeight[selfV] && (A.Degree(av) != 2 || self.Degree(selfV) == 2);
        }

    }
}
