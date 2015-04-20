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
            MakeWebpage(@"C:\Users\landon\Documents\GitHub\WebGraphs\Console\bin\Debug\trees only FixerBreaker winners Delta=4.txt", @"C:\Users\landon\Dropbox\Public\Web\GraphData\Fixable\Delta4Trees");
        }

        static void MakeWebpage(string graphPath, string outputPath)
        {
            var maker = new GraphPictureMaker(graphPath);
            maker.DrawAllAndMakeWebpage(outputPath);
        }
    }
}
