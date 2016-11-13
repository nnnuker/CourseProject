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
    public class EntitiesFactory : IEntitiesFactory
    {
        #region Public methods

        public IEntity CreateEntity(float h, float radius, int n, Color color, bool reverseNormal = false)
        {
            return CreateEntity(h, radius, radius, n, 0, 0, 0, 0, 0, 0, color, reverseNormal);
        }

        public IEntity CreateEntity(float h, float radius, float radiusTop, int n, Color color, bool reverseNormal = false)
        {
            return CreateEntity(h, radius, radiusTop, n, 0, 0, 0, 0, 0, 0, color, reverseNormal);
        }

        public IEntity CreateEntity(float h, float radius, float radiusTop, int n, float deltaX, float deltaY, float deltaZ, 
            float deltaXTop, float deltaYTop, float deltaZTop, Color color, bool reverseNormal = false)
        {
            if (h == 0 || (radius == 0 && radiusTop == 0))
            {
                return new Entity();
            }

            var bottomPoints = GetApproximatedCircle(0, n, radius, deltaX, deltaY, deltaZ);
            var topPoints = GetApproximatedCircle(h, n, radiusTop, deltaXTop, deltaYTop, deltaZTop);

            var verticalFaces = GetFaces(topPoints, bottomPoints, color, reverseNormal);

            return new Entity(verticalFaces, topPoints, bottomPoints);
        }

        public IEntity CreateEntity(IEnumerable<IPoint> top, IEnumerable<IPoint> bottom, Color color, bool reverseNormal = false)
        {
            var verticalFaces = GetFaces(top, bottom, color, reverseNormal);

            return new Entity(verticalFaces, top, bottom);
        }

        #endregion

        #region Private methods

        private List<IPoint> GetApproximatedCircle(float h, int n, float r, float deltaX, float deltaY, float deltaZ)
        {
            var alpha = 360.0 / n;
            double grad = 0.0;
            var points = new List<IPoint>();

            for (var i = 0; i < n; i++)
            {
                var z = (float)Math.Round(r * Math.Sin(grad * Math.PI / 180));
                var x = (float)Math.Round(r * Math.Cos(grad * Math.PI / 180));
                points.Add(new Point(x + deltaX, h + deltaY, z + deltaZ));
                grad += alpha;
            }

            return points;
        }

        private List<IFace> GetFaces(IEnumerable<IPoint> top, IEnumerable<IPoint> bottom, Color color, bool reverseNormal)
        {
            var faces = new List<IFace>();

            var count = top.Count();
            var bottomCount = bottom.Count();

            if (count != bottomCount || bottomCount == 0)
            {
                faces.Add(new Face(top, color) {ReverseNormal = !reverseNormal});
                return faces;
            }

            for (var i = 0; i < count - 1; i++)
            {
                var list = new List<IPoint>
                {
                    top.ElementAt(i),
                    bottom.ElementAt(i),
                    bottom.ElementAt(i + 1),
                    top.ElementAt(i + 1)
                };

                faces.Add(new Face(list, color) { ReverseNormal = reverseNormal});
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
                    { ReverseNormal = reverseNormal});

            return faces;
        }

        #endregion
    }
}