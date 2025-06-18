using P2DEngine.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace P2DEngine.GameObjects
{
    public class myEnemy : myPhysicsBlock
    {
        private mySprite turtleSprite;
        private float moveSpeed = 75f; //Velocidad de movimiento del enemigo
        private bool isFacingRight = true; //Direccion del enemigo
        private float gravity = 1500f;      //Gravedad de caida
        private new float velocityY = 0f;       //Velocidad vertical
        public bool isDead = false; //Estado del enemigo

        public myEnemy(float x, float y, float sizeX, float sizeY, mySprite sprite)
            : base(x, y, sizeX, sizeY, Color.Transparent)
        {
            //Crea el sprite con las animaciones, Cada frame dura 0.1 segundos
            turtleSprite = new mySprite(0.1f);
            //Carga la animacion de caminar hacia la derecha
            turtleSprite.AddFrame(myImageManager.Get("Turtle_R_1")); //0
            turtleSprite.AddFrame(myImageManager.Get("Turtle_R_2")); //1
            turtleSprite.AddFrame(myImageManager.Get("Turtle_R_3")); //2
            //Carga la animacion de caminar hacia la izquierda
            turtleSprite.AddFrame(myImageManager.Get("Turtle_L_1")); //3
            turtleSprite.AddFrame(myImageManager.Get("Turtle_L_2")); //4
            turtleSprite.AddFrame(myImageManager.Get("Turtle_L_3")); //5
            affectedByGravity = true;
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {

            if (isDead) //deberia mostrar al enemigo aplastado
            {
                //Dibuja el sprite correspondiente a la direccion actual
                if (turtleSprite.GetCurrentFrame() != null)
                {
                    g.DrawImage(turtleSprite.GetCurrentFrame(), (float)position.X, (float)position.Y + ((float)size.Y / 2), (float)size.X, (float)size.Y / 2);
                }
                else
                {
                    Draw(g, position, size); //Fallback si no hay sprite
                }
            }
            else
            {
                //Dibuja el sprite correspondiente a la direccion actual
                if (turtleSprite.GetCurrentFrame() != null)
                {
                    g.DrawImage(turtleSprite.GetCurrentFrame(), (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
                }
                else
                {
                    Draw(g, position, size); //Fallback si no hay sprite
                }
            }
        }

        public void EnemyMove(float deltaTime)
        {
            //Mueve al enemigo en la direccion actual
            if (isFacingRight)
            {
                x += moveSpeed * deltaTime; //Mueve a la derecha
            }
            else
            {
                x -= moveSpeed * deltaTime; //Mueve a la izquierda
            }
            //Cambia de direccion al alcanzar los limites de la pantalla
            if (x < 0 || x + sizeX > 800) //Asumiendo un ancho de pantalla de 800
            {
                isFacingRight = !isFacingRight; //Cambiar direccion
            }
        }

        public override void UpdateGameObject(float deltaTime)
        {

            //Animacion de caminar
            turtleSprite.SetAnimationRange(isFacingRight ? 0 : 3, 3);
            
            KeepInBounds(800, 600);

            turtleSprite.Update(deltaTime);

            //Salto basado en fisicas
            velocityY += gravity * deltaTime; //Aplica gravedad
            y += velocityY * deltaTime;       //Actualiza posicion vertical
        }

        public void SetOnGround(bool onGround)
        {
            if (onGround)
                velocityY = 0; //Resetea posicion vertical al dar con el suelo
        }

        public void KeepInBounds(float screenWidth, float screenHeight)
        {
            if (x < 0) x = 0;
            if (x + sizeX > screenWidth) x = screenWidth - sizeX;
            if (y < 0) y = 0;
        }
    }
}
