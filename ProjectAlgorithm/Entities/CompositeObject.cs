using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Entities
{
    public class CompositeObject : ICompositeObject, ICompositeChangeable
    {
        private readonly IList<IEntity> entities;
        private List<ILine> lines;

        public event EventHandler<UpdateObjectEventArgs> OnChange = delegate { };

        public IList<IEntity> Entities
        {
            get
            {
                return entities;
            }
        }

        public CompositeObject()
        {
            entities = new List<IEntity>();
            lines = new List<ILine>();
        }

        public CompositeObject(IEnumerable<IEntity> entities)
        {
            this.entities = entities as IList<IEntity> ?? entities.ToList();
            lines = new List<ILine>();
        }

        public IList<ILine> GetLines()
        {
            if (this.lines.Count != 0)
            {
                return this.lines;
            }

            this.lines.Clear();

            foreach (var entity in entities)
            {
                foreach (var face in entity.Faces)
                {
                    this.lines.AddRange(face.Lines);
                }
            }

            lines = lines.Distinct().ToList();

            OnChangeMethod(new UpdateObjectEventArgs(lines));

            return lines;
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