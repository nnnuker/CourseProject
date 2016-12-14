using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Single;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Infrastructure;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Lights;
using ProjectAlgorithm.Interfaces.Shadows;

namespace ProjectAlgorithm.Shadows
{
    public class Shadow : IShadow
    {
        public ICompositeObject GetShadow(ICompositeObject compositeObject, ILight light)
        {
            var composite = compositeObject.Clone() as ICompositeObject;

            var withoutHole = composite.Entities.Skip(1);

            var list = new List<Face>();

            foreach (var entity in withoutHole)
            {
                list.AddRange(entity.Faces);
            }

            foreach (var face in composite.Entities.First().Faces)
            {
                face.IsHidden = true;
            }

            foreach (Face face in list)
            {
                face.Color = GetGrayColor(face.Color);
                face.IsHidden = false;
            }

            var x = light.LightPoint.X;
            var y = light.LightPoint.Y;
            var z = Math.Abs(light.LightPoint.Z) <= 0.1 ? 0.1f : light.LightPoint.Z;

            var matrix = DenseMatrix.OfArray(new[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { -x/z, -y/z, 0, 0},
                { 0, 0, 0, 1f }
            });

            return composite.Transform(matrix);
        }

        private Color GetGrayColor(Color color)
        {
            var array = new int[] {color.R, color.G, color.B};
            var max = array.Min();
            return Color.FromArgb(70, max, max, max);
        }
    }
}