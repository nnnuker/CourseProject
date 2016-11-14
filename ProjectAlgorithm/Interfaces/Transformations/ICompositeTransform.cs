using ProjectAlgorithm.Interfaces.HiddenLines;
using ProjectAlgorithm.Interfaces.Lights;

namespace ProjectAlgorithm.Interfaces.Transformations
{
    public interface ICompositeTransform : ITransformation, IProjections, IHiddenLines, ILightsColor
    {
    }
}