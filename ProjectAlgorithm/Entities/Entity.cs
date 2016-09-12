using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Entities
{
    public class Entity : IEntity
    {
        private readonly IList<IFace> faces;

        public IList<IFace> Faces => faces;

        public Entity(IEnumerable<IFace> faces)
        {
            this.faces = faces as IList<IFace> ?? faces.ToList();
        }

        public object Clone()
        {
            return new Entity((IEnumerable<IFace>)faces.Select(f => f.Clone()));
        }
    }
}