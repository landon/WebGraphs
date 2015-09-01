using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Choosability;
using System.IO;
using Choosability.FixerBreaker.KnowledgeEngine;
using KE = Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;

namespace UnitTests
{
    [TestClass]
    public class MindTests
    {
        const string RootPath = @"..\..\TestGraphs\";

        #region P_4
        [TestMethod]
        public void P_4_good_super()
        {
            TestGraph<KE.Slim.Super.SuperSlimMind>("P_4_good.graph", 28, false, true);
        }
        [TestMethod]
        public void P_4_bad_super()
        {
            TestGraph<KE.Slim.Super.SuperSlimMind>("P_4_bad.graph", 40, false, false);
        } 
        #endregion

        #region long 3-claw
        [TestMethod]
        public void long_3_claw_very_good_super()
        {
            TestGraph<KE.Slim.Super.SuperSlimMind>("long_3_claw_very_good.graph", 2336, true, false);
        }
        [TestMethod]
        public void long_3_claw_good_super()
        {
            TestGraph<KE.Slim.Super.SuperSlimMind>("long_3_claw_good.graph", 3488, false, true);
        }
        [TestMethod]
        public void long_3_claw_bad_super()
        {
            TestGraph<KE.Slim.Super.SuperSlimMind>("long_3_claw_bad.graph", 5216, false, false);
        } 
        #endregion


        void TestGraph<T>(string name, int totalPositions, bool shouldWin, bool shouldWinNearlyColorable)
            where T : IMind
        {
            var graphFile = Path.Combine(RootPath, name);
            Assert.IsTrue(File.Exists(graphFile), graphFile + " does not exist.");

            var graph = GraphUtility.LoadGraph(graphFile);
            var G = new Choosability.Graph(graph.GetEdgeWeights());

            var potSize = graph.Vertices.Max(v => v.Label.TryParseInt().Value);
            var template = new Template(G.Vertices.Select(v => potSize + G.Degree(v) - graph.Vertices[v].Label.TryParseInt().Value).ToList());

            var mind = (SuperSlimMind)Activator.CreateInstance(typeof(T), G);
            mind.MaxPot = potSize;

            var win = mind.Analyze(template, null);

            Assert.AreEqual(totalPositions, mind.TotalBoards, "total positions fail");
            Assert.AreEqual(shouldWin, win, "outright win fail");

            if (!win)
            {
                mind = (SuperSlimMind)Activator.CreateInstance(typeof(T), G);
                mind.MaxPot = potSize;
                mind.OnlyConsiderNearlyColorableBoards = true;

                win = mind.Analyze(template, null);
                Assert.AreEqual(shouldWinNearlyColorable, win, "nearly colorable fail");
            }
        }
    }
}
