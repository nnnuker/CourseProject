using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Entities;

namespace ProjectAlgorithm
{
    public class EntitiesFactory : IEntitiesFactory
    {
        public Entity CreateEntity(float h, float radius, int n)
        {
            return CreateEntity(h, radius, radius, n, 0, 0, 0, 0, 0, 0);
        }

        public Entity CreateEntity(float h, float radius, float radiusTop, int n)
        {
            return CreateEntity(h, radius, radiusTop, n, 0, 0, 0, 0, 0, 0);
        }

        public Entity CreateEntity(float h, float radius, float radiusTop, int n,
            float deltaX, float deltaY, float deltaZ, float deltaXTop, float deltaYTop, float deltaZTop)
        {
            if (h == 0 || (radius == 0 && radiusTop == 0))
            {
                return new Entity();
            }

            var bottomPoints = GetApproximatedCircle(0, n, radius, deltaX, deltaY, deltaZ);
            var topPoints = GetApproximatedCircle(h, n, radiusTop, deltaXTop, deltaYTop, deltaZTop);

            var verticalLines = GetLines(bottomPoints, topPoints);
            var bottomLines = GetLines(bottomPoints);
            var topLines = GetLines(topPoints);

            var verticalFaces = GetFaces(verticalLines, bottomLines, topLines);
            var bottomFace = GetFaces(bottomLines);
            var topFace = GetFaces(topLines);

            var faces = new List<Face> {bottomFace, topFace};
            faces.AddRange(verticalFaces);
            
            return new Entity(faces);
        }

        private IList<Point> GetApproximatedCircle(float h, int n, float r, float deltaX, float deltaY, float deltaZ)
        {
            var alpha = 360.0 / n;
            var grad = 0.0;
            var points = new List<Point>();

            for (int i = 0; i < n; i++)
            {
                var z = (int)Math.Round(r * Math.Sin(grad * Math.PI / 180));
                var x = (int)Math.Round(r * Math.Cos(grad * Math.PI / 180));
                points.Add(new Point(x + deltaX, h + deltaY, z + deltaZ));
                grad += alpha;
            }

            return points;
        }

        private IList<Line> GetLines(IEnumerable<Point> points)
        {
            var lines = new List<Line>();

            var count = points.Count();

            for (int i = 0; i < count - 1; i++)
            {
                lines.Add(new Line(points.ElementAt(i), points.ElementAt(i + 1)));
            }

            lines.Add(new Line(points.ElementAt(count - 1), points.ElementAt(0)));

            return lines;
        }

        private IList<Line> GetLines(IEnumerable<Point> pointsFirst, IEnumerable<Point> pointsSecond)
        {
            var lines = new List<Line>();

            for (int i = 0; i < pointsFirst.Count(); i++)
            {
                lines.Add(new Line(pointsFirst.ElementAt(i), pointsSecond.ElementAt(i)));
            }

            return lines;
        }

        private IList<Face> GetFaces(IEnumerable<Line> verticalLines, IEnumerable<Line> bottomLines, IEnumerable<Line> topLines)
        {
            var faces = new List<Face>();

            var count = bottomLines.Count();

            for (int i = 0; i < count - 1; i++)
            {
                faces.Add(
                    new Face(
                        new List<Line> { verticalLines.ElementAt(i), topLines.ElementAt(i),
                            verticalLines.ElementAt(i + 1), bottomLines.ElementAt(i) }));
            }

            faces.Add(
                new Face(
                        new List<Line> { verticalLines.ElementAt(count - 1), topLines.ElementAt(count - 1),
                            verticalLines.ElementAt(0), bottomLines.ElementAt(count - 1) }));

            return faces;
        }

        private Face GetFaces(IEnumerable<Line> lines)
        {
            return new Face(lines);
        }
    }
}