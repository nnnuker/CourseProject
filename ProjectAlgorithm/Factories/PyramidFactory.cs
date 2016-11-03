using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Factories;

namespace ProjectAlgorithm.Factories
{
    public class PyramidFactory : IEntitiesFactory
    {
        public IEntity CreateEntity(float h, float radius, int n)
        {
            return CreateEntity(h, radius, radius, n, 0, 0, 0, 0, 0, 0);
        }

        public IEntity CreateEntity(float h, float radius, float rariusTop, int n)
        {
            return CreateEntity(h, radius, rariusTop, n, 0, 0, 0, 0, 0, 0);
        }

        public IEntity CreateEntity(float h, float radius, float rariusTop, int n, float deltaX, float deltaY, float deltaZ, float deltaXTop, float deltaYTop, float deltaZTop)
        {
            var bottomPoints = GetApproximatedCircle(0, n, radius, deltaX, deltaY, deltaZ);
            var topPoints = GetApproximatedCircle(h, n, rariusTop, deltaXTop, deltaYTop, deltaZTop);

            var verticalLines = GetLines(bottomPoints, topPoints);
            var bottomLines = GetLines(bottomPoints);
            var topLines = GetLines(topPoints);

            var verticalFaces = GetFaces(verticalLines, bottomLines, topLines);
            var bottomFace = GetFaces(bottomLines);
            var topFace = GetFaces(topLines);

            var faces = new List<IFace> { bottomFace, topFace };
            faces.AddRange(verticalFaces);

            return new Entity();
        }

        private IEnumerable<IPoint> GetApproximatedCircle(float h, int n, float r, float deltaX, float deltaY, float deltaZ)
        {
            var alpha = 360.0 / n;
            var points = new List<IPoint>();

            points.Add(new Point((int)Math.Round(r * Math.Cos(330 * Math.PI / 180)), h, (int)Math.Round(r * Math.Sin(330 * Math.PI / 180))));
            points.Add(new Point((int)Math.Round(r * Math.Cos(210 * Math.PI / 180)), h, (int)Math.Round(r * Math.Sin(210 * Math.PI / 180))));
            points.Add(new Point((int)Math.Round(r * Math.Cos(90 * Math.PI / 180)), h, (int)Math.Round(r * Math.Sin(90 * Math.PI / 180))));

            return points;
        }

        private IEnumerable<ILine> GetLines(IEnumerable<IPoint> points)
        {
            var lines = new List<ILine>
            {
                new Line(points.ElementAt(0), points.ElementAt(1)),
                new Line(points.ElementAt(1), points.ElementAt(2)),
                new Line(points.ElementAt(2), points.ElementAt(0))
            };

            return lines;
        }

        private IEnumerable<ILine> GetLines(IEnumerable<IPoint> pointsFirst, IEnumerable<IPoint> pointsSecond)
        {
            var lines = new List<ILine>();

            for (int i = 0; i < pointsFirst.Count(); i++)
            {
                lines.Add(new Line(pointsFirst.ElementAt(i), pointsSecond.ElementAt(i)));
            }

            return lines;
        }

        private IEnumerable<IFace> GetFaces(IEnumerable<ILine> verticalLines, IEnumerable<ILine> bottomLines, IEnumerable<ILine> topLines)
        {
            var faces = new List<IFace>();

            //var count = bottomLines.Count();

            //for (int i = 0; i < count - 1; i++)
            //{
            //    faces.Add(
            //        new Face(
            //            new List<ILine> { verticalLines.ElementAt(i), topLines.ElementAt(i),
            //                verticalLines.ElementAt(i + 1), bottomLines.ElementAt(i) }));
            //}

            //faces.Add(
            //    new Face(
            //            new List<ILine> { verticalLines.ElementAt(count - 1), topLines.ElementAt(count - 1),
            //                verticalLines.ElementAt(0), bottomLines.ElementAt(count - 1) }));

            return faces;
        }

        private IFace GetFaces(IEnumerable<ILine> lines)
        {
            return new Face();
        }

        public IEntity CreateEntity(float h, float radius, int n, bool reverseNormal)
        {
            throw new NotImplementedException();
        }

        public IEntity CreateEntity(float h, float radius, float rariusTop, int n, bool reverseNormal)
        {
            throw new NotImplementedException();
        }

        public IEntity CreateEntity(float h, float radius, float rariusTop, int n, float deltaX, float deltaY, float deltaZ, float deltaXTop, float deltaYTop, float deltaZTop, bool reverseNormal)
        {
            throw new NotImplementedException();
        }

        public IEntity CreateEntity(IEnumerable<IPoint> top, IEnumerable<IPoint> bottom, bool reverseNormal)
        {
            throw new NotImplementedException();
        }
    }
}