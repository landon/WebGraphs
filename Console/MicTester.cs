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
            using (var graphEnumerator = new GraphEnumerator("mictest.txt", 2, 10, false))
            {
                graphEnumerator.FileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";

                foreach (var g in graphEnumerator.EnumerateGraph6File())
                {
                    var micG = g.Mic();

                    System.Console.WriteLine(g.ToGraph6() + " :: ");
                    foreach (var S in g.Vertices.EnumerateSublists())
                    {
                        if (S.Count <= 0 || S.Count >= g.N)
                            continue;
                        

                        var H = g.InducedSubgraph(S);
                        var micH = H.Mic();
                        var bound = micH + g.N - S.Count;
                        
                        if (micG < bound)
                        {
                            System.Console.Write(string.Join(",", S) + " :: ");
                            System.Console.WriteLine(micG - bound);

                            if (micG < bound)
                                System.Console.ReadKey();
                        }
                    }
                }
            }
        }
    }
}
