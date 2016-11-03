using System;
using System.Collections.Generic;

namespace ProjectAlgorithm.Interfaces.Entities
{
    public interface ILine : ICloneable
    {
        IPoint First { get; set; }

        IPoint Second { get; set; }

        IEnumerable<IPoint> Points { get; }
    }
}