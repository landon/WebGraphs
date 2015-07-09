using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Choosability.FixerBreaker.KnowledgeEngine.Slim.Super.Proofs
{
    public class ArbitraryDegreeProofBuilder : PermutationAwareProofBuilder
    {
        string _figureTikz;
        public ArbitraryDegreeProofBuilder(SuperSlimMind mind, string figureTikz = "")
            : base(mind)
        {
            _figureTikz = figureTikz;
        }

        public override string WriteProof()
        {
            var sb = new StringBuilder();

            var figureID = "fig:" + Guid.NewGuid().ToString();

            sb.AppendLine("\\begin{figure}");
            sb.AppendLine("\\centering");
            sb.AppendLine(_figureTikz);
            sb.AppendLine("\\caption{Vertices are labeled with their list size.}\\label{" + figureID + "}");
            sb.AppendLine("\\end{figure}");

            sb.AppendLine("\\begin{lem}");
            sb.AppendLine("The graph in Figure \\ref{" + figureID + "} is reducible.");
            sb.AppendLine("\\end{lem}");

            return sb.ToString();
        }
    }
}
