using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Factories;
using Point = ProjectAlgorithm.Entities.Point;

namespace ProjectAlgorithm.Factories
{
    public class PyramidFactory: IEntitiesFactory
    {
        public IEntity CreateEntity(float h, float radius, int n, Color color, bool reverseNormal)
        {
            return CreateEntity(h, radius, radius, n, 0, 0, 0, 0, 0, 0, color, reverseNormal);
        }

        public IEntity CreateEntity(float h, float radius, float radiusTop, int n, Color color, bool reverseNormal)
        {
            return CreateEntity(h, radius, radiusTop, n, 0, 0, 0, 0, 0, 0, color, reverseNormal);
        }

        public IEntity CreateEntity(float h, float radius, float radiusTop, int n, float deltaX, float deltaY, float deltaZ,
            float deltaXTop, float deltaYTop, float deltaZTop, Color color, bool reverseNormal)
        {
            var bottomPoints = GetApproximatedCircle(0, n, radius, deltaX, deltaY, deltaZ);
            var topPoints = GetApproximatedCircle(h, n, radiusTop, deltaXTop, deltaYTop, deltaZTop);

            var verticalFaces = GetFaces(topPoints, bottomPoints, color, reverseNormal);

            return new Entity(verticalFaces, topPoints, bottomPoints);
        }

        public IEntity CreateEntity(IEnumerable<IPoint> top, IEnumerable<IPoint> bottom, Color color, bool reverseNormal)
        {
            var verticalFaces = GetFaces(top, bottom, color, reverseNormal);

            return new Entity(verticalFaces, top, bottom);
        }

        private IEnumerable<IPoint> GetApproximatedCircle(float h, int n, float r, float deltaX, float deltaY, float deltaZ)
        {
            var points = new List<IPoint>();

            points.Add(new Point((int)Math.Round(r * Math.Cos(90 * Math.PI / 180)), h, (int)Math.Round(r * Math.Sin(90 * Math.PI / 180))));
            points.Add(new Point((int)Math.Round(r * Math.Cos(210 * Math.PI / 180)), h, (int)Math.Round(r * Math.Sin(210 * Math.PI / 180))));
            points.Add(new Point((int)Math.Round(r * Math.Cos(330 * Math.PI / 180)), h, (int)Math.Round(r * Math.Sin(330 * Math.PI / 180))));

            return points;
        }

        private List<IFace> GetFaces(IEnumerable<IPoint> top, IEnumerable<IPoint> bottom, Color color, bool reverseNormal)
        {
            var faces = new List<IFace>();

            var count = top.Count();

            for (int i = 0; i < count - 1; i++)
            {
                var list = new List<IPoint>
                {
                    top.ElementAt(i),
                    bottom.ElementAt(i),
                    bottom.ElementAt(i + 1),
                    top.ElementAt(i + 1)
                };

                faces.Add(new Face(list, color) { ReverseNormal = reverseNormal });
            }

            faces.Add(
                new Face(
                        new List<IPoint>
                        {
                            top.ElementAt(count - 1),
                            bottom.ElementAt(count - 1),
                            bottom.ElementAt(0),
                            top.ElementAt(0)
                        }, color)
                { ReverseNormal = reverseNormal });

            return faces;
        }
    }
}