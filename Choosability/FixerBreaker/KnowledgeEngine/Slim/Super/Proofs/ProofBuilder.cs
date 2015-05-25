using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class ProofBuilder
    {
        public SuperSlimMind Mind { get; private set; }
        public List<ProofCase> Cases { get; private set; }

        public ProofBuilder(SuperSlimMind mind)
        {
            Mind = mind;
            ExtractCases();
        }

        void ExtractCases()
        {
            Cases = new List<ProofCase>();

            var colorableCase = new ProofCase(Mind, 0, Mind.ColorableBoards);
            Cases.Add(colorableCase);

            var depthToCase = new Dictionary<int, ProofCase>();
            foreach (var board in Mind.NonColorableBoards)
            {
                var tree = Mind.BuildGameTree(board);
                var depth = tree.GetDepth();

                ProofCase proofCase;
                if (!depthToCase.TryGetValue(depth, out proofCase))
                {
                    proofCase = new ProofCase(Mind, depth);
                    depthToCase[depth] = proofCase;
                    Cases.Add(proofCase);
                }

                proofCase.AddBoard(board);
            }

            Cases.Sort((c1, c2) => c1.Depth.CompareTo(c2.Depth));
        }

        public virtual string WriteProof()
        {
            var sb = new StringBuilder();
            
            for(int caseNumber = 1; caseNumber <= Cases.Count; caseNumber++)
            {
                var c = Cases[caseNumber - 1];

                var boardsXYZ = string.Join(", ", c.Boards.Select(b => b.ToXYZ()));
                var countModifier = c.Boards.Count > 1 ? "one of " : "";
                sb.AppendLine(string.Format("\\case{{{0}}}{{$B$ is " + countModifier + boardsXYZ + ".}}", caseNumber));

                if (caseNumber == 1)
                {
                    sb.AppendLine("In all these cases, $G$ is immediately colorable from the lists. (JUSTIFICATION NEEDED?)");
                }
                else
                {
                    if (c.Boards.Count > 1)
                        sb.AppendLine("We show that for each board, one of X-fix, Y-fix, or Z-fix gets to a board in a previous case.");
                    foreach (var b in c.Boards)
                    {
                        sb.AppendLine();
                        if (c.Boards.Count > 1)
                            sb.Append("For " + b.ToXYZ() + ", ");
                        
                        var treeInfo = Mind.GetWinTreeInfo(b);
                        var fixLetter = ((long)((1 << treeInfo.First().Alpha) | (1 << treeInfo.First().Beta))).ToXYZ();
                        var others = new List<string>() {"X", "Y", "Z"};
                        others.Remove(fixLetter);

                        if (c.Boards.Count > 1)
                            sb.Append("we");
                        else
                            sb.Append("We");
                        sb.AppendLine(" perform " + fixLetter.GetArticle() + " " + fixLetter + "-fix.");

                        foreach (var bc in treeInfo)
                        {
                            var partitionId = b.ToPartitionId(bc.Partition);
                            sb.Append("If the partition is " + b.ToCompactedPartitionId(bc.Partition) + ", then swapping " + others[0] + " and " + others[1] + " at " + bc.SwapVertices.Select(v => partitionId[v]).Distinct().Listify());
                            
                            var childBoard = new SuperSlimBoard(b._trace, bc.Alpha, bc.Beta, bc.Response, b._stackCount);
                            sb.Append(" yields " + childBoard.ToXYZ() + " which we already handled in Case ");
                            var handledCaseNumber = Cases.IndicesWhere(cc => cc.Boards.Contains(childBoard)).First() + 1;
                            sb.AppendLine(handledCaseNumber + ".");
                        }
                    }
                }

                sb.AppendLine();
            }


            return sb.ToString();
        }
    }
}
