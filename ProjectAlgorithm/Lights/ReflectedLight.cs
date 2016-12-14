using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Lights;

namespace ProjectAlgorithm.Lights
{
    public class ReflectedLight
    {
        public Color GetColor(IPoint point, Color basic, IPoint viewPoint, ILight lightPoint, IEnumerable<float> normal, float kd, float kA, float ks, int iA, int n)
        {
            if (lightPoint == null)
            {
                return basic;
            }

            var light = GetAlpha(point, viewPoint, normal, kd, kA, ks, iA, n, lightPoint);

            light = light < 0 ? 0 : light > 255 ? 255 : light;

            var res = light / 255;

            double r = (basic.R + 255) * res;
            double g = (basic.G + 255) * res;
            double b = (basic.B + 255) * res;
            
            r = Cast(r);
            g = Cast(g);
            b = Cast(b);

            return Color.FromArgb(255, (byte)(int)r, (byte)(int)g, (byte)(int)b);
        }

        private double Cast(double val)
        {
            return val < 0 ? 0 : val > 255 ? 255 : val;
        }

        private double GetAlpha(IPoint point, IPoint viewPoint, IEnumerable<float> normal, float kd, float ka, float ks, int iA, int n, ILight light)
        {
            var a = normal;
            var b = GetViewVector(light.LightPoint.Coordinates, point.Coordinates);
            var c = GetViewVector(viewPoint.Coordinates, point.Coordinates);

            var cosTheta = GetCos(a, b);
            var cosAlpha = GetCos(b, c);

            return iA * ka + light.LightIntensity * (kd * cosTheta + ks * Math.Pow(cosAlpha, n));
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
