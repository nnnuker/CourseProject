using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ProjectAlgorithm;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Interfaces;
using ProjectAlgorithm.Transformations;
using Point = ProjectAlgorithm.Entities.Point;

namespace FormsPL
{
    public partial class FormPL : Form
    {
        #region Fields

        private readonly IEntitiesFactory factory = new EntitiesFactory();
        private IEntity holeEntity;
        private IEntity entity;
        private ICompositeObject compositeObject;
        private ICompositeObject axisObject;
        private readonly ITransformation transformation = new Transformation();
        private float deltaX;
        private float deltaY;
        private bool flag;
        private System.Drawing.Point current;
        private Pen pen = new Pen(Color.Blue);

        #endregion

        #region Ctor

        public FormPL()
        {
            InitializeComponent();
        }

        #endregion

        #region Drawing

        private void Draw(ICompositeObject compositeObject, Action<Graphics, Pen, ILine> DrawAction)
        {
            ChangeDelta();
            var graphics = this.CreateGraphics();
            graphics.Clear(Color.White);

            foreach (var line in compositeObject.GetLines())
            {
                DrawAction(graphics, pen, line);
            }

            var colors = new [] {Color.Black, Color.Red, Color.Green};
            var i = 0;

            foreach (var line in axisObject.GetLines())
            {
                DrawAction(graphics, new Pen(colors[i]), line);
                i++;
            }
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
            Draw(compositeObject, CoordinatesXY);
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            entity = factory.CreateEntity((float)height.Value, (float)radius.Value, (float)radiusTop.Value, (int)number.Value);
            holeEntity = factory.CreateEntity((float)height2.Value, (float)radius2.Value, (float)radius2Top.Value, (int)number2.Value);
            compositeObject = new CompositeObject(new List<IEntity> { holeEntity, entity });
            CreateAxis();
            Draw(compositeObject, CoordinatesXY);
        }

        private void rotateButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.RotateObject(compositeObject, (float)rotateX.Value, (float)rotateY.Value, (float)rotateZ.Value);
            axisObject = transformation.RotateObject(axisObject, (float)rotateX.Value, (float)rotateY.Value, (float)rotateZ.Value);
            Draw(compositeObject, CoordinatesXY);
        }

        private void scaleButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.ScaleObject(compositeObject, (float)scaleX.Value, (float)scaleY.Value, (float)scaleZ.Value);
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
                compositeObject = transformation.RotateObject(compositeObject, x, y, 0);
                axisObject = transformation.RotateObject(axisObject, x, y, 0);
                Draw(compositeObject, CoordinatesXY);
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
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).OrthogonalProjection(composite, (float)anglePsi.Value, (float)angleFi.Value);

            Draw(composite, CoordinatesXY);
        }

        private void xButton_Click(object sender, EventArgs e)
        {
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).ProjectionX(composite);

            Draw(composite, CoordinatesZY);
        }

        private void yButton_Click(object sender, EventArgs e)
        {
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).ProjectionY(composite);

            Draw(composite, CoordinatesXZ);
        }

        private void zButton_Click(object sender, EventArgs e)
        {
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).ProjectionZ(composite);

            Draw(composite, CoordinatesXY);
        }

        private void obliqueButton_Click(object sender, EventArgs e)
        {
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).ObliqueProjection(composite, (float)angleAlpha.Value, (float)lengthOblique.Value);

            Draw(composite, CoordinatesXY);
        }

        private void centralButton_Click(object sender, EventArgs e)
        {
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).CentralProjection(composite, (float)distance.Value);

            Draw(composite, CoordinatesXY);
        }

        #endregion

        private void CreateAxis()
        {
            var length = 40;

            var start = new Point(0, 0, 0);
            var xLine = new Line(start, new Point(length, 0, 0));
            var yLine = new Line(start, new Point(0, length, 0));
            var zLine = new Line(start, new Point(0, 0, length));

            var xy = new Face(new List<ILine> {xLine, yLine});
            var xz = new Face(new List<ILine> {xLine, zLine});
            var yz = new Face(new List<ILine> {yLine, zLine});

            var axis = new Entity(new List<IFace> {xy, xz, yz});

            axisObject = new CompositeObject(new List<IEntity> {axis});
        }

        private void ChangeDelta()
        {
            deltaY = this.Height / 2.0f;
            deltaX = this.Width / 2.0f;
        }

        
    }
}
