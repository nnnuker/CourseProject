using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Entities
{
    public class Entity : IEntity
    {
        private IList<IFace> faces;
        private IList<IPoint> top;
        private IList<IPoint> bottom;

        public IList<IFace> Faces { get { return faces; } }
        public IList<IPoint> Top { get { return top; } }
        public IList<IPoint> Bottom { get { return bottom; } }

        public Entity()
        {
            faces = new List<IFace>();
            top = new List<IPoint>();
            bottom = new List<IPoint>();
        }

        public Entity(IEnumerable<IFace> faces, IEnumerable<IPoint> top, IEnumerable<IPoint> bottom)
        {
            this.top = top.ToList();
            this.bottom = bottom.ToList();
            this.faces = faces as IList<IFace> ?? faces.ToList();
        }

        public object Clone()
        {
            return new Entity(faces.Select(f => (IFace)f.Clone()), top.Select(t => (IPoint)t.Clone()), bottom.Select(b => (IPoint)b.Clone()));
        }
    }
}