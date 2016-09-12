using System;

namespace ProjectAlgorithm.Interfaces
{
    public interface ILine : ICloneable
    {
        IPoint First { get; set; }

        IPoint Second { get; set; }
    }
}