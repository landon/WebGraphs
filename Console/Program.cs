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
        static void Main(string[] args)
        {
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\test_paint.txt".EnumerateWeightedGraphs()
            //.RemoveIsomorphs(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\test.txt".EnumerateWeightedGraphs(), true, IsomorphRemover.WeightConditionDown)
            //.WriteToWeightFile("removed_down.txt");

          //  var excluded = @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\YetAnother\goods3.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Max() <= 2).ToList();
           // var excluded = @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\YetAnother\goods3.txt".EnumerateWeightedGraphs().ToList();
            //NewGenerator.EnumerateWeightedNeighborhoods(6, 5, 7, 2).Select(g =>
            //    {
            //        var h = g.Clone();
            //        for (int i = 0; i < h.VertexWeight.Count; i++)
            //            h.VertexWeight[i] -= 5;
            //        return h;
            //    }).RemoveIsomorphs(excluded, true, IsomorphRemover.WeightConditionDown).WriteToWeightFile("6nbhdtest.txt");
          //  var min2 = @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\generated.txt".EnumerateWeightedGraphs().Min(g => g.VertexWeight.Count(x => x == 2));
         //   @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\generated.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Count(x => x == 2) <= min2 + 1).RemoveSelfIsomorphs(true, IsomorphRemover.WeightConditionUp).WriteToWeightFile("generated_min2b.txt");
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\generated.txt".EnumerateWeightedGraphs().RemoveSelfIsomorphs(true, IsomorphRemover.WeightConditionUp).WriteToWeightFile("generated_noiso.txt");
          //  NewGenerator.Go();
            //FindATCounterexample.Go();
          //  FilterTriangleCutsets.FilterGraph6AndMinWeight(@"C:\Users\landon\Google Drive\research\code\plantri\plantri\Debug\triangulation15_6.txt");
           // @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\test.txt".EnumerateWeightedGraphs().EliminateSinks().RemoveSelfIsomorphs().OrderBy(g => g.VertexWeight.Count(x => x == 0)).WriteToWeightFile("goods3.txt");
          //  @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\4--15-6--8a.txt".EnumerateWeightedGraphs().EliminateSinks().RemoveIsomorphs().WriteToWeightFile("4--15-6--8.txt");
            //for (int i = 15; i <= 16; i++)
            //{
            //    for (int j = 5; j <= 15; j++)
            //    {
            //        var file = string.Format(@"C:\Users\landon\Google Drive\research\code\plantri\plantri\Debug\triangulation{0}_{1}.g6", i, j);
            //        if (File.Exists(file))
            //            FilterTriangleCutsets.FilterGraph6AndMinWeight(file);
            //    }
            //}
            
           // SinkEliminator.Go(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\YetAnother\ring size 11 min spread 0 spread 4 planar triangulationAT winners.txt");
           FindChoosablesAdvanced.Go();
          //  MicTester.Go();
           // GenerateWithExcludedSubgraphs.Go();
            //WeaklyFixableTester.Go();
          //  FixerBreakerTrees.Go();
         //   SuperAbundanceFinder.Go();
          //  FindFixerBreaker.Go();
           // MakePictures.Go();

          //  MixedChoosables.Go();
            //EliminiteDoubleEdgeSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\Mixed spread 2 AT winners1.txt");
           
           //  FindTarpits.Go();
           // FindFixerBreaker.Go();
            //FindChoosables.Go();

         //   EliminateDoubleEdgeNotSubdivisions.Go(@"C:\Users\landon\Google Drive\research\graphs\WithLows\OneHigh\not 10 vertex Mixed spread 2 max high 1 kappa2 winners1.txt");

            //ConAndNon.Go();

           // EliminateTriangleBlowups.Go("not 9 vertex Mixed spread 2 max high 2 kappa2 winners1.txt.not.eliminated.txt");
            System.Console.WriteLine();
            System.Console.WriteLine("done.");
            System.Console.ReadKey();
        }
    }
}
