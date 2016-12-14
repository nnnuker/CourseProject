using System;
using System.Collections.Generic;
using ProjectAlgorithm.Entities;

namespace ProjectAlgorithm.Interfaces.Entities
{
    public interface IEntity : ICloneable
    {
        IList<Face> Faces { get; }
        IList<IPoint> Top { get; }
        IList<IPoint> Bottom { get; }
    }
}