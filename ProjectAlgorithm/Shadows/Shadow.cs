using System;
using System.Drawing;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Single;
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

            foreach (var face in composite.GetFaces())
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
            return Color.FromArgb(128, max, max, max);
        }
    }
}