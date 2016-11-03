using System;
using System.Collections.Generic;

namespace ProjectAlgorithm.Interfaces.Entities
{
    public interface IPoint : ICloneable
    {
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }
        IEnumerable<float> Coordinates { get; }
    }
}