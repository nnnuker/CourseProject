using System;
using System.Collections.Generic;

namespace ProjectAlgorithm.Interfaces.Entities
{
    public interface IEntity : ICloneable
    {
        IList<IFace> Faces { get; }
        IList<IPoint> Top { get; }
        IList<IPoint> Bottom { get; }
    }
}