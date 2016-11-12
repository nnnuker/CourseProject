using System;

namespace ProjectAlgorithm.Infrastructure
{
    public static class MathHelper
    {
        public static double RadiansFromAngle(double angle)
        {
            return angle * (Math.PI / 180.0);
        }
    }
}
