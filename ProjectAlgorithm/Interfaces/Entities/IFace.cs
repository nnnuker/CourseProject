using System;
using System.Collections.Generic;
using ProjectAlgorithm.Interfaces.HiddenLines;

namespace ProjectAlgorithm.Interfaces.Entities
{
    public interface IFace : IHiddenable, ICloneable
    {
        IList<IPoint> Points { get; }
        IList<ILine> Lines { get; }

        bool ReverseNormal { get; set; }
        IEnumerable<float> Normal { get; }
        IEnumerable<float> Center { get; }
    }
}