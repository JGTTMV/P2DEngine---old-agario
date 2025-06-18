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
    public class myPlayer : myPhysicsBlock
    {
        private mySprite marianoSprite;
        private float moveSpeed = 100f; //Velocidad de movimiento
        private float speedIncrease = 50f; //Incremento de velocidad al caminar
        private float currentSpeed = 100f; //Velocidad actual del jugador
        public int lives = 3; //vidas del jugador
        public int score = 0; //puntaje inicial

        private bool isFacingRight = true; //Direccion del personaje
        private bool isWalking = false;
        private bool isJumping = false;

        private float jumpVelocity = -710f; //Velocidad y rango del salto
        private float gravity = 1500f;      //Gravedad del jugador
        private new float velocityY = 0f;   //Velocidad vertical

        public float prevY; //Guarda la posición Y anterior para colisiones

        public myPlayer(float x, float y, float sizeX, float sizeY, mySprite sprite)
            : base(x, y, sizeX, sizeY, Color.Transparent)
        {
            //Crea el sprite con las animaciones, Cada frame dura 0.1 segundos
            marianoSprite = new mySprite(0.1f);

            //Carga la animacion de caminar hacia la derecha
            marianoSprite.AddFrame(myImageManager.Get("Mariano_R_Walk_1")); //0
            marianoSprite.AddFrame(myImageManager.Get("Mariano_R_Walk_2")); //1
            marianoSprite.AddFrame(myImageManager.Get("Mariano_R_Walk_3")); //2

            //Carga la animacion de caminar hacia la izquierda
            marianoSprite.AddFrame(myImageManager.Get("Mariano_L_Walk_1")); //3
            marianoSprite.AddFrame(myImageManager.Get("Mariano_L_Walk_2")); //4
            marianoSprite.AddFrame(myImageManager.Get("Mariano_L_Walk_3")); //5

            //Carga frames de idle, 3 frames iguales para que dure mas
            marianoSprite.AddFrame(myImageManager.Get("Mariano_R_Idle"), 3); //6
            marianoSprite.AddFrame(myImageManager.Get("Mariano_L_Idle"), 3); //7

            //Carga frames de salto
            marianoSprite.AddFrame(myImageManager.Get("Mariano_R_Jump"), 3); //8
            marianoSprite.AddFrame(myImageManager.Get("Mariano_L_Jump"), 3); //9

            affectedByGravity = true;
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            //Dibuja el sprite correspondiente a la direccion actual
            if (marianoSprite.GetCurrentFrame() != null)
            {
                g.DrawImage(marianoSprite.GetCurrentFrame(), (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
            else
            {
                Draw(g, position, size); //Fallback si no hay sprite
            }
        }
        public void SetOnGround(bool onGround)
        {
            isJumping = !onGround; //Confirma si el jugador esta saltando o no
            if (onGround)
                velocityY = 0; //Resetea posicion vertical al dar con el suelo
        }
        public override void UpdateGameObject(float deltaTime)
        {
            prevY = y; //Guarda la posición Y antes de actualizarla

            //Seleccion de animaciones
            if (isJumping)
            {
                marianoSprite.SetAnimationRange(isFacingRight ? 8 : 9, 1);
                //No tengo idea de como implementar la animacion de salto
            }
            else if (isWalking && !isJumping) //Animacion de caminata
            {
                marianoSprite.SetAnimationRange(isFacingRight ? 0 : 3, 3);
            }
            else if (!isWalking && !isJumping) //Animacion de salto
            {
                marianoSprite.SetAnimationRange(isFacingRight ? 6 : 7, 1);
            }

            KeepInBounds(800, 600);

            marianoSprite.Update(deltaTime);

            //Salto basado en fisicas
            velocityY += gravity * deltaTime; //Aplica gravedad
            y += velocityY * deltaTime;       //Actualiza posicion vertical
        }

        public void UpdateDirection(float deltaTime)
        {
            isWalking = false;
            bool moving = false;

            //Controla al jugador
            if (myInputManager.IsKeyPressed(Keys.Right) || myInputManager.IsKeyPressed(Keys.D))
            {
                currentSpeed += speedIncrease * deltaTime;
                if (currentSpeed > moveSpeed * 2) currentSpeed = moveSpeed * 2;
                x += currentSpeed * deltaTime;
                isFacingRight = true;
                isWalking = true;
                moving = true;
            }
            else if (myInputManager.IsKeyPressed(Keys.Left) || myInputManager.IsKeyPressed(Keys.A))
            {
                currentSpeed += speedIncrease * deltaTime;
                if (currentSpeed > moveSpeed * 2) currentSpeed = moveSpeed * 2;
                x -= currentSpeed * deltaTime;
                isFacingRight = false;
                isWalking = true;
                moving = true;
            }
            if (!moving)
            {
                currentSpeed = moveSpeed;
            }

            //Solo permite saltar con la flecha hacia arriba
            if (myInputManager.IsKeyPressed(Keys.Up) && !isJumping)
            {
                velocityY = jumpVelocity;
                y += velocityY * deltaTime;
                myAudioManager.Play("Jump", 0.3f);
                isJumping = true;
            }
            //Bloquea cualquier salto con barra de espacio
            //Esto debido a que ocurrian errores al saltar con la barra de espacio
        }

        //Metodo para mantener al jugador dentro de los limites de la pantalla
        public void KeepInBounds(float screenWidth, float screenHeight)
        {
            if (x < 0) x = 0;
            if (x + sizeX > screenWidth) x = screenWidth - sizeX;
            if (y < 0) y = 0;
        }
    }
}