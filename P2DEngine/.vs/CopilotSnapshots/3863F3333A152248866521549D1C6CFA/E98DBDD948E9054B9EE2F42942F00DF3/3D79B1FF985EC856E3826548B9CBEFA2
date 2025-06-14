using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace P2DEngine
{
    public class Player : myBlock
    {
        public float speed { get; set; } //Se añade una variable de velocidad para controla el movimiento

        public Player(float x, float y, float sizeX, float sizeY, Color color) : base((int)x, (int)y, (int)sizeX, (int)sizeY, color)
        {
            speed = .6f; //Velocidad default
        }

        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed; //Actualiza la velocidad del jugador
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            //Se dibuja como un rectangulo
            g.FillEllipse(brush, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
        }

        public override void Update(float deltaTime)
        {
            //Logica de actualizacion especifica al jugador
        }
    }
}
