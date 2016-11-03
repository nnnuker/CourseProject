using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Factories;
using ProjectAlgorithm.Entities;

namespace ProjectAlgorithm.Factories
{
    public class CompositeFactory
    {
        private IEntitiesFactory entityFactory;

        public CompositeFactory()
        {
        }

        public CompositeFactory(IEntitiesFactory entityFactory)
        {
            //Check for null

            this.entityFactory = entityFactory;
        }

        public ICompositeObject GetComposite(IEntity first, IEntity hole)
        {
            var top = entityFactory.CreateEntity(first.Top, hole.Top, false);
            var bottom = entityFactory.CreateEntity(first.Bottom, hole.Bottom, true);

            return new CompositeObject(new[] { hole, first, top, bottom});
        }
    }
}