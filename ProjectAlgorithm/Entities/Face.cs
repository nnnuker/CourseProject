using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Entities
{
    public class Face : IFace
    {
        #region Fields

        private readonly IList<IPoint> points;
        private readonly IList<ILine> lines;

        #endregion

        #region Properties

        public IList<IPoint> Points { get { return points; } }
        public IList<ILine> Lines { get { return GetLines(); } }

        public IEnumerable<float> Normal { get { return GetNormal(); } }
        public IEnumerable<float> Center { get { return GetCenter(); } }

        public Color Color { get; set; } 

        public bool IsHidden { get; set; }

        public bool ReverseNormal { get; set; }

        #endregion

        #region Ctors

        public Face()
        {
            lines = new List<ILine>();
            points = new List<IPoint>();

            Color = Color.Black;
        }

        public Face(IEnumerable<IPoint> points)
        {
            if (points == null) throw new ArgumentNullException("points");

            this.points = points.ToList();

            lines = GetLines();
            Color = Color.Black;
        }

        public Face(IEnumerable<IPoint> points, Color color)
        {
            if (points == null) throw new ArgumentNullException("points");

            this.points = points.ToList();

            this.Color = color;

            lines = GetLines();
        }

        #endregion

        #region Public methods

        public object Clone()
        {
            return new Face(points.Select(l => (IPoint)l.Clone()), Color)
            {
                IsHidden = this.IsHidden,
                ReverseNormal = this.ReverseNormal
            };
        }

        //public override bool Equals(object obj)
        //{
        //    if (!(obj is Face))
        //    {
        //        return false;
        //    }

        //    var face = (Face)obj;

        //    return this.points.SequenceEqual(face.points);
        //}

        #endregion

        #region Private methods

        private IList<ILine> GetLines()
        {
            var list = new List<ILine>();

            if (points.Count < 2)
            {
                return list;
            }

            for (var i = 0; i < points.Count - 1; i++)
            {
                list.Add(new Line(points[i], points[i + 1], Color, IsHidden));
            }

            list.Add(new Line(points[points.Count - 1], points[0], Color, IsHidden));

            return list;
        }

        private IEnumerable<float> GetCenter()
        {
            if (points == null || points.Count < 3)
            {
                return null;
            }

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
            if (Points.Count < 3)
            {
                return null;
            }

            var first = Points[0];
            var second = Points[1];
            var third = Points[2];

            var x = first.Y * second.Z + second.Y * third.Z + third.Y * first.Z - 
                    second.Y * first.Z - third.Y * second.Z - first.Y * third.Z;
            var y = first.Z * second.X + second.Z * third.X + third.Z * first.X - 
                    second.Z * first.X - third.Z * second.X - first.Z * third.X;
            var z = first.X * second.Y + second.X * third.Y + third.X * first.Y - 
                    second.X * first.Y - third.X * second.Y - first.X * third.Y;

            return ReverseNormal ? new[] { -x, -y, -z } : new [] { x, y, z };
        }

        #endregion
    }
}