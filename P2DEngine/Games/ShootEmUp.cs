using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;
using P2DEngine.GameObjects;
using P2DEngine.GameObjects.Collisions;
using P2DEngine.GameObjects.Enemigos;
using P2DEngine.Managers;

namespace P2DEngine.Games
{
    public class ShootEmUp : myGame //Juan Jose Valdebenito
    {
        /*
        private myPlayer player;
        private int score = 0;
        private int lives = 3;
        private float gameTime = 0;
        private bool gameOver = false;

        private float shootCooldown = 0; //Para controlar el disparo
        private ShootEmUp game;

        //Se crean los sprites del juego
        private Image playerSprite;
        private Image enemy1Sprite;
        private Image enemy2Sprite;
        private Image enemy3Sprite;
        private Image background;

        private float SFXvolume = 0.2f;
        private const float SHOOT_DELAY = 0.2f;
        private Font f;
        */

        public ShootEmUp(int width, int height, int FPS, myCamera c) : base(width, height, FPS, c)
        {
            /*
            game = this;

            //Oculta el cursor
            Cursor.Hide();

            //Carga los sprites del juego
            background = myImageManager.Get("Background");
            playerSprite = myImageManager.Get("Spaceship");
            enemy1Sprite = myImageManager.Get("Enemy1");
            enemy2Sprite = myImageManager.Get("Enemy2");
            enemy3Sprite = myImageManager.Get("Enemy3");

            myAudioManager.Play("BackgroundSong", 0.2f);

            //Crea al jugador con su sprite
            player = new myPlayer(width / 2, height - 60, 20, playerSprite);
            Instantiate(player);

            f = myFontManager.Get("CourierPrime-Regular", 15);
            */
        }

        /*
        //el spawner de cada enemigo
        public void SpawnEnemy1(float x, float y, float radius)
        {
            Instantiate(new Enemy1(x, y, radius, Color.Red, enemy1Sprite));
        }

        public void SpawnEnemy2(float x, float y, float width, float height)
        {
            Instantiate(new Enemy2(x, y, width, height, Color.Blue, enemy2Sprite, player, this));
        }

        public void SpawnEnemy3(float x, float y, float radius)
        {
            Instantiate(new Enemy3(x, y, radius, Color.Purple, enemy3Sprite, this));
        }
        */

        protected override void ProcessInput()
        {
            /*
            if (gameOver && myInputManager.isLeftButtonDown)
            {

                //Reinicia estado del juego
                gameOver = false;
                lives = 3;
                score = 0;
                gameTime = 0;

                //Reinicia al jugador
                player.x = windowWidth / 2;
                player.y = windowHeight - 60;
                player.velocityX = 0;
                player.velocityY = 0;

                //Vuelve a activar la gravedad
                if (player is myPhysicsGameObject physicsPlayer)
                {
                    physicsPlayer.affectedByGravity = false;
                }
            }

            if (gameOver) return;

            //Mueve al jugador con el mouse
            player.MoveTo(myInputManager.mousePosition.X - currentCamera.x);

            //Dispara
            if (myInputManager.isLeftButtonDown && shootCooldown <= 0)
            {
                Instantiate(player.Shoot());
                shootCooldown = SHOOT_DELAY;
                myAudioManager.Play("shoot", SFXvolume);
            }

            shootCooldown -= deltaTime;
            */
        }

        protected override void RenderGame(Graphics g)
        {
            /*
            //Dibuja la UI
            g.DrawString($"Vidas: {lives}", f, Brushes.White, 10, 10);
            g.DrawString($"Puntaje: {score}", f, Brushes.White, 10, 30);
            g.DrawString($"Tiempo: {gameTime:F1}s", f, Brushes.White, 10, 50);
            g.DrawString($"Balas: {gameObjects.Count(o => o is myBullet)}", f, Brushes.White, 10, 70);

            if (gameOver)
            {
                var message = "GAME OVER - Click para reiniciar";
                var size = g.MeasureString(message, f);
                g.DrawString(message, f, Brushes.Red, (windowWidth - size.Width) / 2, (windowHeight - size.Height) / 2);
            }
            */
        }

        /*
        protected override void DrawBackground(Graphics g)
        {
            if (background != null)
            {
                //Dibuja el fondo adaptado al tamaño de la ventana
                g.DrawImage(background, 0, 0, windowWidth, windowHeight);
            }
        }

        public void LoseLife()
        {
            lives--;
            myAudioManager.Play("cruch", SFXvolume);
        }
        */

