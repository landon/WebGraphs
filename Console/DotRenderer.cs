﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Choosability.FixerBreaker;
using Choosability.FixerBreaker.Chronicle;
using Choosability.FixerBreaker.KnowledgeEngine;
using Choosability.FixerBreaker.KnowledgeEngine.Slim.Super;
using Choosability;
using System.Threading;

namespace Console
{
    public enum DotRenderType
    {
        pdf,
        png,
        svg,
        eps,
        ps
    }
    public class DotRenderer
    {
        string _dotPath;
        public bool SkipLayout { get; set; }

        public DotRenderer(string dotPath)
        {
            _dotPath = dotPath;
        }

        public string Render(string dot, string fileName, DotRenderType renderType)
        {
            var root = Path.GetDirectoryName(fileName);
            Directory.CreateDirectory(root);

            fileName = Path.Combine(root, Path.GetFileNameWithoutExtension(fileName) + "." + renderType);

            var tempFile = Path.GetTempFileName();
            using (var sw = new StreamWriter(tempFile))
                sw.Write(dot);

            string arguments;
            if (SkipLayout)
            {
                arguments = string.Format(@"-n -T{0} ""{2}"" -o ""{1}""", renderType, fileName, tempFile);
            }
            else
            {
                arguments = string.Format(@"-T{0} ""{2}"" -o ""{1}""", renderType, fileName, tempFile);
            }
            var info = new ProcessStartInfo(_dotPath, arguments);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            var process = new Process();
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();

            File.Delete(tempFile);

            return fileName;
        }

        public void Render(Graph graph, Dictionary<int, GameTree> treeLookup, string fileName, DotRenderType renderType, bool drawCached)
        {
            Render(ToDotBasic(graph, treeLookup, drawCached), fileName, renderType);
        }

        string ToDotBasic(Graph g, Dictionary<int, GameTree> treeLookup, bool drawCached)
        {
            var sb = new StringBuilder();

            sb.AppendLine("digraph G {");
            sb.Append("edge [fontname = Arial, fontsize = 10];");
            sb.Append("node [margin=0 fontcolor=black shape=box];");
            foreach (int v in g.Vertices)
            {
                var tree = treeLookup[v];
                var shadow = "  ";

                var infoLabel = "";
                if (tree.IsColorable)
                    infoLabel = "colorable";
                else if (!tree.IsSuperabundant)
                    infoLabel = "not superabundant";
                else if (tree.SameAsIndex > 0)
                    infoLabel = "same as " + tree.SameAsIndex;
                var sbb = new StringBuilder();
                sbb.AppendLine(@"<<table cellpadding='3' cellborder='0' cellspacing='0' border='0'>");
                sbb.AppendLine(string.Format(@"<tr><td colspan='2' align='left' bgcolor='black'><font color='white'>{2}</font></td><td colspan='3' align='right' bgcolor='black'><font color='white'>{3}</font></td></tr><tr><td></td><td></td><td></td><td></td><td></td></tr><tr rowspan='2'><td colspan='5'><FONT POINT-SIZE='18'>{0}</FONT></td></tr><tr><td colspan='5'>{1}</td></tr>", 
                    tree.Board, infoLabel, tree.GameTreeIndex, shadow));
                sbb.AppendLine("</table>>");

                sb.AppendLine(string.Format("{0} [label = {2}, color = \"{1}\"];", v, "black", sbb.ToString()));
            }

            for (int i = 0; i < g.N; i++)
                for (int j = 0; j < g.N; j++)
                {
                    if (g.Directed[i, j])
                    {
                        var tree = treeLookup[j];
                        var penWidth = 3;

                        var sbb = new StringBuilder();

                        sbb.AppendLine(@"<<table cellpadding='2' cellborder='1' cellspacing='0' border='0'>");
                        sbb.AppendLine(string.Format(@"<tr>
<td bgcolor='white' colspan='{0}'>swap {1}</td>
</tr>", tree.Info.Partition.Count, tree.Info.Alpha + "" + tree.Info.Beta));

                        sbb.AppendLine("<tr>");
                        for (int qq = 0; qq < tree.Info.Partition.Count; qq++)
                        {
                            var swapped = tree.Info.Partition[qq].All(xx => tree.Info.SwapVertices.Contains(xx));

                            sbb.AppendLine(string.Format("<td bgcolor = '{0}'>{1}</td>", swapped ? "green" : "white", string.Join("", tree.Info.Partition[qq].Select(xx => xx + 1).OrderBy(xx => xx))));
                        }
                        sbb.AppendLine("</tr>");
                        sbb.AppendLine("</table>>");

                        sb.AppendLine(string.Format("{0} -> {1} [label = {2}, penwidth = {3}]", i, j, sbb.ToString(), penWidth));
                    }
                }
            sb.AppendLine("}");

            return sb.ToString();
        }

    }
}
