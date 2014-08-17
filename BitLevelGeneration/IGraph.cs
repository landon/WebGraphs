using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitLevelGeneration
{
    public interface IGraph
    {
        int N { get; }
        IEnumerable<int> Vertices { get; }
        bool IsIndependent(uint set);
        int DegreeInSet(int v, uint set);
        int Degree(int v);
        IEnumerable<uint> MaximalIndependentSubsets(uint set);
    }
}
