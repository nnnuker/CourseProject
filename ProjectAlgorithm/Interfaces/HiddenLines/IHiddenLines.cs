using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Interfaces.HiddenLines
{
    public interface IHiddenLines
    {
        ICompositeObject HideLines(ICompositeObject composite, IPoint viewPoint);
    }
}