using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using ProjectAlgorithm.Factories;
using ProjectAlgorithm.Infrastructure;
using ProjectAlgorithm.Interfaces.Entities;
using ProjectAlgorithm.Interfaces.Lights;
using ProjectAlgorithm.Interfaces.Transformations;
using ProjectAlgorithm.Lights;
using ProjectAlgorithm.Shadows;
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
        private readonly Pen axisPenX = new Pen(Color.Firebrick);
        private readonly Pen axisPenY = new Pen(Color.Firebrick);
        private readonly int axisPadding = 5;
        private Action<Graphics, Pen, Brush, IFace, float, float> DrawAction;
        private Action ProjectionAction;
        private bool hideLines = true;
        private bool fillFaces = true;
        private IPoint viewPoint;
        private bool drawLines = false;
        private bool lightEnabled = false;
        private List<ILight> lights = new List<ILight>();
        //private bool shadowEnabled = false;

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

            ProjectionAction = () =>
            {
                Draw(compositeObject, deltaX, deltaY, CoordinatesXY);
            };

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

                currentComposite = HiddenAndLight(compositeObject, hideLines, lightEnabled);

                Shadow();

                Draw(currentComposite, deltaX, deltaY, CoordinatesXY);

                redraw = true;
            };

            ProjectionAction();
        }

        Bitmap bitmap;
        Graphics graphics;
        bool redraw = true;

        private void Draw(ICompositeObject composite, float deltaX, float deltaY, Action<Graphics, Pen, Brush, IFace, float, float> DrawAction)
        {
            this.DrawAction = DrawAction;

            if (redraw)
            {
                bitmap = new Bitmap(drawingBox.Width, drawingBox.Height);
                graphics = Graphics.FromImage(bitmap);
                drawingBox.Image = bitmap;
            }

            ChangeDelta();

            //PixelDrawer.DrawXY(new Face(new List<IPoint>()
            //{
            //    new ProjectAlgorithm.Entities.Point(0, 10, 0),
            //    new ProjectAlgorithm.Entities.Point(10, 20, 0),
            //    new ProjectAlgorithm.Entities.Point(12, 20, 0),
            //    new ProjectAlgorithm.Entities.Point(5, 10, 0)
            //}), graphics, Color.Blue, this.deltaX, this.deltaY);

            DrawAxis(graphics, this.deltaX, this.deltaY);

            if (composite == null)
                return;

            var pen = new Pen(Color.Aquamarine);
            var brush = new SolidBrush(Color.Black);

            foreach (var entity in composite.Entities)
            {
                foreach (var face in entity.Faces)
                {
                    if (!face.IsHidden || !hideLines)
                    {
                        if (reflectedEnabled.Checked)
                        {
                            PixelDrawer.DrawXY(face, graphics, this.deltaX, this.deltaY, lights.FirstOrDefault(), viewPoint);
                        }
                        else
                        {
                            brush.Color = face.Color;
                            DrawAction(graphics, pen, brush, face, this.deltaX, this.deltaY);
                        }
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

            if (drawLines)
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

            shadowCheck_CheckedChanged(null, null);
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            TransformObject((float)moveX.Value, (float)moveY.Value, (float)moveZ.Value, transformation.MoveObject);
        }

        #region Move values changed

        private void moveX_ValueChanged(object sender, EventArgs e)
        {
            TransformObject((float)moveX.Value, 0, 0, transformation.MoveObject);
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

        private void OrthoProjection(Func<ICompositeObject, ICompositeObject> Func, IPoint viewPoint, bool invertX, bool invertY, Action<Graphics, Pen, Brush, IFace, float, float> CoordAction)
        {
            ProjectionAction = () =>
            {
                this.viewPoint = viewPoint;

                currentComposite = HiddenAndLight(compositeObject, true, true);

                currentComposite = Func(currentComposite);

                InitializeAxisPen(invertX, invertY);

                Shadow();

                Draw(currentComposite, deltaX, deltaY, CoordAction);

                redraw = true;
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

        private void projectionButton_Click(object sender, EventArgs e)
        {
            ProjectionAction = () =>
            {
                viewPoint = viewPoints["xOy"];
                
                currentComposite = HiddenAndLight(compositeObject, false, true);

                currentComposite = transformation.AxonometricProjection(currentComposite, (float)anglePsi.Value, (float)angleFi.Value);

                currentComposite = HiddenAndLight(currentComposite, true, false);

                InitializeAxisPen(false, false);

                Shadow();

                Draw(currentComposite, deltaX, deltaY, CoordinatesXY);

                redraw = true;
            };

            ProjectionAction();
        }

        private void obliqueButton_Click(object sender, EventArgs e)
        {
            ProjectionAction = () =>
            {
                viewPoint = viewPoints["xOy"];
                
                currentComposite = HiddenAndLight(compositeObject, false, true);

                currentComposite = transformation.ObliqueProjection(currentComposite, (float)angleAlpha.Value, (float)lengthOblique.Value);

                currentComposite = HiddenAndLight(currentComposite, true, false);

                InitializeAxisPen(false, false);

                Shadow();

                Draw(currentComposite, deltaX, deltaY, CoordinatesXY);

                redraw = true;
            };

            ProjectionAction();
        }

        private void centralButton_Click(object sender, EventArgs e)
        {
            ProjectionAction = () =>
            {
                viewPoint = viewPoints["xOy"];
                InitializeAxisPen(false, false);
                
                currentComposite = HiddenAndLight(compositeObject, false, true);

                currentComposite = transformation.CentralProjection(currentComposite, (float)distance.Value);

                currentComposite = HiddenAndLight(currentComposite, true, false);

                Shadow();

                Draw(currentComposite, deltaX, deltaY, CoordinatesXY);

                redraw = true;
            };

            ProjectionAction();
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            ProjectionAction = () =>
            {
                viewPoint = ViewPointsHelper.GetViewPoint((float)angleFiView.Value, (float)angleTetaView.Value,
                        (float)ro.Value);
                InitializeAxisPen(false, false);

                currentComposite = HiddenAndLight(compositeObject, false, true);

                currentComposite = transformation.ViewTransformation(currentComposite, (float)angleFiView.Value, (float)angleTetaView.Value + 90,
                    (float)ro.Value);
                
                currentComposite = transformation.CentralProjection(currentComposite, (float)distance.Value);

                currentComposite = HiddenAndLight(currentComposite, true, false);

                Shadow();

                Draw(currentComposite, deltaX, deltaY, CoordinatesXY);

                redraw = true;
            };

            ProjectionAction();
        }

        private ICompositeObject HiddenAndLight(ICompositeObject composite, bool hide, bool light)
        {
            var compositeObj = composite.Clone() as ICompositeObject;

            if (hide)
            {
                compositeObj = transformation.HideLines(compositeObj, viewPoint);
            }

            if (light)
            {
                compositeObj = transformation.ChangeColors(compositeObj, (float)kDLight.Value,
                    (float)kALight.Value, (int)intensityALight.Value, lights.ToArray());
            }

            return compositeObj;
        }
        #endregion

        #region Helping methods

        private void Shadow()
        {
            //if (shadowCheck.Checked)
            //{
            //    var shadow = new Shadow();
                
            //    var composite = shadow.GetShadow(currentComposite, lights.First());

            //    Draw(composite, 0, 0, CoordinatesXY);

            //    redraw = false;
            //}
        }

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
                ProjectionAction();
            }
        }

        private void hideLinesCheck_CheckedChanged(object sender, EventArgs e)
        {
            hideLines = hideLinesCheck.Checked;
            if (!hideLines)
            {
                fillFacesCheck.Checked = false;
            }
            ProjectionAction();
        }

        private void fillFacesCheck_CheckedChanged(object sender, EventArgs e)
        {
            fillFaces = fillFacesCheck.Checked;
            if (fillFaces)
            {
                hideLinesCheck.Checked = true;
            }
            ProjectionAction();
        }

        private void drawLinesCheck_CheckedChanged(object sender, EventArgs e)
        {
            drawLines = drawLinesCheck.Checked;

            ProjectionAction();
        }

        private void lightButton_Click(object sender, EventArgs e)
        {
            lightEnabled = !lightEnabled;

            xLight_ValueChanged(null, null);
        }

        private void xLight_ValueChanged(object sender, EventArgs e)
        {
            if (lightEnabled)
            {
                lights.Clear();

                lights.Add(new Light(new ProjectAlgorithm.Entities.Point((int)xLight.Value, (int)yLight.Value, (int)zLight.Value), (int)intensityLight.Value));
            }

            ProjectionAction();
        }

        #endregion

        private void shadowCheck_CheckedChanged(object sender, EventArgs e)
        {
            //if (shadowCheck.Checked)
            //{
            //    ProjectionAction();
            //}
        }
    }
}
