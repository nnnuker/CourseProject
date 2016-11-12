using System;
using System.Collections.Generic;

namespace ProjectAlgorithm.Interfaces.Entities
{
    public interface ICompositeObject : ICloneable
    {
        IList<IEntity> Entities { get; }
        IList<IFace> GetFaces();
        IList<ILine> GetLines();
        IList<IPoint> GetPoints();
    }
}