using Choosability.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class MicTester
    {
        public static void Go()
        {
            int k = 5;
            using (var graphEnumerator = new GraphEnumerator(string.Format("mic{0}b.txt", k), k, k, false))
            {
                graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\VertexCritical\chi";

                foreach (var g in graphEnumerator.EnumerateGraph6File())
                {
                    var micG = g.Mic();
                    var diff = micG - (g.N + k - 4);

                    //System.Console.WriteLine(g.ToGraph6() + " :: " + diff);

                    if (diff < 0)
                    {
                        graphEnumerator.AddWinner(g);
                        System.Console.WriteLine(diff);
                    }
                }
            }
        }
    }
}
