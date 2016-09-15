using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class DoGallaiTrees
    {
        const int K = 6;
        const int MaxBlocks = 5;
        const int MaxOddCycle = 3;

        public static void Go()
        {
            var gtg = new GallaiTreeGenerator(K);

            var gallaiTrees = new List<Choosability.Graph>();
            for (int blocks = 1; blocks <= MaxBlocks; blocks++)
            {
                System.Console.WriteLine("generating " + blocks + " block Gallai trees for K=" + K + "...");
                var all = gtg.EnumerateAll(blocks, MaxOddCycle).ToList();
                System.Console.WriteLine("removing isomorphs...");
                var distinct = all.RemoveSelfIsomorphs();
                gallaiTrees.AddRange(distinct);
            }

            System.Console.WriteLine("removing isomorphs...");
            gallaiTrees = gallaiTrees.RemoveSelfIsomorphs();
            System.Console.WriteLine("generating vector graphics...");
            gallaiTrees.ToWebPageSimple("gallai\\" + K + "\\" + MaxBlocks + "\\" + MaxOddCycle + "\\", K);
        }
    }
}
