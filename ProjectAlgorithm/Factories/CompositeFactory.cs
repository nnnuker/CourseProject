using System.Drawing;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Factories;
using ProjectAlgorithm.Entities;

namespace ProjectAlgorithm.Factories
{
    public class CompositeFactory
    {
        private readonly IEntitiesFactory entityFactory;
        private readonly Color color;
        private readonly Color colorBottom;

        public CompositeFactory()
        {
        }

        public CompositeFactory(IEntitiesFactory entityFactory)
        {
            //Check for null

            this.entityFactory = entityFactory;
        }

        public CompositeFactory(IEntitiesFactory entityFactory, Color color, Color colorBottom)
        {
            //Check for null

            this.entityFactory = entityFactory;
            this.color = color;
            this.colorBottom = colorBottom;
        }

        public ICompositeObject GetComposite(IEntity first, IEntity hole)
        {
            var top = entityFactory.CreateEntity(first.Top, hole.Top, color, false);
            var bottom = entityFactory.CreateEntity(first.Bottom, hole.Bottom, colorBottom, true);

            return new CompositeObject(new[] { hole, first, top, bottom});
        }
    }
}