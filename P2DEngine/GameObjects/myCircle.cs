using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using P2DEngine.GameObjects.Collisions;

namespace P2DEngine
{
    //Al igual que la clase myBlock, esta hereda de myGameObject, para poder dibujar círculos.
    public class myCircle : myGameObject
    {
        public float radius;
        public myCircle(float x, float y, float radius, Color color) : base(x, y, radius*2, radius*2, color)
        {
            this.radius = radius;
        }

        public myCircle(float x, float y, float radius, Image image) : base(x, y, radius*2, radius*2, image)
        {
            this.radius = radius;
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            if (image == null)
            {
                g.FillEllipse(brush, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
            else
            {
                g.DrawImage(image, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
        }

        public override void Update(float deltaTime)
        {

        }
    }
}
