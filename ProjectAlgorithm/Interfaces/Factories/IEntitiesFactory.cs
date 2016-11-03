using ProjectAlgorithm.Interfaces.Entities;
using System.Collections.Generic;

namespace ProjectAlgorithm.Interfaces.Factories
{
    public interface IEntitiesFactory
    {
        IEntity CreateEntity(float h, float radius, int n, bool reverseNormal);

        IEntity CreateEntity(float h, float radius, float rariusTop, int n, bool reverseNormal);

        IEntity CreateEntity(float h, float radius, float rariusTop, int n,
            float deltaX, float deltaY, float deltaZ, float deltaXTop, float deltaYTop, float deltaZTop, bool reverseNormal);

        IEntity CreateEntity(IEnumerable<IPoint> top, IEnumerable<IPoint> bottom, bool reverseNormal);
    }
}