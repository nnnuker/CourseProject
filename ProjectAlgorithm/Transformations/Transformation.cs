﻿using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Transformations
{
    public class Transformation : ITransformation
    {
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
            var angleRx = (angleX * (Math.PI / 180.0));
            var angleRy = (angleY * (Math.PI / 180.0));
            var angleRz = (angleZ * (Math.PI / 180.0));

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

        public ICompositeObject Proection(ICompositeObject compositeObject, float psi, float fi)
        {
            var anglePsi = (psi * (Math.PI / 180.0));
            var angleFi = (fi * (Math.PI / 180.0));

            var proection = DenseMatrix.OfArray(new[,] {
                                                    { (float)Math.Cos(anglePsi), (float)Math.Sin(anglePsi)*(float)Math.Sin(angleFi), 0, 0},
                                                    { 0, (float)Math.Cos(angleFi), 0, 0},
                                                    { (float)Math.Sin(anglePsi), -(float)Math.Sin(angleFi)*(float)Math.Cos(anglePsi), 0, 0},
                                                    { 0, 0, 0, 1 }
            });

            return Transform(compositeObject, proection);
        }

        public ICompositeObject ProectionZ(ICompositeObject compositeObject)
        {
            var proection = DenseMatrix.OfArray(new[,] {
                                                    { 1.0f, 0, 0, 0},
                                                    { 0, 1.0f, 0, 0},
                                                    { 0, 0, 0, 0},
                                                    { 0, 0, 0, 1.0f }
            });

            return Transform(compositeObject, proection);
        }

        public ICompositeObject ProectionX(ICompositeObject compositeObject)
        {
            var proection = DenseMatrix.OfArray(new[,] {
                                                    { 0, 0, 0, 0},
                                                    { 0, 1, 0, 0},
                                                    { 0, 0, 1, 0},
                                                    { 0, 0, 0, 1.0f }
            });

            return Transform(compositeObject, proection);
        }

        public ICompositeObject ProectionY(ICompositeObject compositeObject)
        {
            var proection = DenseMatrix.OfArray(new[,] {
                                                    { 1, 0, 0, 0},
                                                    { 0, 0, 0, 0},
                                                    { 0, 0, 1, 0},
                                                    { 0, 0, 0, 1.0f }
            });

            return Transform(compositeObject, proection);
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

        private Vector<float> Vector(IPoint point)
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