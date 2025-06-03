using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using P2DEngine.Managers;

namespace P2DEngine.GameObjects
{
    public class myPlayer : myPhysicsBlock
    {
        private Image[] sprites = new Image[4]; //0: front, 1: back, 2: left, 3: right
        Image bulletSprite;
        private int currentDirection = 0; //0: front, 1: back, 2: left, 3: right
        private float speed = 200f; //Velocidad de movimiento
        private int vida = 100; //Vida del jugador
        private float invulnerableTime = 1.0f; //1 segundo de invulnerabilidad inicial

        public int Vida { get { return vida; } }
        public bool IsInvulnerable { get { return invulnerableTime > 0; } }

        public myPlayer(float x, float y, float sizeX, float sizeY)
            : base(x, y, sizeX, sizeY, Color.Transparent)
        {
            //Carga los sprites
            sprites[0] = myImageManager.Get("gas-front");
            sprites[1] = myImageManager.Get("gas-back");
            sprites[2] = myImageManager.Get("gas-left");
            sprites[3] = myImageManager.Get("gas-right");

            affectedByGravity = false;
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            //Dibuja el sprite correspondiente a la direccion actual
            if (sprites[currentDirection] != null)
            {
                g.DrawImage(sprites[currentDirection], (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
            else
            {
                base.Draw(g, position, size); //Fallback si no hay sprite
            }
        }

        public override void UpdateGameObject(float deltaTime)
        {
            if (invulnerableTime > 0) //Genera frames de invulnerabilidad
                invulnerableTime -= deltaTime;
        }

        public void SetLife() { vida = 100; }

        public void UpdateDirection(float deltaTime)
        {
            //Solo permite el movimiento para un eje a la vez (no diagonal)
            bool up = myInputManager.IsKeyPressed(Keys.W);
            bool down = myInputManager.IsKeyPressed(Keys.S);
            bool left = myInputManager.IsKeyPressed(Keys.A);
            bool right = myInputManager.IsKeyPressed(Keys.D);

            //Prioridad: vertical sobre horizontal
            if (up && !down && !left && !right)
            {
                y -= speed * deltaTime;
                currentDirection = 1; // back
            }
            else if (down && !up && !left && !right)
            {
                y += speed * deltaTime;
                currentDirection = 0; // front
            }
            else if (left && !up && !down && !right)
            {
                x -= speed * deltaTime;
                currentDirection = 2; // left
            }
            else if (right && !up && !down && !left)
            {
                x += speed * deltaTime;
                currentDirection = 3; // right
            }
        }

        //Metodo para mantener al jugador dentro de los limites de la pantalla
        public void KeepInBounds(float screenWidth, float screenHeight)
        {
            if (x < 0) x = 0;
            if (x + sizeX > screenWidth) x = screenWidth - sizeX;
            if (y < 0) y = 0;
            if (y + sizeY > screenHeight) y = screenHeight - sizeY;
        }

        public void PerderVida(int cantidad)
        {
            if (IsInvulnerable && cantidad > 0) return;
            vida -= cantidad;
            invulnerableTime = 1.0f;
            if (vida < 0) vida = 0;
        }

        public myBullet Shoot()
        {
            //Dispara en la direccion actual usando siempre la textura de bullet
            bulletSprite = myImageManager.Get("bullet");
            if (currentDirection == 0)
            {
                return new myBullet(x + sizeX / 2 - 2, y + sizeY, 5, Color.Yellow, bulletSprite, 300, 0); // Abajo
            }
            else if (currentDirection == 1)
            {
                return new myBullet(x + sizeX / 2 - 2, y - 10, 5, Color.Yellow, bulletSprite, -300, 0); // Arriba
            }
            else if (currentDirection == 2)
            {
                return new myBullet(x - 10, y + sizeY / 2 - 2, 5, Color.Yellow, bulletSprite, 0, -300); // Izquierda
            }
            else
            {
                return new myBullet(x + sizeX + 2, y + sizeY / 2 - 2, 5, Color.Yellow, bulletSprite, 0, 300); // Derecha
            }
        }
    }
}
