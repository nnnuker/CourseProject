using System;
using MathNet.Numerics.LinearAlgebra.Single;
using ProjectAlgorithm.Infrastructure;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Transformations;

namespace ProjectAlgorithm.Transformations
{
    public class Transform : ITransformation
    {
        public ICompositeObject ScaleObject(ICompositeObject compositeObject, float scaleX, float scaleY, float scaleZ)
        {
            var scaleMatrix = DenseMatrix.OfArray(new[,] {
                {scaleX, 0, 0, 0},
                {0, scaleY, 0, 0},
                {0, 0, scaleZ, 0},
                {0, 0, 0, 1 }
            });

            return compositeObject.Transform(scaleMatrix);
        }

        public ICompositeObject RotateObject(ICompositeObject compositeObject, float angleX, float angleY, float angleZ)
        {
            var angleRx = MathHelper.RadiansFromAngle(angleX);
            var angleRy = MathHelper.RadiansFromAngle(angleY);
            var angleRz = MathHelper.RadiansFromAngle(angleZ);

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

            return compositeObject.Transform(rotateX * rotateY * rotateZ);
        }

        public ICompositeObject MoveObject(ICompositeObject compositeObject, float moveX, float moveY, float moveZ)
        {
            var moveMatrix = DenseMatrix.OfArray(new[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 1, 0},
                { moveX, moveY, moveZ, 1}
            });

            return compositeObject.Transform(moveMatrix);
        }
    }
}