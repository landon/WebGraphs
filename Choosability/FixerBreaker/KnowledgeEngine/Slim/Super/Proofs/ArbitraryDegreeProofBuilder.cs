﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs.ArbitraryMaxDegree;
using Choosability.Utility;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class ArbitraryDegreeProofBuilder : PermutationAwareProofBuilder
    {
        public bool UseWildCards { get; set; }

        string _figureTikz;
        int _maxPot;
        List<int> _activeIndices;
        List<int> _activeListSizes;
        List<int> _possibleListIndices;
        List<List<int>> _possibleLists;
        List<SuperSlimBoard> _allBoards;
        SequenceGeneralizer<int>.VectorComparer _sequenceComparer;
        SequenceGeneralizer<int> _sequenceGeneralizer;

        public ArbitraryDegreeProofBuilder(SuperSlimMind mind, string figureTikz = "")
            : base(mind)
        {
            _figureTikz = figureTikz;
            _maxPot = mind.MaxPot;
            UseWildCards = true;
        }

        public override string WriteProof()
        {
            var sb = new StringBuilder();
       
            AddFigure(sb);
            BeginProof(sb);
            GeneratePossibleLists();
            GeneralizeAllBoards(sb);

            var wonBoards = new List<SuperSlimBoard>();
            for (int caseNumber = 1; caseNumber <= Cases.Count; caseNumber++)
            {
                var c = Cases[caseNumber - 1];

                var boards = c.Boards;
                List<SuperSlimBoard> thisClaimBoards;
                if (caseNumber > 1)
                    thisClaimBoards = boards.SelectMany(b => new[] { b }.Union(_permutationLinked[b].Select(tup => tup.Item2))).ToList();
                else
                    thisClaimBoards = boards;

                wonBoards.AddRange(thisClaimBoards);
                var thisClaimBoardsTex = GeneralizeBoards(thisClaimBoards);

                sb.AppendLine();
                sb.AppendLine("\\bigskip");
                sb.AppendLine(string.Format("\\case{{{0}}}{{$B$ is one of " + thisClaimBoardsTex + ".}}", caseNumber));
                sb.AppendLine();
                sb.AppendLine("\\bigskip");
                if (caseNumber == 1)
                {
                    sb.AppendLine();
                    sb.AppendLine("In all these cases, $H$ is immediately colorable from the lists.");
                }
                else
                {
                    sb.AppendLine();

                    var swapCountGroups = boards.GroupBy(b => Mind.GetWinTreeInfo(b).Max(ti => ti.SwapVertices.Count)).ToList();
                    foreach (var swapCountGroup in swapCountGroups)
                    {
                        if (swapCountGroup.Key == 1)
                        {
                            foreach (var b in swapCountGroup)
                            {
                                var treeInfo = Mind.GetWinTreeInfo(b);
                                var alpha = treeInfo.First().Alpha;
                                var beta = treeInfo.First().Beta;
                                var groups = treeInfo.GroupBy(ss => ss.SwapVertices[0]);

                                sb.Append("$\\K_{" + alpha + "" + beta + ",\\infty}(" + ToListString(b) + "," + groups.OrderBy(gg => gg.Key).Select(gg => gg.Key.GetActiveListIndex(b, _maxPot) + 1).Listify(null) + ")");
                                sb.AppendLine("\\Rightarrow $ " + groups.OrderBy(gg => gg.Key).Select(gg => "$" + GetChildBoardName(b, gg.First()) + "$").Listify(null) + " (Case " + treeInfo.Select(bc => GetHandledCaseNumber(b, bc)).Distinct().OrderBy(xx => xx).Listify() + ").");
                                sb.AppendLine();

                                if (_permutationLinked[b].Count > 0)
                                {
                                    sb.AppendLine();
                                    sb.AppendLine();
                                    sb.AppendLine("Free by vertex permutation: " + _permutationLinked[b].Select(ppp => "$" + ppp.Item1 + "\\Rightarrow " + ToListString(ppp.Item2) + "$").Listify());
                                    sb.AppendLine();
                                    sb.AppendLine();
                                }

                                sb.AppendLine("\\bigskip");
                                sb.AppendLine();
                            }
                        }
                        else if (swapCountGroup.Key == 2)
                        {
                            foreach (var b in swapCountGroup)
                            {
                                var treeInfo = Mind.GetWinTreeInfo(b);
                                var alpha = treeInfo.First().Alpha;
                                var beta = treeInfo.First().Beta;
                                var leftover = treeInfo.ToList();

                                while (leftover.Count > 0)
                                {
                                    var commonestSwapper = Enumerable.Range(0, b._stackCount).MaxIndex(v => leftover.Count(bc => bc.SwapVertices.Contains(v)));
                                    var handledAll = leftover.Where(bc => bc.SwapVertices.Contains(commonestSwapper)).ToList();
                                    var handled = handledAll.Distinct(bc => bc.SwapVertices.Count == 1 ? -1 : bc.SwapVertices.Except(commonestSwapper).First()).ToList();

                                    sb.Append("$\\K_{" + alpha + "" + beta + "," + (commonestSwapper.GetActiveListIndex(b, _maxPot) + 1) + "}(" + ToListString(b));

                                    var single = handled.FirstOrDefault(bc => bc.SwapVertices.Count == 1);
                                    if (single != null)
                                        sb.Append(",\\infty");

                                    if (handled.Where(bc => bc.SwapVertices.Count > 1).Count() > 0)
                                        sb.Append("," + handled.Where(bc => bc.SwapVertices.Count > 1).OrderBy(bc => bc.SwapVertices.Except(commonestSwapper).First()).Select(bc => bc.SwapVertices.Except(commonestSwapper).First().GetActiveListIndex(b, _maxPot) + 1).Listify(null));
                                    sb.Append(")");

                                    sb.AppendLine("\\Rightarrow $ " + handled.OrderBy(bc => bc.SwapVertices.Count == 1 ? -1 : bc.SwapVertices.Except(commonestSwapper).First()).Select(bc => "$" + GetChildBoardName(b, bc) + "$").Listify(null) + " (Case " + handled.Select(bc => GetHandledCaseNumber(b, bc)).Distinct().OrderBy(xx => xx).Listify() + ").");
                                    sb.AppendLine();

                                    foreach (var bc in handledAll)
                                        leftover.Remove(bc);
                                }

                                if (_permutationLinked[b].Count > 0)
                                {
                                    sb.AppendLine();
                                    sb.AppendLine();
                                    sb.AppendLine("Free by vertex permutation: " + _permutationLinked[b].Select(ppp => "$" + ppp.Item1 + "\\Rightarrow " + ToListString(ppp.Item2) + "$").Listify());
                                    sb.AppendLine();
                                    sb.AppendLine();
                                }

                                sb.AppendLine();
                                sb.AppendLine("\\bigskip");
                                sb.AppendLine();
                            }
                        }
                    }
                }
            }

            EndProof(sb);

            return sb.ToString();
        }

        string GetChildBoardName(SuperSlimBoard b, BreakerChoiceInfo bc)
        {
            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
            return ToListString(childBoard);
        }

        void GeneralizeAllBoards(StringBuilder sb)
        {
            _allBoards = Mind.ColorableBoards.Union(Mind.NonColorableBoards).ToList();
            _sequenceComparer = new SequenceGeneralizer<int>.VectorComparer();
            _sequenceGeneralizer = new SequenceGeneralizer<int>(_activeIndices.Count, _possibleListIndices);

            var allBoardsTex = GeneralizeBoards(_allBoards);

            if (Mind.OnlyConsiderNearlyColorableBoards)
                sb.AppendLine("We need to handle all boards that are nearly colorable for edge $e$ up to permutation of colors, so it will suffice to handle all boards of the form " + allBoardsTex + ".");
            else
                sb.AppendLine("We need to handle all boards up to permutation of colors, so it will suffice to handle all boards of the form " + allBoardsTex + ".");

            sb.AppendLine();
        }

        string GeneralizeBoards(List<SuperSlimBoard> boards)
        {
            if (UseWildCards)
            {
                var examples = boards.Select(b => ToListIndices(b)).ToList();
                var nonExamples = Enumerable.Repeat(_possibleListIndices, _activeIndices.Count).CartesianProduct().Select(ll => ll.ToList()).Except(examples.Distinct(_sequenceComparer), _sequenceComparer).ToList();

                var generalized = _sequenceGeneralizer.Generalize(examples, nonExamples, false);
                return generalized.Select(gg => "$" + string.Join("|", gg.Select((_, i) => _.ToTex(_possibleLists, _activeListSizes[i]))) + "$").Listify("or");
            }

            return boards.Select(b => "$" + ToListString(b) + "$").Listify("or");
        }

        List<int> ToListIndices(SuperSlimBoard b)
        {
            var stacks = b.Stacks.Value.Select(l => l.ToSet()).Where(s => s.Count < _maxPot).ToList();
            return stacks.Select(s => _possibleLists.FirstIndex(ss => ss.SequenceEqual(s))).ToList();
        }

        string ToListString(SuperSlimBoard b)
        {
            var stacks = b.Stacks.Value.Select(l => l.ToSet()).Where(s => s.Count < _maxPot).ToList();
            return string.Join("|", stacks.Select(s => string.Join("", s)));
        }

        void GeneratePossibleLists()
        {
            var stacks = Mind.ColorableBoards[0].Stacks.Value.Select(l => l.ToSet()).ToList();
            _activeIndices = stacks.IndicesWhere(s => s.Count < _maxPot).ToList();
            _activeListSizes = stacks.Select(s => s.Count).Where(c => c < _maxPot).ToList();

            var pot = Enumerable.Range(0, _maxPot).ToList();
            _possibleLists = _activeListSizes.Distinct().OrderBy(c => c).ToList().SelectMany(c => pot.EnumerateSublists(c)).ToList();
            _possibleListIndices = Enumerable.Range(0, _possibleLists.Count).ToList();
        }

        protected static void BeginProof(StringBuilder sb)
        {
            sb.AppendLine("\\begin{proof}");
        }

        protected static void EndProof(StringBuilder sb)
        {
            sb.AppendLine("\\end{proof}");
        }

        void AddFigure(StringBuilder sb)
        {
            var figureID = "fig:" + Guid.NewGuid().ToString();

            sb.AppendLine("\\begin{figure}");
            sb.AppendLine("\\centering");
            sb.AppendLine(_figureTikz);
            sb.AppendLine("\\caption{Vertices are ordered as labeled.}\\label{" + figureID + "}");
            sb.AppendLine("\\end{figure}");

            sb.AppendLine("\\begin{lem}");
            sb.AppendLine("The graph in Figure \\ref{" + figureID + "} is reducible.");
            sb.AppendLine("\\end{lem}");
        }
    }
}
