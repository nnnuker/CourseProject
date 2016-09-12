using ProjectAlgorithm.Entities;

namespace ProjectAlgorithm
{
    public interface IEntitiesFactory
    {
        Entity CreateEntity(float h, float radius, int n);

        Entity CreateEntity(float h, float radius, float rariusTop, int n);

        Entity CreateEntity(float h, float radius, float rariusTop, int n,
            float deltaX, float deltaY, float deltaZ, float deltaXTop, float deltaYTop, float deltaZTop);
    }
}