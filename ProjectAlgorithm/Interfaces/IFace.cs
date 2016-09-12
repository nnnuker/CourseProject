using System;
using System.Collections.Generic;

namespace ProjectAlgorithm.Interfaces
{
    public interface IFace : ICloneable
    {
        IList<ILine> Lines { get; }
    }
}