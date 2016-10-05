using System;
using System.Collections.Generic;

namespace ProjectAlgorithm.Interfaces
{
    public interface ILine : ICloneable
    {
        IPoint First { get; set; }

        IPoint Second { get; set; }
    }
}