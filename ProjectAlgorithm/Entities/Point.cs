using System.Collections.Generic;
using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Entities
{
    public class Point : IPoint
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

        public IEnumerable<float> Coordinates { get { return new[] {x, y, z}; } }

        #endregion

        #region Constructor

        public Point()
        {
        }

        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        #endregion

        #region Public methods

        public object Clone()
        {
            return new Point(x, y, z);
        }
        
        #endregion
    }
}