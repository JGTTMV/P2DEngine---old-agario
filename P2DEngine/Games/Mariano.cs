using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P2DEngine.GameObjects;
using P2DEngine.GameObjects.Collisions;
using System.Windows.Forms;
using P2DEngine.Managers;
using System.Windows;
using System.Security.Cryptography.X509Certificates;
using static P2DEngine.myPhysicsBlock;

namespace P2DEngine.Games
{
    public class Mariano : myGame //Juan José Valdebenito
    {
        //Objetos del juego
        private myPlayer player;
        private List<myEnemy> turtles;
        private Image Block;
        private Image pipeR;
        private Image pipeL;
        private List<WallBlock> walls;

        private Font gameFont;
        private bool onGround = false;
        private bool gameOver = false;

        private int musicIndex; //Guarda el indice de la musica

        //Posibles posiciones de spawn para cada enemigo
        float[] spawnYOptions = { 125, 510 };
        float[] spawnXOptions = { 80, 180, 580 };
        private int enemysizeX = 30;
        private int enemysizeY = 50;

        private float spawnTimer = 0f;
        private float interval = 3.0f; //Intervalo de spawn en segundos
        private myTeleporter myTeleporterA, myTeleporterB;
        private myTeleporter myTeleporterC, myTeleporterD;

        public Mariano(int width, int height, int FPS, myCamera c) : base(width, height, FPS, c)
        {
            gameFont = new Font("courierFont", 16);

            musicIndex = myAudioManager.Play("Game Music", 0.3f); //Musica de fondo

            turtles = new List<myEnemy>();

            //Carga las texturas de objetos estaticos
            Block = myImageManager.Get("Block");
            pipeR = myImageManager.Get("Pipe_R");
            pipeL = myImageManager.Get("Pipe_L");

            //Crea los teletransportadores
            myTeleporterA = new myTeleporter(0, 85, 100, 75, pipeL);
            myTeleporterB = new myTeleporter(width - 100, 85, 100, 75, pipeR);
            myTeleporterC = new myTeleporter(0, height - 125, 100, 75, pipeL);
            myTeleporterD = new myTeleporter(width - 100, height - 125, 100, 75, pipeR);

            //Inicia el juego
            StartGame();

            walls = new List<WallBlock>();

            for (int i = 0; i < 21; i++) //Crea el piso
            {
                walls.Add(new WallBlock((39 * i), 565, 40, 40, Block));
            }

            for (int i = 0; i < 7; i++) //Crea el techo A
            {
                walls.Add(new WallBlock((39 * i), 395, 40, 40, Block));
            }

            for (int i = 13; i < 21; i++) //Crea el techo B
            {
                walls.Add(new WallBlock((39 * i), 395, 40, 40, Block));
            }

            for (int i = 8; i < 12; i++) //Crea el techo entremedio
            {
                walls.Add(new WallBlock((39 * i), 265, 40, 40, Block));
            }

            for (int i = 0; i < 7; i++) //Crea el techo C
            {
                walls.Add(new WallBlock((39 * i), 165, 40, 40, Block));
            }

            for (int i = 13; i < 21; i++) //Crea el techo D
            {
                walls.Add(new WallBlock((39 * i), 165, 40, 40, Block));
            }

            foreach (var wall in walls)
                Instantiate(wall);

            Instantiate(myTeleporterA);
            Instantiate(myTeleporterB);
            Instantiate(myTeleporterC);
            Instantiate(myTeleporterD);
        }

        public void SpawnEnemy() //Espawnea a los enemigos iniciales
        {
            turtles.Add(new myEnemy(100, 125, enemysizeX, enemysizeY, null));
            turtles.Add(new myEnemy(600, 125, enemysizeX, enemysizeY, null));
            turtles.Add(new myEnemy(600, 510, enemysizeX, enemysizeY, null));
            turtles.Add(new myEnemy(200, 510, enemysizeX, enemysizeY, null));

            foreach (var turtle in turtles)
                Instantiate(turtle);

        }

