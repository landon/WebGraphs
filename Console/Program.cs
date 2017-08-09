using Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static int Attempt = 1;
        static int EdgesSeen = -1;
        static Choosability.Utility.Permutation CurrentP = null;
        static void Main(string[] args)
        {
             //var g = GraphsCore.GraphIO.GraphFromGraph6("GliC?K");
             //var g = GraphsCore.GraphIO.GraphFromGraph6("I]G_W_O?W");
             //var g = GraphsCore.GraphIO.GraphFromGraph6("GhEKOK");
            var g = GraphsCore.GraphIO.GraphFromGraph6("GlgO_K");
            var gg = new Choosability.Graph(g.GetEdgeWeights());

            var minNodes = long.MaxValue;

            var list = new List<int>();
            var maxEdges = 1;
            for(int i = 0; i <= maxEdges; i++)
            {
                list.Add(i);
            }
            
            foreach (var p in Choosability.Utility.Permutation.EnumerateAll(list.Count))
            {
                CurrentP = p;
                var isit = gg.IsOnlineFGChoosableListerRestricted((x) => 4, (x) => 2, Comparison);
                if (isit)
                {
                    System.Console.WriteLine("it is");
                    System.Console.ReadKey();
                }
                if (Choosability.Graph.NodesVisited < minNodes)
                {
                    minNodes = Choosability.Graph.NodesVisited;
                    
                }
                System.Console.WriteLine(minNodes + "");
                Attempt++;
            }


           // WeaklyFixableTester.Go();
           //  FixerBreakerTrees.Go();
           //   SuperAbundanceFinder.Go();
           //  FindFixerBreaker.Go();
            //MakePictures.Go();

           // MixedChoosables.Go();
            //EliminiteDoubleEdgeSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\Mixed spread 2 AT winners1.txt");
           
           //  FindTarpits.Go();
           // FindFixerBreaker.Go();
            //FindChoosables.Go();

         //   EliminateDoubleEdgeNotSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\OneHigh\not 10 vertex Mixed spread 2 max high 1 kappa2 winners1.txt");

            //ConAndNon.Go();
            System.Console.WriteLine();
            System.Console.WriteLine("done.");
            System.Console.ReadKey();
        }

        static int Comparison(Choosability.Graph G, Choosability.Graph H)
        {
            var h1 = H.E;
            var h2 = G.E;
            if (h1 > EdgesSeen)
                EdgesSeen = h1;
            if (h2 > EdgesSeen)
                EdgesSeen = h2;
            var c = CurrentP[h1].CompareTo(CurrentP[h2]);
            if (c != 0)
                return c;
        
            return 0;
        }
    }
}
