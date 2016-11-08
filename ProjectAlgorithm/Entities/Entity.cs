using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Entities
{
    public class Entity : IEntity
    {
        #region Fields

        private IList<IFace> faces;
        private IList<IPoint> top;
        private IList<IPoint> bottom;

        #endregion

        #region Properties

        public IList<IFace> Faces { get { return faces; } }
        public IList<IPoint> Top { get { return top; } }
        public IList<IPoint> Bottom { get { return bottom; } }

        #endregion

        #region Ctors

        public Entity()
        {
            faces = new List<IFace>();
            top = new List<IPoint>();
            bottom = new List<IPoint>();
        }

        public Entity(IEnumerable<IFace> faces, IEnumerable<IPoint> top, IEnumerable<IPoint> bottom)
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

        //public override bool Equals(object obj)
        //{
        //    if (!(obj is Entity))
        //    {
        //        return false;
        //    }

        //    var entity = (Entity)obj;

        //    return this.Faces.SequenceEqual(entity.faces);
        //}

        public object Clone()
        {
            return new Entity(faces.Select(f => (IFace)f.Clone()), top.Select(t => (IPoint)t.Clone()), bottom.Select(b => (IPoint)b.Clone()));
        }

        #endregion
    }
}