        //Para los metodos que manejan propiedades de tanto el jugador como cada enemigo
        //Ocupo el termino de "actor"
        private void ResolveCollision(myPhysicsGameObject actor, WallBlock wall)
        {
            //Calcula cuanto choca el actor con cada bloque
            float overlapX = Math.Min(actor.x + actor.sizeX, wall.x + wall.sizeX) - Math.Max(actor.x, wall.x);
            float overlapY = Math.Min(actor.y + actor.sizeY, wall.y + wall.sizeY) - Math.Max(actor.y, wall.y);

            //Soluciona los pequeños overlaps (horizontal o vertical)
            if (overlapY < overlapX)
            {
                //Colision vertical
                if (actor.y < wall.y)
                {
                    //Si el actor esta por encima de un bloque (cayo sobre el)
                    actor.y = wall.y - player.sizeY;
                    if (actor is myPlayer)
                    {
                        player.SetOnGround(true); //Maneja los saltos
                        onGround = true;
                        actor.velocityY = 0; //Maneja la velocidad vertical
                    }
                    else if (actor is myEnemy)
                    {
                        ((myEnemy)actor).SetOnGround(true);
                        onGround = true;
                        actor.velocityY = 0;
                    }

                }
                else
                {
                    //Si el actor choco con el techo
                    actor.y = wall.y + wall.sizeY;
                    onGround = false; //Se detecta el salto
                }
            }
            else
            {
                //Colision horizontal
                if (actor.x < wall.x)
                    actor.x = wall.x - actor.sizeX;
                else
                    actor.x = wall.x + wall.sizeX;
            }
        }
        private void teleporterCollision(myTeleporter teleporter, myPhysicsGameObject actor)
        {
            if (actor.IsColliding(teleporter))
            {
                //Teletransporta al actor a la tuberia correspondiente
                //Las conecte diagonalmente para mover mas facilmente a los enemigos
                if (teleporter == myTeleporterA)
                {
                    actor.x = myTeleporterD.x - actor.sizeX;
                    actor.y = myTeleporterD.y;
                }
                else if (teleporter == myTeleporterB)
                {
                    actor.x = myTeleporterC.x + myTeleporterB.sizeX;
                    actor.y = myTeleporterC.y;
                }
                else if (teleporter == myTeleporterC)
                {
                    actor.x = myTeleporterB.x - actor.sizeX;
                    actor.y = myTeleporterB.y;
                }
                else if (teleporter == myTeleporterD)
                {
                    actor.x = myTeleporterA.x + myTeleporterB.sizeX;
                    actor.y = myTeleporterA.y;
                }
            }
        }

        public void StartGame()
        {
            gameOver = false;

            //Elimina enemigos existentes
            foreach (var turtle in turtles)
                Destroy(turtle);
            turtles.Clear();

            //Elimina al jugador anterior, si es que existe
            if (player != null)
                Destroy(player);

            //Detiene la musica anterior si esta sonando
            myAudioManager.Stop(musicIndex);
            musicIndex = myAudioManager.Play("Game Music", 0.3f);

            //Crea a un nuevo jugador y enemigos
            player = new myPlayer(380, 220, 40, 60, null);
            player.score = 0;
            player.lives = 3;
            Instantiate(player);
            SpawnEnemy();
        }

        protected override void ProcessInput()
        {
            if (myInputManager.IsKeyPressed(Keys.R) && gameOver)
            {
                StartGame();
            }
            //Actualiza los inputs desde la clase myPlayer 
            player.UpdateDirection(deltaTime);
            foreach (var turtle in turtles)
            {
                turtle.EnemyMove(deltaTime);
            }
        }

