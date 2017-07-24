using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class MakePictures
    {
        public static void Go()
        {
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\triangle-free FixerBreaker winners Delta=3.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta3TriangleFreeSvg");
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\trees or trees plus edge only FixerBreaker winners Delta=3.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta3TreeOrTreePlusEdge");
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Debug\trees only FixerBreaker winners Delta=4.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta4Trees");
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Debug\trees only FixerBreaker winners Delta=5.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta5Trees");
            //  MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Debug\FixerBreaker winners Delta=4.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta4");
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\FixerBreaker winners Delta=5.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta5");
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Debug\trees or trees plus edge only FixerBreaker winners Delta=5.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta5TreeOrTreePlusEdge");
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\test tree plus two.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta3TreePlusTwoEdge");

            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\AT winners1.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\BorodinKostochka\AT", directed: true);
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\winners2.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\BorodinKostochka\2fold", directed: false, showFactors:true);
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\offline winners1.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\BorodinKostochka\Offline", directed: false, showFactors: true);
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\winners.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\BorodinKostochka\Online", directed: false, showFactors: true);
            // MakeWebpage(new GraphPictureMaker(GraphEnumerator.EnumerateEntireGraph6File(@"C:\Users\landon\Google Drive\research\Graph6\class2.g6").Where(g => g.MaxDegree == 3 && g.MinDegree >= 2)), @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta3class2", directed: false, showFactors: false);
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Debug\superabundance.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\superabundance\all", directed: false, showFactors: false);

            //MakePdfs(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\triangle-free near colorings FixerBreaker winners Delta=3.txt", @"C:\Users\landon\Documents\GitHub\Research\fixable\Delta3TriangleFree");
            //MakePdfs(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\superabundance.txt", @"C:\Users\landon\Documents\GitHub\Research\fixable\Superabundance\all", true);
            //MakePdfs(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\max degree 3trees only superabundance.txt", @"C:\Users\landon\Documents\GitHub\Research\fixable\Superabundance\MaxDegree3Trees", true);

            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\near colorings FixerBreaker winners Delta=4.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\NearColoring\Delta4");
            // MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\trees only near colorings FixerBreaker winners Delta=5.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\NearColoring\Delta5Trees");
            // MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\YetAnother\teeeDelta4Near.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\NearColoring\Delta4Trees");
            // MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Another\treesOrTreesPlusEdgeDelta4Near.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\NearColoring\Delta4TreeOrTreePlusEdge");
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\trees or trees plus edge only near colorings FixerBreaker winners Delta=5.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\NearColoring\Delta5TreeOrTreePlusEdge");

            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\near colorings FixerBreaker winners Delta=5.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\NearColoring\Delta5");

            //MakeWebpage(@"C:\Users\landon\Google Drive\research\graphs\WithLows\OneHigh\10 vertex Mixed spread 2 max high 1 winners1.txt.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\OneHigh\Paint", directed: false, showFactors: false, lowPlus: true);
            // MakeWebpage(@"C:\Users\landon\Google Drive\research\graphs\WithLows\OneHigh\9 vertex Mixed spread 2 max high 1 AT winners1.txt.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\OneHigh\AT", directed: true, showFactors: false, lowPlus: true);

            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\YetAnother\8 vertex Mixed spread 2 max high 2 winners1.txt.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\TwoHigh\Paint", directed: false, showFactors: false, lowPlus: true);
            //  MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\8 vertex Mixed spread 2 max high 3 winners1.txt.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\ThreeHigh\Paint", directed: false, showFactors: false, lowPlus: true);

            //   MakeWebpage(@"C:\Users\landon\Google Drive\research\graphs\WithLows\TwoHigh\8 vertex Mixed spread 2 max high 2 offline winners1.txt.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\TwoHigh\Choose", directed: false, showFactors: false, lowPlus: true);
            //     MakeWebpage(@"C:\Users\landon\Google Drive\research\graphs\WithLows\TwoHigh\8 vertex Mixed spread 2 max high 2 AT winners1.txt.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\TwoHigh\AT", directed: true, showFactors: false, lowPlus: true);

            //MakeWebpage(@"C:\Users\landon\Google Drive\research\graphs\WithLows\OneHigh\10 vertex Mixed spread 2 max high 1 kappa2 winners1.txt.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\OneHigh\Paint2Connected", directed: false, showFactors: false, lowPlus: true);

            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\not 10 vertex Mixed spread 2 max high 1 kappa2 winners1.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\OneHigh\NotPaintDegreesOK10", directed: false, showFactors: false, lowPlus: true);
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\not 8 vertex Mixed spread 2 max high 1 kappa2 winners1.txt.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\OneHigh\NotPaintDegreesOKelim", directed: false, showFactors: false, lowPlus: true);
            //MakePdfs(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\K_4.txt", @"C:\Users\landon\Documents\GitHub\Research\MixedChoosables\pictures", lowPlus:true);

            // MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\YetAnother\not 8 vertex Mixed spread 2 max high 1 kappa2 winners1.txt.not.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\OneHigh\NotPaintDegreesOK8Eliminated", directed: false, showFactors: false, lowPlus: true);
            //MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\YetAnother\not 7 vertex Mixed spread 2 max high 2 kappa2 AT winners1.txt.not.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\TwoHigh\NotATDegreesOK7Eliminated", directed: false, showFactors: false, lowPlus: true);

            //MakeWebpage(@"C:\Users\landon\Google Drive\research\graphs\WithLows\OneHigh\not 10 vertex Mixed spread 2 max high 1 kappa2 winners1.txt.not.eliminated.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\WithLows\OneHigh\NotPaintDegreesOK10-Eliminated", directed: false, showFactors: false, lowPlus: true);

            MakePdfs(new[] { Choosability.Graphs.K(1) }
                    .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph2.g6".EnumerateGraph6File())
                    .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph3.g6".EnumerateGraph6File())
                    .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph4.g6".EnumerateGraph6File())
                    .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph5.g6".EnumerateGraph6File())
                    .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph6.g6".EnumerateGraph6File())
                    .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph7.g6".EnumerateGraph6File())
                    .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph8.g6".EnumerateGraph6File())
                    .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph9.g6".EnumerateGraph6File())
                    .Concat(@"C:\Users\landon\Google Drive\research\Graph6\graph10.g6".EnumerateGraph6File()), @"C: \Users\landon\Documents\GitHub\books\colored graphs\graphs", properColor: true);

            // var gpm = new GraphPictureMaker(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\debug\diam3.txt".EnumerateGraph6File());
            //  MakeWebpage(gpm, @"C:\Users\landon\Documents\GitHub\landon.github.io\graphdata\distinguishingcoloring\vertextransitive\cubic\girth5", directed: false, showFactors: false, lowPlus: false);
        }

        public static void MakePdfs(string graphPath, string outputPath, bool superabundance = false, bool lowPlus = false, bool properColor = false)
        {
            var maker = new GraphPictureMaker(graphPath);
            maker.InDegreeTerms = superabundance;
            maker.IsLowPlus = lowPlus;
            maker.ProperColor = properColor;
            maker.DrawAll(outputPath, DotRenderType.pdf);
        }

        public static void MakePdfs(IEnumerable<Choosability.Graph> graphs, string outputPath, bool superabundance = false, bool lowPlus = false, bool properColor = false)
        {
            var maker = new GraphPictureMaker(graphs);
            maker.InDegreeTerms = superabundance;
            maker.IsLowPlus = lowPlus;
            maker.ProperColor = properColor;
            maker.DrawAll(outputPath, DotRenderType.pdf);
        }

        public static void MakeWebpage(string graphPath, string outputPath, bool directed = false, bool showFactors = false, bool lowPlus = false)
        {
            var maker = new GraphPictureMaker(graphPath);
            MakeWebpage(maker, outputPath, directed, showFactors, lowPlus);
        }

        public static void MakeWebpage(GraphPictureMaker maker, string outputPath, bool directed = false, bool showFactors = false, bool lowPlus = false)
        {
            maker.Directed = directed;
            maker.ShowFactors = showFactors;
            maker.IsLowPlus = lowPlus;
            maker.DrawAllAndMakeWebpage(outputPath);
        }
    }
}
