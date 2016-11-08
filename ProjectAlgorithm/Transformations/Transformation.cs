using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.HiddenLines;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.HiddenLines;
using ProjectAlgorithm.Interfaces.Transformations;

namespace ProjectAlgorithm.Transformations
{
    public class Transformation : ITransformation, IProjections, IHiddenLines
    {
        private readonly IHiddenLines hiddenLines;

        public Transformation()
        {
            hiddenLines = new RobertsAlgorithm();
        }

        public Transformation(IHiddenLines hiddenLines)
        {
            if (hiddenLines == null)
            {
                throw new ArgumentNullException("hiddenLines");
            }

            this.hiddenLines = hiddenLines;
        }

        #region Transformations

        public ICompositeObject ScaleObject(ICompositeObject compositeObject, float scaleX, float scaleY, float scaleZ)
        {
            var scaleMatrix = DenseMatrix.OfArray(new[,] {
                {scaleX, 0, 0, 0},
                {0, scaleY, 0, 0},
                {0, 0, scaleZ, 0},
                {0, 0, 0, 1 }
            });
            return Transform(compositeObject, scaleMatrix);
        }

        public ICompositeObject RotateObject(ICompositeObject compositeObject, float angleX, float angleY, float angleZ)
        {
            var angleRx = RadiansFromAngle(angleX);
            var angleRy = RadiansFromAngle(angleY);
            var angleRz = RadiansFromAngle(angleZ);

            var rotateZ = DenseMatrix.OfArray(new[,] {
                { (float)Math.Cos(angleRz), (float)Math.Sin(angleRz), 0, 0},
                { -(float)Math.Sin(angleRz), (float)Math.Cos(angleRz), 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1 }
            });

            var rotateX = DenseMatrix.OfArray(new[,] {
                { 1, 0, 0, 0 },
                { 0, (float)Math.Cos(angleRx), (float)Math.Sin(angleRx), 0 },
                { 0, -(float)Math.Sin(angleRx), (float)Math.Cos(angleRx), 0 },
                { 0, 0, 0, 1 }
            });

            var rotateY = DenseMatrix.OfArray(new[,] {
                { (float)Math.Cos(angleRy), 0, -(float)Math.Sin(angleRy), 0 },
                { 0, 1, 0, 0 },
                { (float)Math.Sin(angleRy),0, (float)Math.Cos(angleRy),0 },
                { 0, 0, 0, 1 }
            });

            return Transform(compositeObject, (rotateX * rotateY * rotateZ));
        }

        public ICompositeObject MoveObject(ICompositeObject compositeObject, float moveX, float moveY, float moveZ)
        {
            
            var moveMatrix = DenseMatrix.OfArray(new[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 1, 0},
                { moveX, moveY, moveZ, 1}
            });

            return Transform(compositeObject, moveMatrix);
        }

        #endregion

        #region Projections

        public ICompositeObject ViewTransformation(ICompositeObject compositeObject, float fi, float teta, float ro, float distance)
        {
            var fiAngle = RadiansFromAngle(fi);
            var tetaAngle = RadiansFromAngle(teta);

            //var xE = (float)(ro * Math.Sin(fiAngle) * Math.Cos(tetaAngle));
            //var yE = (float)(ro * Math.Sin(fiAngle) * Math.Sin(tetaAngle));
            //var zE = (float)(ro * Math.Cos(fiAngle));

            //var tMatrix = DenseMatrix.OfArray(new[,] {
            //    { 1, 0, 0, 0},
            //    { 0, 1, 0, 0},
            //    { 0, 0, 1, 0},
            //    { -xE, -yE, -zE, 1 }
            //});

            //var rzMatrix = DenseMatrix.OfArray(new[,] {
            //    { (float)Math.Cos(Math.PI/2 - tetaAngle), (float)Math.Sin(Math.PI/2 - tetaAngle), 0, 0},
            //    { -(float)Math.Sin(Math.PI/2 - tetaAngle), (float)Math.Cos(Math.PI/2 - tetaAngle), 0, 0},
            //    { 0, 0, 1, 0},
            //    { 0, 0, 0, 1 }
            //});

            //var rxMatrix = DenseMatrix.OfArray(new[,] {
            //    { 1, 0, 0, 0},
            //    { 0, (float)Math.Cos(fiAngle - Math.PI), (float)Math.Sin(fiAngle - Math.PI), 0},
            //    { 0, -(float)Math.Sin(fiAngle - Math.PI), (float)Math.Cos(fiAngle - Math.PI), 0},
            //    { 0, 0, 0, 1 }
            //});

            //var sMatrix = DenseMatrix.OfArray(new[,] {
            //    { 1.0f, 0, 0, 0},
            //    { 0, 1, 0, 0},
            //    { 0, 0, -1, 0},
            //    { 0, 0, 0, 1 }
            //});

            var vMatrix = DenseMatrix.OfArray(new[,] {
                { (float)Math.Cos(tetaAngle), (float)Math.Cos(fiAngle)*(float)Math.Sin(tetaAngle), (float)Math.Sin(fiAngle)*(float)Math.Sin(tetaAngle), 0},
                { (float)Math.Sin(tetaAngle), (float)Math.Cos(fiAngle)*(float)Math.Cos(tetaAngle), (float)Math.Sin(fiAngle)*(float)Math.Cos(tetaAngle), 0},
                { 0, (float)Math.Sin(fiAngle), (float)Math.Cos(fiAngle), 0},
                { 0, 0, ro, 1 }
            });
            //var vMatrix = tMatrix * rzMatrix * rxMatrix * sMatrix;

            return CentralProjection(Transform(compositeObject, vMatrix), distance);
        }