        protected override void Update()
        {
            Random rand = new Random();
            var ActorsToDestroy = new List<myPhysicsGameObject>(); //Lista para eliminar actores

            if (gameOver) //Para las actualizaciones si pierde el jugador
            {
                return;
            }

            foreach (var wall in walls) //revisa las colisiones del jugador y los enemigos
            {
                if (player.IsColliding(wall))
                {
                    ResolveCollision(player, wall);
                }
                foreach (var turtle in turtles)
                {
                    if (turtle.IsColliding(wall))
                    {
                        ResolveCollision(turtle, wall);
                    }
                }
            }

            foreach (var turtle in turtles)
            {
                if (turtle.IsColliding(player))
                {
                    //La deteccion para que un jugador elimine a un enemigo funciona tal que:
                    //1. El jugador debe estar cayendo (velocityY > 0)
                    //2. El borde inferior del jugador estaba por encima del enemigo en el frame anterior
                    //3. Ahora el borde inferior del jugador está dentro del rango superior del enemigo
                    if (
                        player.velocityY > 0 &&
                        player.prevY + player.sizeY <= turtle.y + 2 &&
                        player.y + player.sizeY >= turtle.y && player.y + player.sizeY <= turtle.y + 20
                    )
                    {
                        turtle.isDead = true; //El enemigo muere
                        ActorsToDestroy.Add(turtle);
                        myAudioManager.Play("Stomp", 0.7f);
                        player.score += 10;
                    }
                    else //el jugador toca a un enemigo por los lados
                    {
                        //Resetea la posicion del jugador
                        player.x = 380;
                        player.y = 220;
                        player.lives--; //Quita una vida
                        if (player.lives <= 0)
                        {
                            player.lives = 0;
                            gameOver = true;
                            myAudioManager.Stop(musicIndex); //Detiene la música al perder
                            myAudioManager.Play("Game Over", 0.7f); //Sonido de Game Over
                        }
                        else
                        {
                            myAudioManager.Play("Bump", 0.7f); //Sonido al perder una vida
                        }
                    }
                }

                //Colisiones del teleporter para los enemigos
                if (myTeleporterA.IsColliding(turtle) || myTeleporterB.IsColliding(turtle) || myTeleporterC.IsColliding(turtle) || myTeleporterD.IsColliding(turtle))
                {
                    teleporterCollision(myTeleporterA, turtle);
                    teleporterCollision(myTeleporterB, turtle);
                    teleporterCollision(myTeleporterC, turtle);
                    teleporterCollision(myTeleporterD, turtle);

                    myAudioManager.Play("Pipe", 0.3f);
                }
            }

            //Colisiones del teleporter para el jugador
            if (myTeleporterA.IsColliding(player) || myTeleporterB.IsColliding(player) || myTeleporterC.IsColliding(player) || myTeleporterD.IsColliding(player))
            {
                teleporterCollision(myTeleporterA, player);
                teleporterCollision(myTeleporterB, player);
                teleporterCollision(myTeleporterC, player);
                teleporterCollision(myTeleporterD, player);

                myAudioManager.Play("Pipe", 0.3f);
            }
            if (!onGround) //Detecta un salto
            {
                player.SetOnGround(false);
            }

            spawnTimer += deltaTime;

            if (spawnTimer >= interval) //Spawnea nuevos enemigos
            {
                spawnTimer = 0;
                //Randomiza basado en 4 posiciones distintas
                float spawnX = spawnXOptions[rand.Next(spawnXOptions.Length)];
                float spawnY = spawnYOptions[rand.Next(spawnYOptions.Length)];
                myEnemy turtle = new myEnemy(spawnX, spawnY, enemysizeX, enemysizeY, null);
                turtles.Add(turtle);
                Instantiate(turtle);
                Console.WriteLine($"Nuevo enemigo spawn en X: {spawnX}, Y: {spawnY}");
            }

            //Revisa que enemigos hay que destruir
            foreach (var e in ActorsToDestroy) Destroy(e);
            turtles.RemoveAll(t => t.isDead);
        }

        protected override void RenderGame(Graphics g)
        {

            g.Clear(Color.SkyBlue);

            //Ilustra los objetos del juego
            foreach (var wall in walls)
                wall.Draw(g, new Vector(wall.x, wall.y), new Vector(wall.sizeX, wall.sizeY));

            myTeleporterA.Draw(g, new Vector(myTeleporterA.x, myTeleporterA.y), new Vector(myTeleporterA.sizeX, myTeleporterA.sizeY));
            myTeleporterB.Draw(g, new Vector(myTeleporterB.x, myTeleporterB.y), new Vector(myTeleporterB.sizeX, myTeleporterB.sizeY));
            myTeleporterC.Draw(g, new Vector(myTeleporterC.x, myTeleporterC.y), new Vector(myTeleporterC.sizeX, myTeleporterC.sizeY));
            myTeleporterD.Draw(g, new Vector(myTeleporterD.x, myTeleporterD.y), new Vector(myTeleporterD.sizeX, myTeleporterD.sizeY));

            player.Draw(g, new Vector(player.x, player.y), new Vector(player.sizeX, player.sizeY));
            foreach (var turtle in turtles)
            {
                turtle.Draw(g, new Vector(turtle.x, turtle.y), new Vector(turtle.sizeX, turtle.sizeY));
            }

            //UI del jugador
            g.DrawString($"Vidas: {player.lives}", gameFont, Brushes.White, new PointF(10, 10));
            g.DrawString($"Puntaje: {player.score}", gameFont, Brushes.White, new PointF(100, 10));

            //Mensaje de que el jugador perdio
            if (gameOver)
            {
                g.DrawString("GAME OVER", myFontManager.Get("courierFont", 50), Brushes.Red, new PointF(200, windowHeight / 2));
                g.DrawString("Presione 'R' para reiniciar", gameFont, Brushes.Red, new PointF(275, 360));
            }


        }
    }
}
