using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
        private readonly IEntitiesFactory factory = new EntitiesFactory();
        private IEntity holeEntity;
        private IEntity entity;
        private ICompositeObject compositeObject;
        private readonly ITransformation transformation = new Transformation();
        private float deltaX;
        private float deltaY;
        private bool flag;
        private Point current;

        public FormPL()
        {
            InitializeComponent();
        }

        private void Draw(ICompositeObject compositeObject)
        {
            ChangeDelta();
            var graphics = this.CreateGraphics();
            graphics.Clear(Color.White);
            var pen = new Pen(Color.Blue);

            foreach (var line in compositeObject.GetLines())
            {
                graphics.DrawLine(pen, line.First.X + deltaX, -line.First.Y + deltaY, line.Second.X + deltaX, -line.Second.Y + deltaY);
            }
        }

        private void DrawX(ICompositeObject compositeObject)
        {
            ChangeDelta();
            var graphics = this.CreateGraphics();
            graphics.Clear(Color.White);
            var pen = new Pen(Color.Blue);

            foreach (var line in compositeObject.GetLines())
            {
                graphics.DrawLine(pen, line.First.Z + deltaX, -line.First.Y + deltaY, line.Second.Z + deltaX, -line.Second.Y + deltaY);
            }
        }

        private void DrawY(ICompositeObject compositeObject)
        {
            ChangeDelta();
            var graphics = this.CreateGraphics();
            graphics.Clear(Color.White);
            var pen = new Pen(Color.Blue);

            foreach (var line in compositeObject.GetLines())
            {
                graphics.DrawLine(pen, line.First.X + deltaX, line.First.Z + deltaY, line.Second.X + deltaX, line.Second.Z + deltaY);
            }
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.MoveObject(compositeObject, (float)moveX.Value, -(float)moveY.Value, (float)moveZ.Value);
            Draw(compositeObject);
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            entity = factory.CreateEntity((float)height.Value, (float)radius.Value, (float)radiusTop.Value, (int)number.Value);
            holeEntity = factory.CreateEntity((float)height2.Value, (float)radius2.Value, (float)radius2Top.Value, (int)number2.Value);
            compositeObject = new CompositeObject(new List<IEntity> { holeEntity, entity });
            Draw(compositeObject);
        }

        private void rotateButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.RotateObject(compositeObject, (float)rotateX.Value, (float)rotateY.Value, (float)rotateZ.Value);
            Draw(compositeObject);
        }

        private void scaleButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.ScaleObject(compositeObject, (float)scaleX.Value, (float)scaleY.Value, (float)scaleZ.Value);
            Draw(compositeObject);
        }

        private void ChangeDelta()
        {
            deltaY = this.Height / 2.0f;
            deltaX = this.Width / 2.0f;
        }

        private void FormPL_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag)
            {
                float y = (e.X - current.X) / 100.0f;
                float x = (e.Y - current.Y) / 100.0f;
                compositeObject = transformation.RotateObject(compositeObject, x, y, 0);
                Draw(compositeObject);
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

        private void proectionButton_Click(object sender, EventArgs e)
        {
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).Proection(composite, (float)anglePsi.Value, (float)angleFi.Value);

            Draw(composite);
        }

        private void xButton_Click(object sender, EventArgs e)
        {
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).ProectionX(composite);

            DrawX(composite);
        }

        private void yButton_Click(object sender, EventArgs e)
        {
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).ProectionY(composite);

            DrawY(composite);
        }

        private void zButton_Click(object sender, EventArgs e)
        {
            var composite = compositeObject.Clone() as ICompositeObject;
            composite = ((Transformation)transformation).ProectionZ(composite);

            Draw(composite);
        }
    }
}
