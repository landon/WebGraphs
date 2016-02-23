using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Console
{
    public static class Potential
    {
        const int K = 5;
        const int Min = K;
        const int Max = 8;

        public static void Go()
        {
            var t1 = new Thread(() =>
                {
                    var bound = (K - 2) * (K + 1);
                    DoIt(K + " potentialX at most " + bound, bound);
                });

            var t2 = new Thread(() =>
            {
                var bound = 2 * (K - 2) * (K - 1);
                DoIt(K + " potentialX at most " + bound, bound);
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();
        }

        static void DoIt(string name, int bound)
        {
            var file = name + ".txt";
            Clear(file);

            using (var graphEnumerator = new GraphEnumerator(file, Min, Max, true))
            {
                graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";
                graphEnumerator.WeightCondition = (_, __, ___, ____) => true;
                graphEnumerator.OnlyExcludeBySpanningSubgraphs = false;

                foreach (var g in graphEnumerator.EnumerateGraph6File(null, Secondary, false))
                {
                    if (PotentialAtMost(g, bound))
                    {
                        graphEnumerator.AddWinner(g);
                        System.Console.WriteLine(bound + " winner : " + g.ToGraph6());
                    }
                }
            }
        }

        static void Clear(string file)
        {
            File.Delete(file);
            File.Delete("previous " + file);
        }

        static IEnumerable<Choosability.Graph> Secondary(Choosability.Graph g)
        {
            yield return g;
        }

        static bool PotentialAtMost(Choosability.Graph g, int bound)
        {
            return P(g, K) <= bound;
        }

        static int P(Choosability.Graph g, int k)
        {
            return (k - 2) * (k + 1) * g.N - 2 * (k - 1) * g.E;
        }
    }
}
