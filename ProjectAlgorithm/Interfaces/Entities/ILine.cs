﻿using System;
using ProjectAlgorithm.Interfaces.HiddenLines;

namespace ProjectAlgorithm.Interfaces.Entities
{
    public interface ILine : IColorable, IHiddenable, ICloneable
    {
        IPoint First { get; set; }
        IPoint Second { get; set; }
    }
}