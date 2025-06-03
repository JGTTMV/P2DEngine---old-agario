using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using P2DEngine.GameObjects;
using P2DEngine.GameObjects.Collisions;

namespace P2DEngine
{
    public class myPhysicsBlock : myPhysicsGameObject
    {
        public myPhysicsBlock(float x, float y, float sizeX, float sizeY, Color color) : base(x, y, sizeX, sizeY, color)
        {
        }

        public myPhysicsBlock(float x, float y, float sizeX, float sizeY, Image image) : base(x, y, sizeX, sizeY, image)
        {
        }

        // Crear el collider de caja. Esto se llamara automatico.
        public override void CreateCollider(float sizeX, float sizeY)
        {
            collider = new BoxCollider2D(sizeX, sizeY, this);
        }

        // Se dibuja igual que un game object.
        public override void Draw(Graphics g, Vector position, Vector size)
        {
            if(image == null)
            {
                g.FillRectangle(brush, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
            else
            {
                g.DrawImage(image, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
        }


        // Me dio flojera cambiarlo para que el nombre se quedase en Update, pero este se supone que es el Update que conocen
        // de todo el semestre :)
        public override void UpdateGameObject(float deltaTime)
        {
        }
    }

    public class Medkit : myPhysicsBlock
    {
        public Medkit(float x, float y, float sizeX, float sizeY)
            : base(x, y, sizeX, sizeY, Color.Green)
        {
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            if (image != null)
            {
                g.DrawImage(image, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
            else
            {
                g.FillRectangle(Brushes.Green, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
        }
    }

    public class WallBlock : myPhysicsBlock
    {
        public WallBlock(float x, float y, float sizeX, float sizeY)
            : base(x, y, sizeX, sizeY, Color.Black)
        {
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            g.FillRectangle(Brushes.DarkBlue, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            using (var pen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(pen, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
        }
    }
}
