using BitLevelGeneration;
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
            var gg = new BitGraph_long(g.GetEdgeWeights());

            List<List<int>> bad;
            var choosable = gg.IsFGChoosable((x) => 4, (x) => 2, out bad, x => System.Console.WriteLine("finished " + x));



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

    }
}
