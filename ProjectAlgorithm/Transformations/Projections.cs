using System;
using MathNet.Numerics.LinearAlgebra.Single;
using ProjectAlgorithm.Infrastructure;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Transformations;

namespace ProjectAlgorithm.Transformations
{
    public class Projections : IProjections
    {
        public ICompositeObject ViewTransformation(ICompositeObject compositeObject, float fi, float teta, float ro)
        {
            var fiAngle = MathHelper.RadiansFromAngle(fi);
            var tetaAngle = MathHelper.RadiansFromAngle(teta);
            
            var vMatrix = DenseMatrix.OfArray(new[,] {
                { -(float)Math.Sin(tetaAngle), -(float)Math.Cos(fiAngle)*(float)Math.Cos(tetaAngle), -(float)Math.Sin(fiAngle)*(float)Math.Cos(tetaAngle), 0},
                { (float)Math.Cos(tetaAngle), -(float)Math.Cos(fiAngle)*(float)Math.Sin(tetaAngle), -(float)Math.Sin(fiAngle)*(float)Math.Sin(tetaAngle), 0},
                { 0, (float)Math.Sin(fiAngle), -(float)Math.Cos(fiAngle), 0},
                { 0, 0, ro, 1 }
            });

            return compositeObject.Transform(vMatrix);
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