using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class GenerateWithExcludedSubgraphs
    {
        const int MinN = 9;
        const int MaxN = 9;
        const bool Induced = true;
        const string ForbiddenPath = @"C:\Users\landon\Documents\GitHub\Research\Graph Data\Borodin Kostochka\offline winners1.txt";
        const string GraphFileRoot = @"C:\Users\landon\Google Drive\research\Graph6\graph";
        static readonly string OutputFile = ForbiddenPath + " " + MaxN + (Induced ? " induced" : "") + ".txt";

        public static void Go()
        {
            int total = 0;
            int skipped = 0;

            var forbidden = GraphEnumerator.EnumerateGraphFile(ForbiddenPath).ToList();

            var gn = new GraphEnumerator(OutputFile, MinN, MaxN, false);
            gn.FileRoot = GraphFileRoot;
            foreach (var g in gn.EnumerateGraph6File())
            {
                total++;
                if (forbidden.Any(h => g.Contains(h, Induced)))
                {
                    skipped++;
                    continue;
                }

                gn.AddWinner(g);
            }

            System.Console.WriteLine(100 * skipped / total + "% skipped");
        }
    }
}
