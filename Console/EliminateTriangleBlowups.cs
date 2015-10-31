using Choosability;
using Choosability.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    static class EliminateTriangleBlowups
    {
        public static void Go(string path)
        {
            Eliminate(path);
        }

        static void Eliminate(string path)
        {
            var eliminatedPath = path + ".triangle_eliminated.txt";
            File.Delete(eliminatedPath);

            foreach (var g in GraphEnumerator.EnumerateGraphFile(path))
            {
                var ones = g.VertexWeight.IndicesWhere(w => w == 1).ToList();

                if (ones.Count == 2 && g[ones[0], ones[1]])
                {
                    var n1 = g.Neighbors[ones[0]];
                    var n2 = g.Neighbors[ones[1]];
                    var common = n1.Intersection(n2);

                    if (common.Any(v => g.Degree(v) == 2))
                        continue;
                }

                g.AppendToFile(eliminatedPath);
            }
        }
    }
}
