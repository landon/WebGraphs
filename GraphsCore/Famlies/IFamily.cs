using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphsCore.Famlies
{
    public interface IFamily
    {
        string Name { get; }
        Graphs.Graph Create(double w, object data);
    }
}
