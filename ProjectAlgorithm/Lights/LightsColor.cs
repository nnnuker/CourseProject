using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Lights;

namespace ProjectAlgorithm.Lights
{
    public class LightsColor : ILightsColor
    {
        public ICompositeObject ChangeColors(ICompositeObject compositeObject, float kd, float ka, int iA, params ILight[] lights)
        {
            foreach (var compositeEntity in compositeObject.Entities)
            {
                foreach (var face in compositeEntity.Faces)
                {
                    var light = lights.Sum(l => GetAlpha(face, kd, ka, iA, l));

                    light = light < 0 ? 0 : light > 255 ? 255 : light;

                    var res = light/255;

                    double r = face.Color.R * res;
                    double g = face.Color.G * res;
                    double b = face.Color.B * res;

                    face.Color = Color.FromArgb(255, (byte)(int)r, (byte)(int)g, (byte)(int)b);
                }
            }

            return compositeObject;
        }

        private double GetAlpha(IFace face, float kd, float ka, int iA, ILight light)
        {
            var a = face.Normal;
            var b = GetViewVector(light.LightPoint.Coordinates, face.Center);

            var cos = GetCos(a, b);

            return iA * ka + light.LightIntensity * kd * cos;
        }

        private float GetCos(IEnumerable<float> a, IEnumerable<float> b)
        {
            var multiply = MultiplyVectors(a, b);
            var aLength = GetVectorLength(a);
            var bLength = GetVectorLength(b);

            return multiply / (aLength * bLength);
        }

        private float MultiplyVectors(IEnumerable<float> first, IEnumerable<float> second)
        {
            float result = 0;

            for (var i = 0; i < first.Count(); i++)
            {
                result += first.ElementAt(i) * second.ElementAt(i);
            }

            return result;
        }

        private List<float> GetViewVector(IEnumerable<float> viewPoint, IEnumerable<float> center)
        {
            var vector = new List<float>();

            for (var i = 0; i < viewPoint.Count(); i++)
            {
                vector.Add(viewPoint.ElementAt(i) - center.ElementAt(i));
            }

            return vector;
        }

        private float GetVectorLength(IEnumerable<float> vector)
        {
            var sum = vector.Aggregate(0f, (res, f) => res + (float)Math.Pow(f, 2));

            return (float)Math.Sqrt(sum);
        }
    }
}