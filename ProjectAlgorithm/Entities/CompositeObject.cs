using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Entities
{
    public class CompositeObject : ICompositeObject
    {
        private readonly IList<IEntity> entities;

        public IList<IEntity> Entities => entities;

        public CompositeObject(IEnumerable<IEntity> entities)
        {
            this.entities = entities as IList<IEntity> ?? entities.ToList();
        }

        public IList<ILine> GetLines()
        {
            var lines = new List<ILine>();

            foreach (var entity in entities)
            {
                foreach (var face in entity.Faces)
                {
                    lines.AddRange(face.Lines);
                }
            }

            return lines;
        }

        public object Clone()
        {
            return new CompositeObject((IEnumerable<IEntity>)entities.Select(e => e.Clone()));
        }
    }
}