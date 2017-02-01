using Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Choosability;
namespace Console
{
    class Program
    {
        static Program()
        {
            System.Console.ForegroundColor = ConsoleColor.White;
        }

        static void Main(string[] args)
        {
            UnitDistance.Go();
          //  DoGallaiTrees.Go();
          // DoGallaiTrees.Go().RemoveSelfIsomorphs().ToWebPageSimple(@"gallai\test\");
          //FindFractionalColorableRandomized.Go();
          //  FindKP.Go();
          //  MixedChoosables.Go();
          //for (int i = 2; i <= 2; i++)
          //{
          //    var ll = @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\mostlow\Mixed spread 2 offline winners1.txt".EnumerateWeightedGraphs()
          //        .Where(g => g.VertexWeight.Count(w => w == 1) == i)
          //        .Where(g => g.Vertices.All(v => g.Degree(v) <= 3 + g.VertexWeight[v]))
          //        .Where(g => g.IsConnected(g.Vertices.Where(v => g.VertexWeight[v] == 0).ToList()))
          //        .ToList();

            //    ll.WriteToWeightFile(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\mostlow\filtered mixed spread 2 offline winners1_" + i + ".txt");
            //    (@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\mostlow\filtered mixed spread 2 offline winners1_" + i + ".txt").EnumerateWeightedGraphs().ToWebPage(@"mostlow\list\" + i + @"\connectedlow\", lowPlus: true, directed: false, useLaplacian: false, compressName:false);
            //    System.Console.WriteLine(ll.Count);
            //}
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\5 potentialX at most 24.txt".EnumerateWeightedGraphs().Select(g => { g.VertexWeight = null; return g;}).ToWebPage(@"potential\5\24\X\", false, false, false, false);
            //  Potential.Go();
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\9 vertex Mixed spread 2 kappa2 AT max degree 4_winners1.txt"
            //    .EnumerateWeightedGraphs()
            //    .Where(g => g.InducedSubgraph(g.Vertices.Where(v => g.VertexWeight[v] == 0).ToList()).MinDegree >= 2)
            //    .WriteToWeightFile("tttest.txt");
            // MixedChoosables.Go();
            //var P4 = Choosability.Graphs.P(4);
            //P4.VertexWeight = new List<int>() { 5, 5, 5, 5 };

            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\test.txt"
            //    .EnumerateWeightedGraphs(removeOrientation: true, weightAdjustment: 5)
            //    .Where(g => g.VertexWeight.Count(w => w == 5) == 4 && g.Contains(P4, true, IsomorphRemover.WeightConditionEqual))
            //    .ToWebPage("test");

            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\3_13.txt".EnumerateWeightedGraphs().GetMinimals(WeightConditionTest).WriteToWeightFile("3_13.txt.cleaned.txt");



            //MakePictures.Go();
            //NonCrossing.MakePicture(7);
            //  NonCrossing.Generate();
            //Folkman.Go();
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\dcharge_test_7_5_7_2_1.txt.cleaned.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Count(w => w == 7) == 1).WriteToWeightFile("dcharge_test_7_5_7_2_1.txt.cleaned.txt.cut.txt");
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\dcharge_test_7_5_7_2_2.txt.cleaned.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Count(w => w == 7) == 2).WriteToWeightFile("dcharge_test_7_5_7_2_2.txt.cleaned.txt.cut.txt");
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\dcharge_test_5_5_7_2_2.txt.cleaned.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Count(w => w == 7) == 2).WriteToWeightFile("dcharge_test_5_5_7_2_2.txt.cleaned.txt.cut.txt");
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\dcharge_test_5_5_7_2_3.txt.cleaned.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Count(w => w == 7) == 3).WriteToWeightFile("dcharge_test_5_5_7_2_3.txt.cleaned.txt.cut.txt");
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Debug\dcharge_test_5_5_7_2_4.txt.cleaned.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Count(w => w == 7) == 4).WriteToWeightFile("dcharge_test_5_5_7_2_4.txt.cleaned.txt.cut.txt");
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\dcharge_test_6_5_7_2_1.txt.cleaned.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Count(w => w == 7) == 1).WriteToWeightFile("dcharge_test_6_5_7_2_1.txt.cleaned.txt.cut.txt");
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\dcharge_test_6_5_7_2_2.txt.cleaned.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Count(w => w == 7) == 2).WriteToWeightFile("dcharge_test_6_5_7_2_2.txt.cleaned.txt.cut.txt");
            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\dcharge_test_6_5_7_2_3.txt.cleaned.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Count(w => w == 7) == 3).WriteToWeightFile("dcharge_test_6_5_7_2_3.txt.cleaned.txt.cut.txt");
            //   @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Debug\dcharge_test_5_5_7_2_4.txt.cleaned.txt".EnumerateWeightedGraphs().OrderBy(g => g.VertexWeight.Count(w => w == 2)).WriteToWeightFile("dcharge_test_5_5_7_2_4.txt.cleaned.txt.ordered.txt");
            //   System.Console.WriteLine("making pictures");
            //   @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\dcharge_test_6_5_7_2_3.txt".EnumerateWeightedGraphs().RemoveSelfIsomorphs(true, IsomorphRemover.PriorityUp, true).WriteToWeightFile("dcharge_test_6_5_7_2_3.txt.cleaned.txt");
            //for (int i = 0; i < 20; i++)
            //{
            //    System.Console.WriteLine("doing " + i + "...");
            //    Discharging.BuildNeighborhoods(5, 5, 7, 3, i);
            //}
            //FindFractionalPaintable.Go();

            // @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\test.txt".EnumerateWeightedGraphs(removeOrientation: true, weightAdjustment: 5).WriteToWeightFile("test_adjusted.txt");
            //var excluded = @"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\test.txt".EnumerateWeightedGraphs(removeOrientation: true, weightAdjustment: 5).Where(g => g.VertexWeight.Max() <= 7).ToList();
            //Discharging.BuildNeighborhoods(5, 5, 7, 2, excluded).WriteToWeightFile("dcharge_test5572.txt");

            //Discharging.BuildNeighborhoods(5, 5, 6, 2, new List<Choosability.Graph>()).Select(g => 
            //{
            //    g.VertexWeight = g.Vertices.ToList();
            //    return g;
            //}).WriteToWeightFile("ordered_test.txt");

            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\test.txt".EnumerateWeightedGraphs().Select(g =>
            //{
            //    foreach (var w in g.Vertices.Where(v => g.VertexWeight[v] < g.InDegree(v) - 1 && g.EdgesOn(g.Neighbors[v]) != g.Degree(v)))
            //        g.VertexWeight[w] = g.InDegree(w) - 1;
            //    return g;
            //}).WriteToWeightFile("test_modded.txt");

            //@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\OneMore\test.txt".EnumerateWeightedGraphs().Where(g => g.VertexWeight.Sum() + g.N != g.E).Select(g =>
            //{
            //    var modified = false;
            //    foreach (var w in g.Vertices.Where(v => g.VertexWeight[v] < g.InDegree(v) - 1 && g.EdgesOn(g.Neighbors[v]) != g.Degree(v)))
            //    {
            //        modified = true;
            //        g.VertexWeight[w] = g.InDegree(w) - 1;
            //    }

            //    if (modified)
            //        return g;
            //    return null;
            //}).Where(g => g != null).WriteToWeightFile("off_modded.txt");

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
            //for (int i = 16; i <= 16; i++)
            //{
            //    for (int j = 5; j <= 15; j++)
            //    {
            //        var file = string.Format(@"C:\Users\landon\Google Drive\research\code\plantri\plantri\Debug\triangulation{0}_{1}.g6", i, j);
            //        if (File.Exists(file))
            //            FilterTriangleCutsets.FilterGraph6AndMinWeight(file);
            //    }
            //}

            // SinkEliminator.Go(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\YetAnother\ring size 11 min spread 0 spread 4 planar triangulationAT winners.txt");
            //FindChoosablesAdvanced.Go();
            //  MicTester.Go();
            // GenerateWithExcludedSubgraphs.Go();
            //WeaklyFixableTester.Go();
            //  FixerBreakerTrees.Go();
            //   SuperAbundanceFinder.Go();
            //  FindFixerBreaker.Go();
            //MakePictures.Go();

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

        static bool WeightConditionTest(Graph self, Graph A, int selfV, int av)
        {
            return A.VertexWeight[av] == 0 || A.VertexWeight[av] == self.VertexWeight[selfV];
        }
    }
}
