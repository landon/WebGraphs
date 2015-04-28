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
            MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Release\winners.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\BorodinKostochka\Online", directed: false, showFactors: true);
        }

        static void MakeWebpage(string graphPath, string outputPath, bool directed = false, bool showFactors = false)
        {
            var maker = new GraphPictureMaker(graphPath);
            maker.Directed = directed;
            maker.ShowFactors = true;
            maker.DrawAllAndMakeWebpage(outputPath);
        }
    }
}
