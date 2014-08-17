using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuncLib.Functions;
using FuncLib.Optimization;

namespace Choosability
{
    public static class PolynomialMasterFuncLib
    {
        public static Function ColoringPolynomialFuncLib(this Choosability.Graph g)
        {
            List<Variable> x;
            return ColoringPolynomialFuncLib(g, out x);
        }
        public static Function ColoringPolynomialFuncLib(this Choosability.Graph g, out List<Variable> x)
        {
            x = new List<Variable>();
            for (int v = 0; v < g.N; v++)
                x.Add(new Variable("x_" + v));

            Function f = 1;
            for (int v = 0; v < g.N; v++)
            {
                for (int w = v + 1; w < g.N; w++)
                {
                    if (g[v, w])
                        f *= x[v] - x[w];
                }
            }

            return f;
        }

        public static int GetCoefficientFuncLib(this Choosability.Graph g, params int[] power)
        {
            System.Diagnostics.Debug.Assert(power.Count() == g.N);
            System.Diagnostics.Debug.Assert(power.Sum() == g.E);

            List<Variable> x;
            var f = g.ColoringPolynomialFuncLib(out x);
            for(int v = 0; v < power.Length; v++)
            {
                f = f.Derivative(x[v], power[v]);
                f = f.PartialValue(new VariableAssignment(x[v], 0));
            }

            var vv = (int)f.Value();
            for (int i = 0; i < power.Length; i++)
                for (int j = 1; j <= power[i]; j++)
                    vv /= j;

            return vv;
        }
    }
}
