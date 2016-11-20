﻿using ProjectAlgorithm.Interfaces.Entities;

namespace ProjectAlgorithm.Interfaces.Transformations
{
    public interface IProjections
    {
        ICompositeObject ViewTransformation(ICompositeObject compositeObject, float fi, float teta, float ro);

        ICompositeObject CentralProjection(ICompositeObject compositeObject, float distance);

        ICompositeObject ObliqueProjection(ICompositeObject compositeObject, float alpha, float l);

        ICompositeObject AxonometricProjection(ICompositeObject compositeObject, float psi, float fi);

        ICompositeObject ProjectionZ(ICompositeObject compositeObject);

        ICompositeObject ProjectionX(ICompositeObject compositeObject);

        ICompositeObject ProjectionY(ICompositeObject compositeObject);
    }
}