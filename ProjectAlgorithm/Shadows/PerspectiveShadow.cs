using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;
using ProjectAlgorithm.Infrastructure;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Lights;
using ProjectAlgorithm.Interfaces.Shadows;

namespace ProjectAlgorithm.Shadows
{
    public class PerspectiveShadow:IShadow
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
            var z = light.LightPoint.Z;

            var matrix = DenseMatrix.OfArray(new[,] {
                                                    { z, 0, 0, 0},
                                                    { 0, z, 0, 0},
                                                    { -x, -y, 0, 1},
                                                    { 0, 0, 0, z}
                                                  });

            return composite.Transform(matrix);
        }

        private Color GetGrayColor(Color color)
        {
            var array = new int[] { color.R, color.G, color.B };
            var max = array.Min();
            return Color.FromArgb(128, max, max, max);
        }

    }
}