        protected override void Update()
        {
            /*
            //Listas temporales para objetos a destruir
            var enemiesToDestroy = new List<myPhysicsGameObject>();
            var bulletsToDestroy = new List<myBullet>();
            var objectsToRemove = new List<myGameObject>(gameObjects);

            if (score < 0 || lives <= 0) 
            {
                myAudioManager.Play("explosion", 1.5f);
                gameOver = true;

                foreach (var obj in objectsToRemove)
                {
                    //No destruye al jugador
                    if (obj != player)
                    {
                        Destroy(obj);
                    }
                }

                //Limpia cualquier lista temporal de objetos pendientes
                bulletsToDestroy.Clear();
                enemiesToDestroy.Clear();
            }
            if (gameOver) return;

            gameTime += deltaTime;

           
            bool playerHit = false;
            Random random = new Random();
            float rand, spawnX;

            //Genera enemigos aleatorios
            if (random.NextDouble() < 0.02f)
            {
                rand = (float)random.NextDouble();
                spawnX = player.x < windowWidth / 2 ?
                    random.Next(windowWidth / 2, windowWidth - 20) :
                    random.Next(0, windowWidth / 2);

                if (rand < 0.9f) SpawnEnemy1(spawnX, -30, 20);
                else if (rand < 0.99f) SpawnEnemy2(spawnX, -40, 35, 35);
                else SpawnEnemy3(spawnX, -60, 50);
            }

            //Destruye las balas marcadas
            foreach (var bullet in bulletsToDestroy)
            {
                Destroy(bullet);
            }

            //Detecta colisiones
            foreach (var obj in gameObjects)
            {
                if (obj is myBullet bullet)
                {
                    //Destruye balas que salen de pantalla
                    if (bullet.y < -bullet.sizeY || bullet.y > windowHeight)
                    {
                        bulletsToDestroy.Add(bullet);
                        continue;
                    }

                    foreach (var enemy in gameObjects.Where(o => o is Enemy1 || o is Enemy2 || o is Enemy3))
                    {
                        //Se revisa cual enemigo destruir
                        if (bullet.IsColliding((myPhysicsGameObject)enemy))
                        {
                            if (enemy is Enemy1)
                            {
                                score += 10;
                                myAudioManager.Play("shipcrash", SFXvolume);
                                enemiesToDestroy.Add((myPhysicsGameObject)enemy);
                            }
                            else if (enemy is Enemy2 e2)
                            {
                                e2.Hit();
                                myAudioManager.Play("shipcrash", SFXvolume);
                                if (e2.Hits >= 3)
                                {
                                    score += 30;
                                    myAudioManager.Play("explosion1", SFXvolume);
                                    enemiesToDestroy.Add(e2);
                                }
                            }
                            else if (enemy is Enemy3 e3)
                            {
                                e3.Hit();
                                myAudioManager.Play("shipcrash", SFXvolume);
                                if (e3.Hits >= 5)
                                {
                                    score += 50;
                                    myAudioManager.Play("explosion2", SFXvolume);
                                    enemiesToDestroy.Add(e3);
                                }
                            }

                            bulletsToDestroy.Add(bullet);
                            break;
                        }
                    }
                }
                else if (obj == player && !playerHit)
                {
                    foreach (var enemy in gameObjects.Where(o => o is Enemy1 || o is Enemy2 || o is Enemy3))
                    {
                        if (player.IsColliding((myPhysicsGameObject)enemy))
                        {
                            LoseLife();
                            enemiesToDestroy.Add((myPhysicsGameObject)enemy);
                            playerHit = true;
                            break;
                        }
                    }
                }
                //Detecta enemigos que tocan el suelo
                if (obj is Enemy1 enemy1 && enemy1.y > windowHeight)
                {
                    enemiesToDestroy.Add(enemy1);
                    score -= 10;
                }
                else if (obj is Enemy2 enemy2 && enemy2.y > windowHeight)
                {
                    enemiesToDestroy.Add(enemy2);
                    score -= 30;
                }
                else if (obj is Enemy3 enemy3 && enemy3.y > windowHeight)
                {
                    enemiesToDestroy.Add(enemy3);
                    score -= 50;
                }
            }

            //Destruye objetos marcados
            foreach (var bullet in bulletsToDestroy) Destroy(bullet);
            foreach (var enemy in enemiesToDestroy) Destroy(enemy);
            */
        }
    }
}
