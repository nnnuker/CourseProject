using ProjectAlgorithm.Interfaces.Entities;
using System.Collections.Generic;
using System.Drawing;

namespace ProjectAlgorithm.Interfaces.Factories
{
    public interface IEntitiesFactory
    {
        IEntity CreateEntity(float h, float radius, int n, Color color, bool reverseNormal);

        IEntity CreateEntity(float h, float radius, float rariusTop, int n, Color color, bool reverseNormal);

        IEntity CreateEntity(float h, float radius, float rariusTop, int n,
            float deltaX, float deltaY, float deltaZ, float deltaXTop, float deltaYTop, float deltaZTop, Color color, bool reverseNormal);

        IEntity CreateEntity(IEnumerable<IPoint> top, IEnumerable<IPoint> bottom, Color color, bool reverseNormal);
    }
}