        public ICompositeObject ObliqueProjection(ICompositeObject compositeObject, float alpha, float l)
        {
            var angleAlpha = RadiansFromAngle(alpha);
            
            var projection = DenseMatrix.OfArray(new[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { l*(float)Math.Cos(angleAlpha), l*(float)Math.Sin(angleAlpha), 0, 0},
                { 0, 0, 0, 1 }
            });

            return Transform(compositeObject, projection);
        }

        public ICompositeObject CentralProjection(ICompositeObject compositeObject, float distance)
        {
            //var projection = DenseMatrix.OfArray(new[,] {
            //                                        { 1, 0, 0, 0},
            //                                        { 0, 1, 0, 0},
            //                                        { 0, 0, 1, 1/distance},
            //                                        { 0, 0, 0, 0 }
            //});

            var lines = compositeObject.GetLines();

            foreach (var line in lines)
            {
                line.First = CentralPoints(line.First, distance);

                line.Second = CentralPoints(line.Second, distance);
            }

            return compositeObject;

            //return Transform(compositeObject, projection);
        }

        public ICompositeObject OrthogonalProjection(ICompositeObject compositeObject, float psi, float fi)
        {
            var anglePsi = RadiansFromAngle(psi);
            var angleFi = RadiansFromAngle(fi);

            var projection = DenseMatrix.OfArray(new[,] {
                { (float)Math.Cos(anglePsi), (float)Math.Sin(anglePsi)*(float)Math.Sin(angleFi), 0, 0},
                { 0, (float)Math.Cos(angleFi), 0, 0},
                { (float)Math.Sin(anglePsi), -(float)Math.Sin(angleFi)*(float)Math.Cos(anglePsi), 0, 0},
                { 0, 0, 0, 1 }
            });

            return Transform(compositeObject, projection);
        }

        public ICompositeObject ProjectionZ(ICompositeObject compositeObject)
        {
            var projection = DenseMatrix.OfArray(new[,] {
                { 1.0f, 0, 0, 0},
                { 0, 1.0f, 0, 0},
                { 0, 0, 0, 0},
                { 0, 0, 0, 1.0f }
            });

            return Transform(compositeObject, projection);
        }

        public ICompositeObject ProjectionX(ICompositeObject compositeObject)
        {
            var projection = DenseMatrix.OfArray(new[,] {
                { 0, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1.0f }
            });

            return Transform(compositeObject, projection);
        }

        public ICompositeObject ProjectionY(ICompositeObject compositeObject)
        {
            var projection = DenseMatrix.OfArray(new[,] {
                { 1, 0, 0, 0},
                { 0, 0, 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1.0f }
            });

            return Transform(compositeObject, projection);
        }

        #endregion

        #region Hidden lines

        public ICompositeObject HideLines(ICompositeObject composite, IPoint viewPoint)
        {
            return hiddenLines.HideLines(composite, viewPoint);
        }

        #endregion

        #region Private

        private IPoint CentralPoints(IPoint point, float distance)
        {
            var point1 = new Point();
            var param = 0.1f;
            point1.Z = Math.Abs(point.Z) <= 0.1f ? param : point.Z;
            point1.X = point.X * distance / point1.Z;
            point1.Y = point.Y * distance / point1.Z;
            point1.Z = distance;

            return point1;
        }

        private ICompositeObject Transform(ICompositeObject compositeObject, DenseMatrix matrix)
        {
            var lines = compositeObject.GetLines();

            foreach (var line in lines)
            {
                line.First = PointOutVector(Vector(line.First) * matrix);
                line.Second = PointOutVector(Vector(line.Second) * matrix);
            }

            return compositeObject;
        }

        private static double RadiansFromAngle(double angle)
        {
            return (angle * (Math.PI / 180.0));
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

        #endregion
    }
}