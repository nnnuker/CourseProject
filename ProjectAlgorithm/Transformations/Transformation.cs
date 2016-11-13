﻿using System;
using ProjectAlgorithm.HiddenLines;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.HiddenLines;
using ProjectAlgorithm.Interfaces.Transformations;

namespace ProjectAlgorithm.Transformations
{
    public class Transformation : ICompositeTransform
    {
        private readonly IHiddenLines hiddenLines;
        private readonly ITransformation transform;
        private readonly IProjections projections;

        public Transformation()
        {
            projections = new Projections();
            transform = new Transform();
            hiddenLines = new RobertsAlgorithm();
        }

        public Transformation(IHiddenLines hiddenLines, ITransformation transform, IProjections projections)
        {
            if (hiddenLines == null) throw new ArgumentNullException("hiddenLines");
            if (transform == null) throw new ArgumentNullException("transform");
            if (projections == null) throw new ArgumentNullException("projections");

            this.hiddenLines = hiddenLines;
            this.transform = transform;
            this.projections = projections;
        }

        #region Transformations

        public ICompositeObject ScaleObject(ICompositeObject compositeObject, float scaleX, float scaleY, float scaleZ)
        {
            return transform.ScaleObject(compositeObject, scaleX, scaleY, scaleZ);
        }

        public ICompositeObject RotateObject(ICompositeObject compositeObject, float angleX, float angleY, float angleZ)
        {
            return transform.RotateObject(compositeObject, angleX, angleY, angleZ);
        }

        public ICompositeObject MoveObject(ICompositeObject compositeObject, float moveX, float moveY, float moveZ)
        {
            return transform.MoveObject(compositeObject, moveX, moveY, moveZ);
        }

        #endregion

        #region Projections

        public ICompositeObject ViewTransformation(ICompositeObject compositeObject, float fi, float teta, float ro, float distance)
        {
            return projections.ViewTransformation(compositeObject, fi, teta, ro, distance);
        }

        public ICompositeObject ObliqueProjection(ICompositeObject compositeObject, float alpha, float l)
        {
            return projections.ObliqueProjection(compositeObject, alpha, l);
        }

        public ICompositeObject CentralProjection(ICompositeObject compositeObject, float distance)
        {
            return projections.CentralProjection(compositeObject, distance);
        }

        public ICompositeObject AxonometricProjection(ICompositeObject compositeObject, float psi, float fi)
        {
            return projections.AxonometricProjection(compositeObject, psi, fi);
        }

        public ICompositeObject ProjectionZ(ICompositeObject compositeObject)
        {
            return projections.ProjectionZ(compositeObject);
        }

        public ICompositeObject ProjectionX(ICompositeObject compositeObject)
        {
            return projections.ProjectionX(compositeObject);
        }

        public ICompositeObject ProjectionY(ICompositeObject compositeObject)
        {
            return projections.ProjectionY(compositeObject);
        }

        #endregion

        #region Hidden lines

        public ICompositeObject HideLines(ICompositeObject composite, IPoint viewPoint)
        {
            return hiddenLines.HideLines(composite, viewPoint);
        }

        #endregion
    }
}