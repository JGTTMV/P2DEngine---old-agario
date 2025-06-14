using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace P2DEngine
{
    // Clase de GameObjects, todos los objetos que vemo sen el juego son un GameObject.
    public abstract class myGameObject
    {
        public float x;
        public float y;
        public float sizeX;
        public float sizeY;
        public SolidBrush brush; // Para el pintado.

        public myGameObject(float x, float y, float sizeX, float sizeY, Color color)
        {
            this.x = x;
            this.y = y;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.brush = new SolidBrush(color);

            //Se asegura que las variables de x e y esten bien inicializadas para la colision
            if (sizeX <= 0 || sizeY <= 0)
            {
                throw new ArgumentException("sizeX and sizeY deben ser mayores a 0.");
            }
        }

        //Se añade un metodo para la colision del rectangulo
        public Rectangle GetCollisionRectangle()
        {
            return new Rectangle((int)x, (int)y, (int)sizeX, (int)sizeY);
        }

        //Cada GameObject que usemos deberá implementar su propio Draw y su propio Update.
        public abstract void Update(float deltaTime);

        public abstract void Draw(Graphics g, Vector position, Vector size);

        public void SetColor(Color color)
        {
            brush.Color = color;
        }
    }
}
