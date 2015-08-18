using Choosability.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class ConAndNon
    {
        public static void Go()
        {
            TestPlanarCondition();
        }

        static void TestPlanarCondition()
        {
            using (var sw = new StreamWriter("concon.txt"))
            {
                for (int n = 2; n <= 10; n++)
                {
                    foreach (var g in string.Format(@"C:\Users\landon\Google Drive\research\Graph6\planar\planar_conn.{0}.g6", n).EnumerateGraph6File())
                    {
                        var d = MinEdgeDegree(g);
                        var ee = 6;

                        if (d > ee)
                            System.Console.ForegroundColor = ConsoleColor.Red;
                        else
                            System.Console.ForegroundColor = ConsoleColor.Green;

                        System.Console.WriteLine(g.ToGraph6() + "\t " + ee + "\t - \t" + d + "\t " + (ee - d));

                        if (d > ee)
                        {
                            sw.WriteLine(g.ToGraph6() + "\t " + ee + "\t - \t" + d + "\t " + (ee - d));
                            sw.Flush();
                        }
                    }
                }
            }
        }

        static int MinEdgeDegree(Choosability.Graph g)
        {
            var min = int.MaxValue;
            for (int v = 0; v < g.N; v++)
            {
                for (int w = v + 1; w < g.N; w++)
                {
                    if (g[v, w])
                    {
                        var cc = ListUtility.Union(g.Neighbors[v], g.Neighbors[w]).Count;
                        min = Math.Min(min, cc);
                    }
                }
            }

            return min;
        }
    }
}
