using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Entities
{
    public class CompositeObject : ICompositeObject, ICompositeChangeable
    {
        private readonly IList<IEntity> entities;

        public event EventHandler<UpdateObjectEventArgs> OnChange = delegate { };

        public IList<IEntity> Entities
        {
            get
            {
                return entities;
            }
        }

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

            var distLines = lines.Distinct().ToList();

            OnChangeMethod(new UpdateObjectEventArgs(distLines));

            return distLines;
        }

        protected virtual void OnChangeMethod(UpdateObjectEventArgs eventArgs)
        {
            if (eventArgs == null) throw new ArgumentNullException("eventArgs");

            OnChange(this, eventArgs);
        }

        public object Clone()
        {
            return new CompositeObject(entities.Select(e => (IEntity)e.Clone()));
        }

        
    }
}