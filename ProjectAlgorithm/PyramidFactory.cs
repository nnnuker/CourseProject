using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Entities;

namespace ProjectAlgorithm
{
    public class PyramidFactory : IEntitiesFactory
    {
        public Entity CreateEntity(float h, float radius, int n)
        {
            return CreateEntity(h, radius, radius, n, 0, 0, 0, 0, 0, 0);
        }

        public Entity CreateEntity(float h, float radius, float rariusTop, int n)
        {
            return CreateEntity(h, radius, rariusTop, n, 0, 0, 0, 0, 0, 0);
        }

        public Entity CreateEntity(float h, float radius, float rariusTop, int n,
            float deltaX, float deltaY, float deltaZ, float deltaXTop, float deltaYTop, float deltaZTop)
        {
            var bottomPoints = GetApproximatedCircle(0, n, radius, deltaX, deltaY, deltaZ);
            var topPoints = GetApproximatedCircle(h, n, rariusTop, deltaXTop, deltaYTop, deltaZTop);

            var verticalLines = GetLines(bottomPoints, topPoints);
            var bottomLines = GetLines(bottomPoints);
            var topLines = GetLines(topPoints);

            var verticalFaces = GetFaces(verticalLines, bottomLines, topLines);
            var bottomFace = GetFaces(bottomLines);
            var topFace = GetFaces(topLines);

            var faces = new List<Face> { bottomFace, topFace };
            faces.AddRange(verticalFaces);

            return new Entity(faces);
        }

        private IList<Point> GetApproximatedCircle(float h, int n, float r, float deltaX, float deltaY, float deltaZ)
        {
            var alpha = 360.0 / n;
            var points = new List<Point>();

            points.Add(new Point((int)Math.Round(r * Math.Cos(330 * Math.PI / 180)), h, (int)Math.Round(r * Math.Sin(330 * Math.PI / 180))));
            points.Add(new Point((int)Math.Round(r * Math.Cos(210 * Math.PI / 180)), h, (int)Math.Round(r * Math.Sin(210 * Math.PI / 180))));
            points.Add(new Point((int)Math.Round(r * Math.Cos(90 * Math.PI / 180)), h, (int)Math.Round(r * Math.Sin(90 * Math.PI / 180))));

            return points;
        }

        private IList<Line> GetLines(IEnumerable<Point> points)
        {
            var lines = new List<Line>
            {
                new Line(points.ElementAt(0), points.ElementAt(1)),
                new Line(points.ElementAt(1), points.ElementAt(2)),
                new Line(points.ElementAt(2), points.ElementAt(0))
            };

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