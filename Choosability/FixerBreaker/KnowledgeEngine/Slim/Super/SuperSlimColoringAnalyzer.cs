using BitLevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super
{
    public class SuperSlimColoringAnalyzer
    {
       // BitGraph _bitLineGraph;
        Graph _lineGraph;
        Func<SuperSlimBoard, int, long> _getEdgeColorList;

        public SuperSlimColoringAnalyzer(Graph lineGraph, Func<SuperSlimBoard, int, long> getEdgeColorList)
        {
            _lineGraph = lineGraph;
            _getEdgeColorList = getEdgeColorList;

          //  _bitLineGraph = new BitGraph(_lineGraph.GetEdgeWeights());
        }

        //public bool Analyze(SuperSlimBoard b)
        //{
        //    return _bitLineGraph.IsChoosable(Enumerable.Range(0, _lineGraph.N).Select(e => (uint)_getEdgeColorList(b, e)).ToArray());
        //}

        //public bool ColorableWithoutEdge(SuperSlimBoard b, int edgeIndex)
        //{
        //    return _bitLineGraph.IsChoosable(Enumerable.Range(0, _lineGraph.N).Select(e =>
        //    {
        //        if (e == edgeIndex)
        //            return ~0U;

        //        return (uint)_getEdgeColorList(b, e);
        //    }).ToArray());
        //}

        public bool Analyze(SuperSlimBoard b)
        {
            return _lineGraph.IsChoosable(Enumerable.Range(0, _lineGraph.N).Select(e => _getEdgeColorList(b, e)).ToList());
        }

        public bool ColorableWithoutEdge(SuperSlimBoard b, int edgeIndex)
        {
            return _lineGraph.IsChoosable(Enumerable.Range(0, _lineGraph.N).Select(e =>
            {
                if (e == edgeIndex)
                    return -1;

                return _getEdgeColorList(b, e);
            }).ToList());
        }
    }
}
