using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class Folkman
    {
        const int K = 2;

        public static void Go()
        {
            var file = "folkman" + K + ".txt";
            File.Delete(file);
            File.Delete("previous " + file);
            using (var graphEnumerator = new GraphEnumerator(file, 3, 10, true))
            {
                graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";
                graphEnumerator.WeightCondition = (_, __, ___, ____) => true;

                foreach (var g in graphEnumerator.EnumerateGraph6File(null, Secondary, false))
                {
                    if (!SatisfiesFolkman(g, K))
                    {
                        graphEnumerator.AddWinner(g);
                        System.Console.WriteLine(g.ToGraph6());
                    }
                }
            }
        }

        static IEnumerable<Choosability.Graph> Secondary(Choosability.Graph g)
        {
            yield return g;
        }

        static bool SatisfiesFolkman(Choosability.Graph g, int k)
        {
            var d = g.N - 2 * g.IndependenceNumber();

            return d <= k;
        }
    }
}
