using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using P2DEngine.GameObjects;
using P2DEngine.GameObjects.Enemigos;
using P2DEngine.Managers;

namespace P2DEngine.Games
{
    public class CopperGear : myGame //Juan Jose Valdebenito
    {
        private myPlayer player;
        private List<myBullet> bullets;
        private List<Room> rooms;
        private Image background;
        private const float SHOOT_DELAY = 0.2f;
        private float shootCooldown = 0; //Para controlar el disparo
        private const int MAX_BULLETS = 5; //Limite de balas activas
        private bool isGameOver = false;
        private int maxAmmo = 25;
        private int currentAmmo = 25;
        private bool hasWon = false;
        private int currentRoom = 0;
        private float gameOverTime = -1f;
        private bool playedGameOverSound = false;
        private bool playedWinSound = false;
        private int musicIndex = -1;

        //Clase interna para definir una habitación
        private class Room
        {
            public List<WallBlock> walls = new List<WallBlock>();
            public List<EnemyBase> enemies = new List<EnemyBase>();
            public Teleporter teleporter;
            public Teleporter winTeleporter;
        }

        //Clase interna para lo teletransportadores
        private class Teleporter : myPhysicsBlock
        {
            public int targetRoom;
            public Teleporter(float x, float y, float sizeX, float sizeY, int targetRoom)
                : base(x, y, sizeX, sizeY, Color.Gray) { this.targetRoom = targetRoom; }
            public override void Draw(Graphics g, Vector position, Vector size)
            {
                g.FillRectangle(brush, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
                using (var pen = new Pen(Color.Black, 2))
                    g.DrawRectangle(pen, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
        }

        private void SetupRooms()
        {
            rooms = new List<Room>();
            //Room 0: Abajo izquierda
            var r0 = new Room();
            r0.walls.Add(new WallBlock(0, 0, 800, 20)); //top
            r0.walls.Add(new WallBlock(0, 580, 800, 20)); //bottom
            r0.walls.Add(new WallBlock(0, 0, 20, 600)); //left
            r0.walls.Add(new WallBlock(780, 0, 20, 600)); //right
            //Pared interna simple
            r0.walls.Add(new WallBlock(200, 200, 400, 20));
            r0.walls.Add(new WallBlock(200, 400, 400, 20));
            r0.walls.Add(new WallBlock(400, 220, 20, 180));
            r0.walls.Add(new WallBlock(100, 400, 20, 200));
            r0.teleporter = new Teleporter(780, 260, 20, 80, 1);
            r0.enemies.Add(new Enemy1(80, 80, 34, 60, Color.Transparent, player, this, 70f, true));
            r0.enemies.Add(new Enemy1(600, 400, 34, 60, Color.Transparent, player, this, 70f, false));
            rooms.Add(r0);

            //Room 1: Abajo derecha
            var r1 = new Room();
            r1.walls.Add(new WallBlock(0, 0, 800, 20));
            r1.walls.Add(new WallBlock(0, 580, 800, 20));
            r1.walls.Add(new WallBlock(0, 0, 20, 600));
            r1.walls.Add(new WallBlock(780, 0, 20, 600));
            //Mas paredes internas
            r1.walls.Add(new WallBlock(200, 300, 400, 20));
            r1.walls.Add(new WallBlock(400, 100, 20, 200));
            //Teleportador secreto a room 3
            var secretTp = new Teleporter(780, 100, 20, 80, 3);
            r1.teleporter = new Teleporter(380, 0, 40, 20, 2); //arriba a room 2
            r1.enemies.Add(new Enemy1(200, 400, 34, 60, Color.Transparent, player, this, 80f, true));
            r1.enemies.Add(new Enemy2(600, 100, 34, 60, Color.Transparent, player, this, 100f, false));
            r1.enemies.Add(new Enemy1(300, 200, 34, 60, Color.Transparent, player, this, 90f, true));
            rooms.Add(r1);

            //Room 2: Jefe Final
            var r2 = new Room();
            r2.walls.Add(new WallBlock(0, 0, 800, 20));
            r2.walls.Add(new WallBlock(0, 580, 800, 20));
            r2.walls.Add(new WallBlock(0, 0, 20, 600));
            r2.walls.Add(new WallBlock(780, 0, 20, 600));
            //Paredes internas complejas
            r0.walls.Add(new WallBlock(300, 200, 200, 20));

            r2.enemies.Add(new Enemy2(100, 400, 34, 60, Color.Transparent, player, this, 100f, true));
            r2.enemies.Add(new Enemy2(600, 400, 34, 60, Color.Transparent, player, this, 100f, false));

            r2.enemies.Add(new Enemy3(100, 100, 90, 120, Color.Purple, player, this, 150f, false, true));
            rooms.Add(r2);

            //Room 3: Secreta (extra)
            var r3 = new Room();
            r3.walls.Add(new WallBlock(0, 0, 800, 20));
            r3.walls.Add(new WallBlock(0, 580, 800, 20));
            r3.walls.Add(new WallBlock(0, 0, 20, 600));
            r3.walls.Add(new WallBlock(780, 0, 20, 600));
            r3.walls.Add(new WallBlock(200, 200, 400, 20));
            r3.walls.Add(new WallBlock(400, 300, 20, 200));
            //Teleportador de regreso a room 1 (izquierda)
            r3.teleporter = new Teleporter(0, 100, 20, 80, 1);
            r3.enemies.Add(new Enemy1(400, 400, 34, 60, Color.Transparent, player, this, 130f, true));
            r3.enemies.Add(new Enemy1(200, 100, 34, 60, Color.Transparent, player, this, 110f, false));
            r3.enemies.Add(new Enemy1(600, 200, 34, 60, Color.Transparent, player, this, 110f, true));
            rooms.Add(r3);

            //Instanciar el teleporter secreto en room 1 (como teleporter adicional)
            rooms[1].winTeleporter = secretTp;
        }

        private void LoadRoom(int idx)
        {
            //Cambia la musica si es la sala del jefe
            if (musicIndex != -1)
            {
                myAudioManager.Stop(musicIndex);
                musicIndex = -1;
            }

            if (idx == 2) //Sala del jefe
            {
                musicIndex = myAudioManager.Play("boss-music", 0.6f);
            }
            else
            {
                musicIndex = myAudioManager.Play("music", 0.6f);
            }

            //Limpia objetos actuales (excepto jugador y balas)
            var toRemove = gameObjects.Where(o => o != player && !(o is myBullet)).ToList();
            foreach (var obj in toRemove) Destroy(obj);
            //Instancia paredes, enemigos y teletransportador
            foreach (var wall in rooms[idx].walls) Instantiate(wall);
            foreach (var enemy in rooms[idx].enemies) Instantiate(enemy);
            if (rooms[idx].teleporter != null) Instantiate(rooms[idx].teleporter);
            if (rooms[idx].winTeleporter != null) Instantiate(rooms[idx].winTeleporter);
        }

        public CopperGear(int width, int height, int FPS, myCamera c) : base(width, height, FPS, c)
        {
            player = new myPlayer(45, 500, 34, 60);
            Instantiate(player);
            bullets = new List<myBullet>();
            background = myImageManager.Get("Background");
            SetupRooms();
            LoadRoom(0);
        }

        protected override void ProcessInput()
        {
            if (isGameOver || hasWon) return;
            //Procesa los disparos
            if (myInputManager.IsKeyPressed(Keys.Space) && shootCooldown <= 0 && bullets.Count < MAX_BULLETS && currentAmmo > 0)
            {
                var bullet = player.Shoot();
                shootCooldown = SHOOT_DELAY;
                bullets.Add(bullet);
                Instantiate(bullet);
                currentAmmo--;
                myAudioManager.Play("gunshot", 0.4f);
            }
            shootCooldown -= deltaTime;
        }

        public void OnBossDefeated()
        {
            hasWon = true;
            myAudioManager.Play("boss-defeated", 0.8f); //Sonido especial de victoria
        }

        protected override void Update()
        {
            if (isGameOver)
            {
                if (musicIndex != -1) { myAudioManager.Stop(musicIndex); musicIndex = -1; }
                if (gameOverTime < 0)
                {
                    if (!playedGameOverSound)
                    {
                        myAudioManager.Play("gameover", 0.6f);
                        playedGameOverSound = true;
                    }
                    gameOverTime = 0;
                }
                else
                {
                    gameOverTime += deltaTime;
                    if (gameOverTime >= 5f)
                    {
                        //Resetea el juego
                        isGameOver = false;
                        hasWon = false;
                        gameOverTime = -1f;
                        currentAmmo = maxAmmo;
                        player = new myPlayer(windowWidth / 2, windowHeight / 2, 34, 60);
                        Instantiate(player);
                        bullets.Clear();
                        SetupRooms();
                        LoadRoom(0);
                        playedGameOverSound = false;
                        playedWinSound = false;
                        musicIndex = myAudioManager.Play("music", 0.3f);
                    }
                }
                return;
            }

            if (hasWon)
            {
                if (musicIndex != -1) { myAudioManager.Stop(musicIndex); musicIndex = -1; }
                if (!playedWinSound)
                {
                    myAudioManager.Play("win", 0.6f);
                    playedWinSound = true;
                }
                return;
            }

            foreach (var obj in gameObjects)
            {
                if (obj is Teleporter tp && player.IsColliding(tp))
                {
                    currentRoom = tp.targetRoom;
                    //Reposiciona al jugador segun la dirección de entrada
                    if (currentRoom == 1) { player.x = 40; player.y = 300; }
                    else if (currentRoom == 2) { player.x = 400; player.y = 500; }
                    else if (currentRoom == 3) { player.x = 40; player.y = 300; }
                    else { player.x = 40; player.y = 300; }
                    LoadRoom(currentRoom);
                    return;
                }
            }

            float prevX = player.x, prevY = player.y;
            player.UpdateDirection(deltaTime);
            player.KeepInBounds(windowWidth, windowHeight);
            float intendedX = player.x, intendedY = player.y;
            const float pushback = 2f;

            player.x = intendedX;
            player.y = prevY;
            foreach (var obj in gameObjects)
            {
                if (obj is WallBlock wall && player.IsColliding(wall))
                {
                    if (intendedX > prevX)
                        player.x = prevX - pushback;
                    else if (intendedX < prevX) 
                        player.x = prevX + pushback;
                    else
                        player.x = prevX;
                    break;
                }
            }

            player.y = intendedY;
            foreach (var obj in gameObjects)
            {
                if (obj is WallBlock wall && player.IsColliding(wall))
                {
                    if (intendedY > prevY) 
                        player.y = prevY - pushback;
                    else if (intendedY < prevY) 
                        player.y = prevY + pushback;
                    else
                        player.y = prevY;
                    break;
                }
            }

            var bulletsCopy = bullets.ToList();
            for (int i = bulletsCopy.Count - 1; i >= 0; i--)
            {
                bool destroy = false;
                if (bulletsCopy[i].y < 0 || bulletsCopy[i].y > windowHeight ||
                    bulletsCopy[i].x < 0 || bulletsCopy[i].x > windowWidth)
                {
                    destroy = true;
                }
                else
                {
                    foreach (var obj in gameObjects)
                    {
                        if (obj is WallBlock wall && bulletsCopy[i].IsColliding(wall))
                        {
                            destroy = true;
                            break;
                        }
                    }
                }
                if (destroy)
                {
                    Destroy(bulletsCopy[i]);
                    bullets.Remove(bulletsCopy[i]);
                }
            }

            //Lista para destruccion
            var enemiesToDestroy = new List<EnemyBase>();
            var bulletsToDestroy = new List<myBullet>();
            var medkitsToDestroy = new List<Medkit>();
            var enemyBulletsToDestroy = new List<myEnemyBullet>();

            //Copia temporal de gameObjects para evitar modificar la colección durante la iteración
            var gameObjectsCopy = gameObjects.ToList();
            foreach (var obj in gameObjectsCopy)
            {
                if (obj is EnemyBase enemy)
                {
                    //Colision con jugador
                    if (player.IsColliding(enemy))
                    {
                        myAudioManager.Play("cruch", 0.2f);
                        player.PerderVida(20);
                    }
                    //Colision con balas del jugador
                    foreach (var bullet in bulletsCopy)
                    {
                        if (enemy.IsColliding(bullet))
                        {
                            myAudioManager.Play("cruch", 0.2f);
                            enemy.Die();
                            bulletsToDestroy.Add(bullet);
                            break;
                        }
                    }
                }
                //Colision jugador con botiquin
                else if (obj is Medkit medkit)
                {
                    if (player.IsColliding(medkit) && player.Vida >= 70)
                    {
                        player.SetLife(); //Cura vida hasta llegar a 100
                        medkitsToDestroy.Add(medkit);
                        myAudioManager.Play("health", 0.3f);
                    }
                    else if (player.IsColliding(medkit))
                    {
                        player.PerderVida(-30); //Cura 30 de vida
                        medkitsToDestroy.Add(medkit);
                        myAudioManager.Play("health", 0.3f);
                    }
                }
                //Colision jugador con balas enemigas
                else if (obj is myEnemyBullet enemyBullet)
                {
                    if (player.IsColliding(enemyBullet))
                    {
                        myAudioManager.Play("cruch", 0.2f);
                        player.PerderVida(25);
                        enemyBulletsToDestroy.Add(enemyBullet);
                    }
                }
            }

            //Destruir objetos marcados
            foreach (var e in enemiesToDestroy) Destroy(e);
            foreach (var b in bulletsToDestroy) { Destroy(b); bullets.Remove(b); }
            foreach (var m in medkitsToDestroy) Destroy(m);
            foreach (var eb in enemyBulletsToDestroy) Destroy(eb);

            //Si la vida llega a 0, termina el juego
            if (player.Vida <= 0)
            {
                isGameOver = true;
            }
        }

        protected override void DrawBackground(Graphics g)
        {
            if (background != null)
            {
                g.DrawImage(background, 0, 0, windowWidth, windowHeight);
            }
            else
            {
                //Fondo por defecto si no hay imagen
                g.Clear(Color.Black);
            }
        }

        protected override void RenderGame(Graphics g)
        {
            //Barra de vida visual
            int barX = 30, barY = 30, barW = 200, barH = 24;
            float lifePercent = player.Vida / 100f;
            g.FillRectangle(Brushes.White, barX, barY, barW, barH);
            g.FillRectangle(Brushes.Red, barX, barY, (int)(barW * lifePercent), barH);
            using (var pen = new Pen(Color.Black, 2))
                g.DrawRectangle(pen, barX, barY, barW, barH);
            g.DrawString($"{player.Vida}/100", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, barX + barW + 8, barY + 2);

            //Municion
            g.DrawString($"Balas: {currentAmmo}/25", new Font("Arial", 16), Brushes.White, 25, 60);

            if (isGameOver)
            {
                var msg = "MISSION FAILED - reiniciando en 5 secs";
                var font = new Font("Arial", 27, FontStyle.Bold);
                var size = g.MeasureString(msg, font);
                g.DrawString(msg, font, Brushes.Red, (windowWidth - size.Width) / 2, (windowHeight - size.Height) / 2);
            }
            if (hasWon)
            {
                var msg = "MISSION ACCOMPLISHED!";
                var font = new Font("Arial", 27, FontStyle.Bold);
                var size = g.MeasureString(msg, font);
                g.DrawString(msg, font, Brushes.Lime, (windowWidth - size.Width) / 2, (windowHeight - size.Height) / 2);
            }
        }
    }
}
