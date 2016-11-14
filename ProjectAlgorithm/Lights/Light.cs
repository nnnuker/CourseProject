using System;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Lights;

namespace ProjectAlgorithm.Lights
{
    public class Light: ILight
    {
        public IPoint LightPoint { get; set; }
        public int LightIntensity { get; set; }

        public Light()
        {
            LightPoint = new Point();
        }

        public Light(IPoint lightPoint, int lightIntensity)
        {
            if (lightPoint == null) throw new ArgumentNullException("lightPoint");
            if (lightIntensity < 0 || lightIntensity > 256) throw new ArgumentOutOfRangeException("lightIntensity");

            LightPoint = lightPoint;
            LightIntensity = lightIntensity;
        }
    }
}