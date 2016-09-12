using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Entities
{
    public class Face : IFace
    {
        private readonly IList<ILine> lines;

        public IList<ILine> Lines => lines;

        public Face(IEnumerable<ILine> lines)
        {
            this.lines = lines as IList<ILine> ?? lines.ToList();
        }

        public object Clone()
        {
            return new Face((IList<ILine>)lines.Select(l => l.Clone()));
        }
    }
}