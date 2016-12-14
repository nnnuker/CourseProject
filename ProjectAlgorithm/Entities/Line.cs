using System;
using System.Drawing;
using ProjectAlgorithm.Interfaces.Entities;

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
        
        public Color Color { get; set; } 

        public bool IsHidden { get; set; }

        #endregion

        #region Constructors

        public Line()
        {
            first = second = new Point();

            Color = Color.Black;
        }

        public Line(IPoint first, IPoint second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            this.first = first;
            this.second = second;

            Color = Color.Black;
        }

        public Line(IPoint first, IPoint second, Color color, bool isHidden) : this(first, second)
        {
            this.Color = color;
            IsHidden = isHidden;
        }

        #endregion

        #region Public methods

        public object Clone()
        {
            return new Line((IPoint) first.Clone(), (IPoint) second.Clone(), Color, IsHidden);
        }
        
        #endregion
    }
}