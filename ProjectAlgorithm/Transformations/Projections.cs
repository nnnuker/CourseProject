using System;
using MathNet.Numerics.LinearAlgebra.Single;
using ProjectAlgorithm.Infrastructure;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Transformations;

namespace ProjectAlgorithm.Transformations
{
    public class Projections : IProjections
    {
        public ICompositeObject ViewTransformation(ICompositeObject compositeObject, float fi, float teta, float ro, float distance)
        {
            var fiAngle = MathHelper.RadiansFromAngle(fi);
            var tetaAngle = MathHelper.RadiansFromAngle(teta);

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
                { -(float)Math.Sin(tetaAngle), -(float)Math.Cos(fiAngle)*(float)Math.Cos(tetaAngle), -(float)Math.Sin(fiAngle)*(float)Math.Cos(tetaAngle), 0},
                { (float)Math.Cos(tetaAngle), -(float)Math.Cos(fiAngle)*(float)Math.Sin(tetaAngle), -(float)Math.Sin(fiAngle)*(float)Math.Sin(tetaAngle), 0},
                { 0, (float)Math.Sin(fiAngle), -(float)Math.Cos(fiAngle), 0},
                { 0, 0, ro, 1 }
            });
            //var vMatrix = tMatrix * rzMatrix * rxMatrix * sMatrix;

            return CentralProjection(compositeObject.Transform(vMatrix), distance);
        }

        public ICompositeObject ObliqueProjection(ICompositeObject compositeObject, float alpha, float l)
        {
            var angleAlpha = MathHelper.RadiansFromAngle(alpha);

            var projection = DenseMatrix.OfArray(new[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { -l*(float)Math.Cos(angleAlpha), -l*(float)Math.Sin(angleAlpha), 0, 0},
                { 0, 0, 0, 1 }
            });

            return compositeObject.Transform(projection);
        }

        public ICompositeObject CentralProjection(ICompositeObject compositeObject, float distance)
        {
            var points = compositeObject.GetPoints();
            var param = 0.1f;

            foreach (var point in points)
            {
                point.Z = Math.Abs(point.Z) <= 0.1f ? param : point.Z;
                point.X = point.X * distance / point.Z;
                point.Y = point.Y * distance / point.Z;
                point.Z = distance;
            }

            return compositeObject;
        }

        public ICompositeObject AxonometricProjection(ICompositeObject compositeObject, float psi, float fi)
        {
            var anglePsi = MathHelper.RadiansFromAngle(psi);
            var angleFi = MathHelper.RadiansFromAngle(fi);

            var projection = DenseMatrix.OfArray(new[,] {
                { (float)Math.Cos(anglePsi), (float)Math.Sin(anglePsi)*(float)Math.Sin(angleFi), 0, 0},
                { 0, (float)Math.Cos(angleFi), 0, 0},
                { (float)Math.Sin(anglePsi), -(float)Math.Sin(angleFi)*(float)Math.Cos(anglePsi), 0, 0},
                { 0, 0, 0, 1 }
            });

            return compositeObject.Transform(projection);
        }

        public ICompositeObject ProjectionZ(ICompositeObject compositeObject)
        {
            var projection = DenseMatrix.OfArray(new[,] {
                { 1.0f, 0, 0, 0},
                { 0, 1.0f, 0, 0},
                { 0, 0, 0, 0},
                { 0, 0, 0, 1.0f }
            });

            return compositeObject.Transform(projection);
        }

        public ICompositeObject ProjectionX(ICompositeObject compositeObject)
        {
            var projection = DenseMatrix.OfArray(new[,] {
                { 0, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1.0f }
            });

            return compositeObject.Transform(projection);
        }

        public ICompositeObject ProjectionY(ICompositeObject compositeObject)
        {
            var projection = DenseMatrix.OfArray(new[,] {
                { 1, 0, 0, 0},
                { 0, 0, 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1.0f }
            });

            return compositeObject.Transform(projection);
        }
    }
}