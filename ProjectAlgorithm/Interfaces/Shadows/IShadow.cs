using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Lights;

namespace ProjectAlgorithm.Interfaces.Shadows
{
    public interface IShadow
    {
        ICompositeObject GetShadow(ICompositeObject compositeObject, ILight lights);
    }
}