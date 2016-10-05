﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProjectAlgorithm;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Interfaces;
using ProjectAlgorithm.Transformations;
using Point = System.Drawing.Point;

namespace FormsPL
{
    public partial class FormPL : Form
    {
        #region Fields

        private readonly IEntitiesFactory factory = new EntitiesFactory();
        private IEntity holeEntity;
        private IEntity entity;
        private ICompositeObject compositeObject;
        private ICompositeObject currentComposite;
        private readonly ITransformation transformation = new Transformation();
        private float deltaX;
        private float deltaY;
        private bool flag;
        private Point current;
        private Pen pen = new Pen(Color.Blue);
        private Pen axisPen = new Pen(Color.Coral);
        private int axisPadding = 5;
        private Action<Graphics, Pen, ILine> DrawAction;

        #endregion

        #region Ctor

        public FormPL()
        {
            InitializeAxisPen();

            InitializeComponent();

            DrawAction = CoordinatesXY;
        }

        #endregion

        #region Drawing

        private void Draw(ICompositeObject compositeObject, Action<Graphics, Pen, ILine> DrawAction)
        {
            this.DrawAction = DrawAction;
            var bitmap = new Bitmap(drawingBox.Width, drawingBox.Height);
            ChangeDelta();
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            drawingBox.Image = bitmap;

            DrawAxis(graphics, deltaX, deltaY);

            if (compositeObject == null)
            {
                return;
            }

            foreach (var line in compositeObject.GetLines())
            {
                DrawAction(graphics, pen, line);
            }
        }

        private void DrawAxis(Graphics graphics, float deltaX, float deltaY)
        {
            graphics.DrawLine(axisPen, deltaX, drawingBox.Height - axisPadding, deltaX, axisPadding);

            graphics.DrawLine(axisPen, axisPadding, deltaY, drawingBox.Width - axisPadding, deltaY);
        }

        private void InitializeAxisPen()
        {
            axisPen.CustomEndCap = new AdjustableArrowCap(5, 5);
        }

        private void CoordinatesXY(Graphics graphics, Pen pen, ILine line)
        {
            graphics.DrawLine(pen, line.First.X + deltaX, -line.First.Y + deltaY, line.Second.X + deltaX, -line.Second.Y + deltaY);
        }

        private void CoordinatesZY(Graphics graphics, Pen pen, ILine line)
        {
            graphics.DrawLine(pen, line.First.Z + deltaX, -line.First.Y + deltaY, line.Second.Z + deltaX, -line.Second.Y + deltaY);
        }

        private void CoordinatesXZ(Graphics graphics, Pen pen, ILine line)
        {
            graphics.DrawLine(pen, line.First.X + deltaX, line.First.Z + deltaY, line.Second.X + deltaX, line.Second.Z + deltaY);
        }

        #endregion

        #region Transformations

        private void moveButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.MoveObject(compositeObject, (float)moveX.Value, (float)moveY.Value, (float)moveZ.Value);
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, CoordinatesXY);
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            entity = factory.CreateEntity((float)height.Value, (float)radius.Value, (float)radiusTop.Value, (int)number.Value);
            holeEntity = factory.CreateEntity((float)height2.Value, (float)radius2.Value, (float)radius2Top.Value, (int)number2.Value);
            compositeObject = new CompositeObject(new List<IEntity> { holeEntity, entity });
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, CoordinatesXY);
        }

        private void rotateButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.RotateObject(compositeObject, (float)rotateX.Value, (float)rotateY.Value, (float)rotateZ.Value);
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, CoordinatesXY);
        }

        private void scaleButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.ScaleObject(compositeObject, (float)scaleX.Value, (float)scaleY.Value, (float)scaleZ.Value);
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, CoordinatesXY);
        }

        #endregion

        #region Mouse rotation

        private void FormPL_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag)
            {
                float y = (e.X - current.X) / 100.0f;
                float x = (e.Y - current.Y) / 100.0f;
                currentComposite = transformation.RotateObject(currentComposite, x, y, 0);
                Draw(currentComposite, DrawAction);
            }
        }

        private void FormPL_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Middle) != 0)
            {
                flag = true;
                current = e.Location;
            }
        }

        private void FormPL_MouseUp(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Middle) != 0 && flag)
            {
                flag = false;
            }
        }

        #endregion

        #region Projections

        private void projectionButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).OrthogonalProjection(currentComposite, (float)anglePsi.Value, (float)angleFi.Value);

            Draw(currentComposite, CoordinatesXY);
        }

        private void xButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).ProjectionX(currentComposite);

            Draw(currentComposite, CoordinatesZY);
        }

        private void yButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).ProjectionY(currentComposite);

            Draw(currentComposite, CoordinatesXZ);
        }

        private void zButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).ProjectionZ(currentComposite);

            Draw(currentComposite, CoordinatesXY);
        }

        private void obliqueButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).ObliqueProjection(currentComposite, (float)angleAlpha.Value, (float)lengthOblique.Value);

            Draw(currentComposite, CoordinatesXY);
        }

        private void centralButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).CentralProjection(currentComposite, (float)distance.Value);

            Draw(currentComposite, CoordinatesXY);
        }

        #endregion

        private void ChangeDelta()
        {
            deltaY = drawingBox.Height / 2.0f;
            deltaX = drawingBox.Width / 2.0f;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Draw(currentComposite, DrawAction);
        }
    }
}
