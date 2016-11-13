using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Infrastructure
{
    public static class TransformHelper
    {
        public static ICompositeObject Transform(this ICompositeObject compositeObject, DenseMatrix matrix)
        {
            var points = compositeObject.GetPoints();

            foreach (var point in points)
            {
                var p = PointOutVector(Vector(point) * matrix);

                point.X = p.X;
                point.Y = p.Y;
                point.Z = p.Z;
            }

            return compositeObject;
        }

        private static Vector<float> Vector(IPoint point)
        {
            if (point == null) throw new ArgumentNullException("point");

            return Vector<float>.Build.DenseOfArray(new[] { point.X, point.Y, point.Z, 1 });
        }

        private static IPoint PointOutVector(IList<float> vector)
        {
            return new Point
            {
                X = vector[0],
                Y = vector[1],
                Z = vector[2]
            };
        }
    }
}