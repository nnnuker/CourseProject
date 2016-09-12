using System;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Entities
{
    public struct Point : IPoint
    {
        #region Fields

        private float x;
        private float y;
        private float z;

        #endregion

        #region Properties

        public float X
        {
            get { return x; }

            set { x = value; }
        }

        public float Y
        {
            get { return y; }

            set { y = value; }
        }

        public float Z
        {
            get { return z; }

            set { z = value; }
        }

        #endregion

        #region Constructor

        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        #endregion

        public object Clone()
        {
            return new Point(x, y, z);
        }
    }
}