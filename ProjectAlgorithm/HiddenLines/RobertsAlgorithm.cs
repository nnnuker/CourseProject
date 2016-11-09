using System;
using System.Collections.Generic;
using System.Linq;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.HiddenLines;

namespace ProjectAlgorithm.HiddenLines
{
    public class RobertsAlgorithm : IHiddenLines
    {
        public ICompositeObject HideLines(ICompositeObject composite, IPoint viewPoint)
        {
            foreach (var compositeEntity in composite.Entities)
            {
                foreach (var face in compositeEntity.Faces)
                {
                    if (IsHidden(face, viewPoint))
                    {
                        face.IsHidden = true;
                    }
                    else
                    {
                        face.IsHidden = false;
                    }
                }
            }

            return composite;
        }

        private bool IsHidden(IFace face, IPoint viewPoint)
        {
            var a = face.Normal;
            var b = GetViewVector(viewPoint.Coordinates, face.Center);

            var cos = GetCos(a, b);

            return cos < 0;
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