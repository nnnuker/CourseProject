using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Entities
{
    public class Face : IFace
    {
        private IList<IPoint> points;
        private IList<ILine> lines;

        public IList<IPoint> Points { get { return points; } }
        public IList<ILine> Lines { get { return lines; } }

        public IEnumerable<float> Normal { get { return GetNormal(); } }
        public IEnumerable<float> Center { get { return GetCenter(); } }

        public Color Color { get; set; }

        public bool IsHidden { get; set; }

        public bool ReverseNormal { get; set; }

        public Face()
        {
            lines = new List<ILine>();
            points = new List<IPoint>();
        }

        public Face(IEnumerable<IPoint> points)
        {
            this.points = points as IList<IPoint> ?? points.ToList();

            lines = GetLines();
            IsHidden = false;
        }

        public Face(IEnumerable<IPoint> points, Color color)
        {
            this.points = points as IList<IPoint> ?? points.ToList();

            lines = GetLines();
            IsHidden = false;
            this.Color = color;
        }

        public object Clone()
        {
            return new Face(points.Select(l => (IPoint)l.Clone()), Color)
            {
                IsHidden = this.IsHidden,
                ReverseNormal = this.ReverseNormal
            };
        }

        private IList<ILine> GetLines()
        {
            var list = new List<ILine>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                list.Add(new Line(points[i], points[i + 1], Color));
            }

            list.Add(new Line(points[points.Count - 1], points[0], Color));

            return list;
        }

        private IEnumerable<float> GetCenter()
        {
            var points = Lines.Aggregate(new List<IPoint>(), (list, line) =>
            {
                list.AddRange(line.Points);
                return list;
            }).Distinct().ToList();

            var coordCount = points.First().Coordinates.Count();

            var center = new float[coordCount];

            foreach (var point in points)
            {
                for (var i = 0; i < coordCount; i++)
                {
                    center[i] += point.Coordinates.ElementAt(i);
                }
            }

            var n = points.Count;

            return center.Select(c => c / n);
        }

        private IEnumerable<float> GetNormal()
        {
            if (Lines.Count < 3)
            {
                return null;
            }

            var first = Lines[0].First;
            var second = Lines[1].First;
            var third = Lines[2].First;

            var x = first.Y * second.Z + second.Y * third.Z + third.Y * first.Z - 
                second.Y * first.Z - third.Y * second.Z - first.Y * third.Z;
            var y = first.Z * second.X + second.Z * third.X + third.Z * first.X - 
                second.Z * first.X - third.Z * second.X - first.Z * third.X;
            var z = first.X * second.Y + second.X * third.Y + third.X * first.Y - 
                second.X * first.Y - third.X * second.Y - first.X * third.Y;

            if (ReverseNormal)
            {
                return new[] { -x, -y, -z };
            }

            return new [] { x, y, z };
        }

        
    }
}