using System;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Infrastructure
{
    public class ViewPointsHelper
    {
        public static IPoint GetViewPoint(float fi, float teta, float ro)
        {
            var fiAngle = MathHelper.RadiansFromAngle(fi);
            var tetaAngle = MathHelper.RadiansFromAngle(teta);

            var xE = (float)(ro * Math.Sin(fiAngle) * Math.Cos(tetaAngle));
            var yE = (float)(ro * Math.Sin(fiAngle) * Math.Sin(tetaAngle));
            var zE = (float)(ro * Math.Cos(fiAngle));

            return new Point(xE, yE, zE);
        }
    }
}
