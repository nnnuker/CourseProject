using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Interfaces.Lights
{
    public interface ILightsColor
    {
        ICompositeObject ChangeColors(ICompositeObject compositeObject, float kd, float ka, int iA, params ILight[] lights);
    }
}