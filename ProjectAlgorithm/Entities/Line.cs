using System;
using System.Collections.Generic;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm.Entities
{
    public class Line : ILine
    {
        #region Fields

        private IPoint first;
        private IPoint second;

        #endregion

        #region Properties

        public IPoint First
        {
            get { return first; }
            set { first = value; }
        }

        public IPoint Second
        {
            get { return second; }
            set { second = value; }
        }

        #endregion

        #region Constructors

        public Line()
        {
            first = second = new Point();
        }

        public Line(IPoint first, IPoint second)
        {
            this.first = first;
            this.second = second;
        }

        #endregion

        public object Clone()
        {
            return new Line((IPoint)first.Clone(), (IPoint)second.Clone());
        }
    }
}