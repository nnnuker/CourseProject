using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Interfaces.Lights
{
    public interface ILight
    {
        IPoint LightPoint { get; set; }
        int LightIntensity { get; set; }
    }
}