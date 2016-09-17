using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProjectAlgorithm;
using ProjectAlgorithm.Entities;
using ProjectAlgorithm.Interfaces;
using ProjectAlgorithm.Transformations;

namespace FormsPL
{
    public partial class FormPL : Form
    {
        private readonly IEntitiesFactory factory = new EntitiesFactory();
        private IEntity holeEntity;
        private IEntity entity;
        private ICompositeObject compositeObject;
        private ITransformation transformation = new Transformation();
        private float deltaX;
        private float deltaY;

        public FormPL()
        {
            InitializeComponent();
        }

        private void Draw()
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

        private void moveButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.MoveObject(compositeObject, (float)moveX.Value, -(float)moveY.Value, (float)moveZ.Value);
            Draw();
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            entity = factory.CreateEntity((float)height.Value, (float)radius.Value, (float)radiusTop.Value, (int)number.Value);
            holeEntity = factory.CreateEntity((float)height2.Value, (float)radius2.Value, (float)radius2Top.Value, (int)number2.Value);
            compositeObject = new CompositeObject(new List<IEntity> { holeEntity, entity });
            Draw();
        }

        private void rotateButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.RotateObject(compositeObject, (float)rotateX.Value, (float)rotateY.Value, (float)rotateZ.Value);
            Draw();
        }

        private void scaleButton_Click(object sender, EventArgs e)
        {
            compositeObject = transformation.ScaleObject(compositeObject, (float)scaleX.Value, (float)scaleY.Value, (float)scaleZ.Value);
            Draw();
        }

        private void ChangeDelta()
        {
            deltaY = this.Height / 2;
            deltaX = this.Width / 2;
        }
    }
}
