using System;

namespace ProjectAlgorithm.Interfaces
{
    public interface ICompositeChangeable
    {
        event EventHandler<UpdateObjectEventArgs> OnChange;
    }
}