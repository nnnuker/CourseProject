using System;
using System.Collections.Generic;

namespace ProjectAlgorithm.Interfaces
{
    public interface ICompositeObject : ICloneable
    {
        IList<IEntity> Entities { get; }

        IList<ILine> GetLines();
    }
}