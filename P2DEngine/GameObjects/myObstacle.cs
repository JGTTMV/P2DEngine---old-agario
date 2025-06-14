using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace P2DEngine
{
    //Clase para Obstaculos, hereda de myBlock.
    public class myObstacle : myBlock
    {
        public bool isObstacle;

        public myObstacle(int x, int y, int sizeX, int sizeY, Color c, bool isObstacle) : base(x, y, sizeX, sizeY, c)
        {
            this.isObstacle = isObstacle;
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            //Se dibuja como un rectangulo
            g.FillRectangle(brush, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
        }

        public override void Update(float deltaTime)
        {
            //Los bloques no se actualizan
        }
    }
}