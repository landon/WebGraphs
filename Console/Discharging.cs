using Choosability;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    public static class Discharging
    {
        public static List<Graph> BuildNeighborhoods(int d, int dmin, int dmax, int rad, List<Graph> excluded)
        {
            return NewGenerator.GenerateWeightedNeighborhoods(d, dmin, dmax, rad, excluded);
        }
    }
}
