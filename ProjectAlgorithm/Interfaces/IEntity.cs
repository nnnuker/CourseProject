using System;
using System.Collections.Generic;

namespace ProjectAlgorithm.Interfaces
{
    public interface IEntity : ICloneable
    {
        IList<IFace> Faces { get; }
    }
}