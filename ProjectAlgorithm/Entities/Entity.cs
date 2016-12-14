using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Entities
{
    public class Entity : IEntity
    {
        #region Fields

        private IList<Face> faces;
        private IList<IPoint> top;
        private IList<IPoint> bottom;

        #endregion

        #region Properties

        public IList<Face> Faces { get { return faces; } }
        public IList<IPoint> Top { get { return top; } }
        public IList<IPoint> Bottom { get { return bottom; } }

        #endregion

        #region Ctors

        public Entity()
        {
            faces = new List<Face>();
            top = new List<IPoint>();
            bottom = new List<IPoint>();
        }

        public Entity(IEnumerable<Face> faces, IEnumerable<IPoint> top, IEnumerable<IPoint> bottom)
        {
            if (faces == null) throw new ArgumentNullException("faces");
            if (top == null) throw new ArgumentNullException("top");
            if (bottom == null) throw new ArgumentNullException("bottom");

            this.top = top.ToList();
            this.bottom = bottom.ToList();
            this.faces = faces.ToList();
        }

        #endregion

        #region Public methods
        
        public object Clone()
        {
            return new Entity(faces.Select(f => (Face)f.Clone()), top.Select(t => (IPoint)t.Clone()), bottom.Select(b => (IPoint)b.Clone()));
        }

        #endregion
    }
}