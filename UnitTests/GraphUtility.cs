using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    static class GraphUtility
    {
        public static Graphs.Graph LoadGraph(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var s = sr.ReadToEnd();
                var g = Graphs.Graph.Deserialize(s);

                if (g != null)
                    g.ParametersDirty = true;
                return g;
            }
        }
    }
}
