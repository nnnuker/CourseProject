using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ProjectAlgorithm.Factories;
using ProjectAlgorithm.HiddenLines;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Transformations;
using ProjectAlgorithm.Transformations;
using Point = System.Drawing.Point;

namespace FormsPL
{
    public partial class FormPL : Form
    {
        #region Fields

        private ICompositeObject compositeObject;
        private ICompositeObject currentComposite;
        private readonly ITransformation transformation = new Transformation();
        private float deltaX;
        private float deltaY;
        private bool flag;
        private Point current;
        private Pen axisPen = new Pen(Color.Coral);
        private int axisPadding = 5;
        private Action<Graphics, Pen, ILine, float, float> DrawAction;

        #endregion

        #region Ctor

        public FormPL()
        {
            InitializeAxisPen();

            InitializeComponent();

            DrawAction = CoordinatesXY;
        }

        private void InitializeAxisPen()
        {
            axisPen.CustomEndCap = new AdjustableArrowCap(5, 5);
        }

        #endregion

        #region Drawing

        private void Draw(ICompositeObject compositeObject, float deltaX, float deltaY, Action<Graphics, Pen, ILine, float, float> DrawAction)
        {
            this.DrawAction = DrawAction;
            var bitmap = new Bitmap(drawingBox.Width, drawingBox.Height);
            ChangeDelta();
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            drawingBox.Image = bitmap;

            DrawAxis(graphics, this.deltaX, this.deltaY);

            if (compositeObject == null)
                return;

            var lines = new List<ILine>();

            var pen = new Pen(Color.Black);

            foreach (var entity in compositeObject.Entities)
            {
                lines.Clear();

                foreach (var face in entity.Faces)
                {
                    if (!face.IsHidden)
                    {
                        lines.AddRange(face.Lines);
                    }
                }

                lines = lines.Distinct().ToList();

                foreach (var item in lines)
                {
                    pen.Color = item.Color;
                    DrawAction(graphics, pen, item, this.deltaX, this.deltaY);
                }
            }

            //foreach (var line in compositeObject.GetLines())
            //{
            //    DrawAction(graphics, pen, line, this.deltaX, this.deltaY);
            //}
        }

        private void DrawAxis(Graphics graphics, float deltaX, float deltaY)
        {
            graphics.DrawLine(axisPen, deltaX, drawingBox.Height - axisPadding, deltaX, axisPadding);

            graphics.DrawLine(axisPen, axisPadding, deltaY, drawingBox.Width - axisPadding, deltaY);
        }

        private void CoordinatesXY(Graphics graphics, Pen pen, ILine line, float deltaX, float deltaY)
        {
            graphics.DrawLine(pen, line.First.X + deltaX, -line.First.Y + deltaY, line.Second.X + deltaX, -line.Second.Y + deltaY);
        }

        private void CoordinatesZY(Graphics graphics, Pen pen, ILine line, float deltaX, float deltaY)
        {
            graphics.DrawLine(pen, line.First.Z + deltaX, -line.First.Y + deltaY, line.Second.Z + deltaX, -line.Second.Y + deltaY);
        }

        private void CoordinatesXZ(Graphics graphics, Pen pen, ILine line, float deltaX, float deltaY)
        {
            graphics.DrawLine(pen, line.First.X + deltaX, -line.First.Z + deltaY, line.Second.X + deltaX, -line.Second.Z + deltaY);
        }

        #endregion

        #region Transformations

        private void moveButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.MoveObject(compositeObject, (float)moveX.Value, (float)moveY.Value, (float)moveZ.Value);
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, deltaX, deltaY, CoordinatesXY);
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            var factory = new EntitiesFactory();

            var entity = factory.CreateEntity((float)height.Value, (float)radius.Value, (float)radiusTop.Value, (int)number.Value, Color.Blue, false);
            var holeEntity = factory.CreateEntity((float)height2.Value, (float)radius2.Value, (float)radius2Top.Value, (int)number2.Value, Color.Red, true);

            var compositeFactory = new CompositeFactory(factory, Color.LawnGreen, Color.Orange);

            compositeObject = compositeFactory.GetComposite(entity, holeEntity);
            currentComposite = compositeObject.Clone() as ICompositeObject;

            Draw(compositeObject, deltaX, deltaY, CoordinatesXY);
        }

        private void rotateButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.RotateObject(compositeObject, (float)rotateX.Value, (float)rotateY.Value, (float)rotateZ.Value);
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, deltaX, deltaY, CoordinatesXY);
        }

        private void scaleButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.ScaleObject(compositeObject, (float)scaleX.Value, (float)scaleY.Value, (float)scaleZ.Value);
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, deltaX, deltaY, CoordinatesXY);
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
                Draw(currentComposite, deltaX, deltaY, DrawAction);
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

            Draw(currentComposite, deltaX, deltaY, CoordinatesXY);
        }

        private void xButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).ProjectionX(currentComposite);

            Draw(currentComposite, deltaX, deltaY, CoordinatesZY);
        }

        private void yButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).ProjectionY(currentComposite);

            Draw(currentComposite, deltaX, deltaY, CoordinatesXZ);
        }

        private void zButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).ProjectionZ(currentComposite);

            Draw(currentComposite, deltaX, deltaY, CoordinatesXY);
        }

        private void obliqueButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).ObliqueProjection(currentComposite, (float)angleAlpha.Value, (float)lengthOblique.Value);

            Draw(currentComposite, deltaX, deltaY, CoordinatesXY);
        }

        private void centralButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).CentralProjection(currentComposite, (float)distance.Value);

            Draw(currentComposite, deltaX, deltaY, CoordinatesXY);
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            currentComposite = compositeObject.Clone() as ICompositeObject;
            currentComposite = ((Transformation)transformation).ViewTransformation(currentComposite, (float)angleFiView.Value, (float)angleTetaView.Value,
                (float)ro.Value, (float)distance.Value);

            Draw(currentComposite, deltaX, deltaY, CoordinatesXY);
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
            Draw(currentComposite, deltaX, deltaY, DrawAction);
        }

        private void distance_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var algorithm = new RobertsAlgorithm();

            var obj = currentComposite.Clone() as ICompositeObject;

            obj = algorithm.HideLines(obj, new ProjectAlgorithm.Entities.Point(0, 0, 1000));

            Draw(obj, 0, 0, CoordinatesXY);
        }

        private void moveX_ValueChanged(object sender, EventArgs e)
        {
            compositeObject = transformation.MoveObject(compositeObject, (float)moveX.Value, 0, 0);
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, deltaX, deltaY, CoordinatesXY);
        }

        private void moveY_ValueChanged(object sender, EventArgs e)
        {
            compositeObject = transformation.MoveObject(compositeObject, 0, (float)moveY.Value, 0);
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, deltaX, deltaY, CoordinatesXY);
        }

        private void moveZ_ValueChanged(object sender, EventArgs e)
        {
            compositeObject = transformation.MoveObject(compositeObject, 0, 0, (float)moveZ.Value);
            currentComposite = compositeObject.Clone() as ICompositeObject;
            Draw(compositeObject, deltaX, deltaY, CoordinatesXY);
        }
    }
}
