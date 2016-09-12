using System;

namespace ProjectAlgorithm.Interfaces
{
    public interface IPoint : ICloneable
    {
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }
    }
}