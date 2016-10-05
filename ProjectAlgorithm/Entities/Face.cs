using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Entities
{
    public class Face : IFace
    {
        private readonly IList<ILine> lines;

        public IList<ILine> Lines { get { return lines; } }

        public Face()
        {
            lines = new List<ILine>();
        }

        public Face(IEnumerable<ILine> lines)
        {
            this.lines = lines as IList<ILine> ?? lines.ToList();
        }

        public object Clone()
        {
            return new Face(lines.Select(l => (ILine)l.Clone()));
        }
    }
}