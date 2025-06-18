using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using P2DEngine.GameObjects.Collisions;

namespace P2DEngine.GameObjects
{
    public class myTeleporter : myPhysicsBlock
    {
        private new Image image;

        public myTeleporter(float x, float y, float sizeX, float sizeY, Image texture) : base(x, y, sizeX, sizeY, Color.Red)
        {
            this.image = texture;
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            if (image != null)
            {
                g.DrawImage(image, (float)position.X, (float)position.Y,
                    (float)size.X, (float)size.Y);
            }
            else
            {
                g.FillRectangle(brush, (float)position.X, (float)position.Y,
                    (float)size.X, (float)size.Y);
            }
        }

        public void Teleport(myPlayer player, float x, float y)
        {
            //Teletransporta al jugador a la posicion deseada
            player.x = x;
            player.y = y;
        }
    }
}