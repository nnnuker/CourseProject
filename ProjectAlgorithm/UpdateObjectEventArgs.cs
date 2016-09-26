﻿using System;
using System.Collections.Generic;
using ProjectAlgorithm.Interfaces;

namespace ProjectAlgorithm
{
    public class UpdateObjectEventArgs:EventArgs
    {
        private readonly IList<ILine> lines;

        public IList<ILine> Lines
        {
            get { return lines; }
        }

        public UpdateObjectEventArgs()
        {
            lines = new List<ILine>();
        }

        public UpdateObjectEventArgs(IEnumerable<ILine> lines)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));

            this.lines = lines as IList<ILine> ?? new List<ILine>();
        }

        
    }
}