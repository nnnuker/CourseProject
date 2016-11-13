using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ProjectAlgorithm.Factories;
using ProjectAlgorithm.Infrastructure;
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
        private readonly ICompositeTransform transformation = new Transformation();
        private float deltaX;
        private float deltaY;
        private bool flag;
        private Point current;
        private readonly Pen axisPenX = new Pen(Color.Coral);
        private readonly Pen axisPenY = new Pen(Color.Coral);
        private readonly int axisPadding = 5;
        private Action<Graphics, Pen, Brush, IFace, float, float> DrawAction;
        private Action ProjectionAction;
        private bool hideLines = true;
        private bool fillFaces = true;
        private IPoint viewPoint;

        private readonly Dictionary<string, IPoint> viewPoints = new Dictionary<string, IPoint>
        {
            {"xOy", new ProjectAlgorithm.Entities.Point(0, 0, 10000)},
            {"xOz", new ProjectAlgorithm.Entities.Point(0, 10000, 0)},
            {"yOz", new ProjectAlgorithm.Entities.Point(10000, 0, 0)}
        };

        #endregion

        #region Ctor

        public FormPL()
        {
            InitializeAxisPen(false, false);

            InitializeComponent();

            DrawAction = CoordinatesXY;
            viewPoint = viewPoints["xOy"];

            drawButton_Click(null, null);
        }

        private void InitializeAxisPen(bool invertX, bool invertY)
        {
            var customEndCap = new AdjustableArrowCap(5, 5);

            if (invertX)
            {
                axisPenX.CustomStartCap = customEndCap;
                axisPenX.EndCap = LineCap.Flat;
            }
            else
            {
                axisPenX.CustomEndCap = customEndCap;
                axisPenX.StartCap = LineCap.Flat;
            }

            if (invertY)
            {
                axisPenY.CustomStartCap = customEndCap;
                axisPenY.EndCap = LineCap.Flat;
            }
            else
            {
                axisPenY.CustomEndCap = customEndCap;
                axisPenY.StartCap = LineCap.Flat;
            }
        }

        private void InitializeComposite()
        {
            var factory = new EntitiesFactory();

            var entity = factory.CreateEntity((float)height.Value, (float)radius.Value, (float)radiusTop.Value, (int)number.Value, Color.Blue, true);
            var holeEntity = factory.CreateEntity((float)height2.Value, (float)radius2.Value, (float)radius2Top.Value, (int)number2.Value, Color.Red, false);

            var compositeFactory = new CompositeFactory(factory, Color.Blue, Color.Blue);

            compositeObject = compositeFactory.GetComposite(entity, holeEntity);
            currentComposite = compositeObject.Clone() as ICompositeObject;
        }

        #endregion

        #region Drawing

        private void drawButton_Click(object sender, EventArgs e)
        {
            InitializeComposite();

            ProjectionAction = () =>
            {
                viewPoint = viewPoints["xOy"];
                Draw(compositeObject, deltaX, deltaY, CoordinatesXY);
            };

            ProjectionAction();
        }

        private void Draw(ICompositeObject composite, float deltaX, float deltaY, Action<Graphics, Pen, Brush, IFace, float, float> DrawAction)
        {
            this.DrawAction = DrawAction;

            var bitmap = new Bitmap(drawingBox.Width, drawingBox.Height);
            var graphics = Graphics.FromImage(bitmap);
            drawingBox.Image = bitmap;

            ChangeDelta();

            DrawAxis(graphics, this.deltaX, this.deltaY);

            if (composite == null)
                return;

            if (hideLines)
            {
                composite = transformation.HideLines(composite, viewPoint);
            }

            var pen = new Pen(Color.Aquamarine);
            var brush = new SolidBrush(Color.Black);

            foreach (var entity in composite.Entities)
            {
                foreach (var face in entity.Faces)
                {
                    if (!face.IsHidden || !hideLines)
                    {
                        brush.Color = face.Color;
                        DrawAction(graphics, pen, brush, face, this.deltaX, this.deltaY);
                    }
                }
            }
        }

        private void DrawAxis(Graphics graphics, float deltaX, float deltaY)
        {
            graphics.DrawLine(axisPenY, deltaX, drawingBox.Height - axisPadding, deltaX, axisPadding);

            graphics.DrawLine(axisPenX, axisPadding, deltaY, drawingBox.Width - axisPadding, deltaY);
        }

        private void CoordinatesXY(Graphics graphics, Pen pen, Brush brush, IFace face, float deltaX, float deltaY)
        {
            var points = face.Points.Select(p => new PointF(p.X + deltaX, -p.Y + deltaY)).ToArray();
            DrawPolygons(graphics, pen, brush, points);
        }

        private void CoordinatesZY(Graphics graphics, Pen pen, Brush brush, IFace face, float deltaX, float deltaY)
        {
            var points = face.Points.Select(p => new PointF(p.Z + deltaX, -p.Y + deltaY)).ToArray();
            DrawPolygons(graphics, pen, brush, points);
        }

        private void CoordinatesXZ(Graphics graphics, Pen pen, Brush brush, IFace face, float deltaX, float deltaY)
        {
            var points = face.Points.Select(p => new PointF(p.X + deltaX, p.Z + deltaY)).ToArray();
            DrawPolygons(graphics, pen, brush, points);
        }

        private void DrawPolygons(Graphics graphics, Pen pen, Brush brush, PointF[] points)
        {
            if (fillFaces)
            {
                graphics.FillPolygon(brush, points);
            }

            if (pen != null)
            {
                graphics.DrawPolygon(pen, points);
            }
        }

        #endregion

        #region Transformations

        private void TransformObject(float x, float y, float z, Func<ICompositeObject, float, float, float, ICompositeObject> Func)
        {
            compositeObject = Func(compositeObject, x, y, z);
            currentComposite = compositeObject.Clone() as ICompositeObject;

            ProjectionAction();
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            TransformObject((float)moveX.Value, (float)moveY.Value, (float)moveZ.Value, transformation.MoveObject);
        }

        #region Move values changed

        private void moveX_ValueChanged(object sender, EventArgs e)
        {
            TransformObject((float) moveX.Value, 0, 0, transformation.MoveObject);
        }

        private void moveY_ValueChanged(object sender, EventArgs e)
        {
            TransformObject(0, (float)moveY.Value, 0, transformation.MoveObject);
        }

        private void moveZ_ValueChanged(object sender, EventArgs e)
        {
            TransformObject(0, 0, (float)moveZ.Value, transformation.MoveObject);
        }

        #endregion

        private void rotateButton_Click(object sender, EventArgs e)
        {
            TransformObject((float)rotateX.Value, (float)rotateY.Value, (float)rotateZ.Value, transformation.RotateObject);
        }

        #region Rotate values changed

        private void rotateX_ValueChanged(object sender, EventArgs e)
        {
            TransformObject((float)rotateX.Value, 0, 0, transformation.RotateObject);
        }

        private void rotateY_ValueChanged(object sender, EventArgs e)
        {
            TransformObject(0, (float)rotateY.Value, 0, transformation.RotateObject);
        }

        private void rotateZ_ValueChanged(object sender, EventArgs e)
        {
            TransformObject(0, 0, (float)rotateZ.Value, transformation.RotateObject);
        }

        #endregion

        private void scaleButton_Click(object sender, EventArgs e)
        {
            TransformObject((float)scaleX.Value, (float)scaleY.Value, (float)scaleZ.Value, transformation.ScaleObject);
        }

        #region Scale values changed

        private void scaleX_ValueChanged(object sender, EventArgs e)
        {
            TransformObject((float)scaleX.Value, 1, 1, transformation.ScaleObject);
        }

        private void scaleY_ValueChanged(object sender, EventArgs e)
        {
            TransformObject(1, (float)scaleY.Value, 1, transformation.ScaleObject);
        }

        private void scaleZ_ValueChanged(object sender, EventArgs e)
        {
            TransformObject((float)scaleX.Value, 1, 1, transformation.ScaleObject);
        }

        #endregion

        #endregion

        #region Mouse rotation

        private void FormPL_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag)
            {
                float y = (e.X - current.X) / 100.0f;
                float x = (e.Y - current.Y) / 100.0f;

                TransformObject(x, y, 0, transformation.RotateObject);
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
            ProjectionAction = () =>
            {
                currentComposite = compositeObject.Clone() as ICompositeObject;
                currentComposite = transformation.AxonometricProjection(currentComposite, (float)anglePsi.Value, (float)angleFi.Value);

                InitializeAxisPen(false, false);

                viewPoint = viewPoints["xOy"];

                Draw(currentComposite, deltaX, deltaY, CoordinatesXY);
            };

            ProjectionAction();
        }

        private void OrthoProjection(Func<ICompositeObject, ICompositeObject> Func, IPoint viewPoint, bool invertX, bool invertY, Action<Graphics, Pen, Brush, IFace, float, float> CoordAction)
        {
            ProjectionAction = () =>
            {
                currentComposite = compositeObject.Clone() as ICompositeObject;
                currentComposite = Func(currentComposite);

                InitializeAxisPen(invertX, invertY);

                this.viewPoint = viewPoint;

                Draw(currentComposite, deltaX, deltaY, CoordAction);
            };

            ProjectionAction();
        }

        private void xButton_Click(object sender, EventArgs e)
        {
            OrthoProjection(transformation.ProjectionX, viewPoints["yOz"], false, false, CoordinatesZY);
        }

        private void yButton_Click(object sender, EventArgs e)
        {
            OrthoProjection(transformation.ProjectionY, viewPoints["xOz"], false, true, CoordinatesXZ);
        }

        private void zButton_Click(object sender, EventArgs e)
        {
            OrthoProjection(transformation.ProjectionZ, viewPoints["xOy"], false, false, CoordinatesXY);
        }

        private void obliqueButton_Click(object sender, EventArgs e)
        {
            ProjectionAction = () =>
            {
                currentComposite = compositeObject.Clone() as ICompositeObject;
                currentComposite = transformation.ObliqueProjection(currentComposite, (float)angleAlpha.Value, (float)lengthOblique.Value);

                InitializeAxisPen(false, false);

                viewPoint = viewPoints["xOy"];

                Draw(currentComposite, deltaX, deltaY, CoordinatesXY);
            };

            ProjectionAction();
        }

        private void centralButton_Click(object sender, EventArgs e)
        {
            ProjectionAction = () =>
            {
                currentComposite = compositeObject.Clone() as ICompositeObject;
                currentComposite = transformation.CentralProjection(currentComposite, (float)distance.Value);

                InitializeAxisPen(false, false);

                viewPoint = viewPoints["xOy"];

                Draw(currentComposite, deltaX, deltaY, CoordinatesXY);
            };

            ProjectionAction();
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            ProjectionAction = () =>
            {
                currentComposite = compositeObject.Clone() as ICompositeObject;
                currentComposite = transformation.ViewTransformation(currentComposite, (float)angleFiView.Value, (float)angleTetaView.Value,
                    (float)ro.Value, (float)distance.Value);

                InitializeAxisPen(false, false);

                viewPoint = ViewPointsHelper.GetViewPoint((float)angleFiView.Value, (float)angleTetaView.Value,
                    (float)ro.Value);

                Draw(currentComposite, deltaX, deltaY, CoordinatesXY);
            };

            ProjectionAction();
        }

        #endregion

        #region Helping methods

        private void ChangeDelta()
        {
            deltaY = drawingBox.Height / 2.0f;
            deltaX = drawingBox.Width / 2.0f;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (drawingBox.Width != 0 && drawingBox.Height != 0)
            {
                Draw(currentComposite, deltaX, deltaY, DrawAction);
            }
        }

        private void hideLinesCheck_CheckedChanged(object sender, EventArgs e)
        {
            hideLines = hideLinesCheck.Checked;
            if (!hideLines)
            {
                fillFacesCheck.Checked = false;
            }
            Draw(currentComposite, 0, 0, DrawAction);
        }

        private void fillFacesCheck_CheckedChanged(object sender, EventArgs e)
        {
            fillFaces = fillFacesCheck.Checked;
            if (fillFaces)
            {
                hideLinesCheck.Checked = true;
            }
            Draw(currentComposite, 0, 0, DrawAction);
        }

        #endregion
    }
}
