using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Interfaces;
using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Entities
{
    public class CompositeObject : ICompositeObject, ICompositeChangeable
    {
        #region Fields

        private readonly IList<IEntity> entities;

        #endregion

        #region Event

        public event EventHandler<UpdateObjectEventArgs> OnChange = delegate { };

        #endregion

        #region Properties

        public IList<IEntity> Entities
        {
            get
            {
                return entities;
            }
        }

        #endregion

        #region Ctors

        public CompositeObject()
        {
            entities = new List<IEntity>();
        }

        public CompositeObject(IEnumerable<IEntity> entities)
        {
            this.entities = entities as IList<IEntity> ?? entities.ToList();
        }

        #endregion

        #region Public methods

        public IList<IFace> GetFaces()
        {
            return entities.Aggregate(new List<IFace>(), (list, entity) =>
            {
                list.AddRange(entity.Faces);
                return list;
            });
        }

        public IList<IPoint> GetPoints()
        {
            var points = new List<IPoint>();

            foreach (var entity in entities)
            {
                foreach (var face in entity.Faces)
                {
                    points.AddRange(face.Points);
                }
            }

            points = points.Distinct().ToList();

            //OnChangeMethod(new UpdateObjectEventArgs(lines));

            return points;
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

            lines = lines.Distinct().ToList();

            //OnChangeMethod(new UpdateObjectEventArgs(lines));

            return lines;
        }

        public object Clone()
        {
            return new CompositeObject(entities.Select(e => (IEntity)e.Clone()));
        }

        #endregion

        #region Protected method

        protected virtual void OnChangeMethod(UpdateObjectEventArgs eventArgs)
        {
            if (eventArgs == null) throw new ArgumentNullException("eventArgs");

            OnChange(this, eventArgs);
        }

        #endregion
    }
}