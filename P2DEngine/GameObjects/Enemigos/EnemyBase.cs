using System;
using System.Drawing;
using System.Windows;
using P2DEngine.Games;
using P2DEngine.Managers;

namespace P2DEngine.GameObjects
{
    public abstract class EnemyBase : myPhysicsBlock
    {
        protected myPlayer target;
        protected CopperGear game;
        protected Image[] sprites = new Image[4]; //0: front, 1: back, 2: left, 3: right
        protected int currentDirection = 0;
        protected float speed;
        protected int hits = 0;
        protected float detectionRadius = 200f;
        protected bool playerDetected = false;
        private bool isFinalBoss; //Revisa si es el jefe final

        //Propiedades para movimiento en eje fijo
        protected bool movesHorizontally; //true = eje X, false = eje Y
        protected bool movingPositive = true; //true = derecha/abajo, false = izquierda/arriba
        protected float movementRange = 300f; //Rango maximo de movimiento
        protected float initialPosition; //Posicion inicial para calcular el rango

        private float shootCooldown = ENEMY_SHOOT_DELAY; //Cooldown inicial para evitar disparo en el primer frame
        private const float ENEMY_SHOOT_DELAY = 1.0f;
        private static Random rng = new Random();

        public bool IsDead { get; private set; } = false; //Revisa si el enemigo fue eliminado

        public EnemyBase(float x, float y, float sizeX, float sizeY, Color color,
                        myPlayer target, CopperGear game, float speed, bool movesHorizontally, bool isFinalBoss)
            : base(x, y, sizeX, sizeY, color)
        {
            this.isFinalBoss = isFinalBoss;
            this.target = target;
            this.game = game;
            this.speed = speed;
            this.movesHorizontally = movesHorizontally;
            this.initialPosition = movesHorizontally ? x : y;
            affectedByGravity = false;

            LoadSprites();

            //Establecer dirección inicial segun el eje
            if (movesHorizontally)
            {
                currentDirection = movingPositive ? 3 : 2; //Derecha o izquierda
            }
            else
            {
                currentDirection = movingPositive ? 0 : 1; //Abajo o arriba
            }
        }

        protected abstract void LoadSprites();

        public int Hits => hits;
        public virtual int MaxHits => 3;

        public void Hit() //Propiedad para la cantidad de hits de un enemigo
        {
            hits++;
        }

        public void Die()
        {
            IsDead = true;
            //Probabilidad de dejar botiquín (40%)
            if (rng.NextDouble() < 0.4)
            {
                var medkit = new Medkit(x + sizeX / 2 - 10, y + sizeY / 2 - 10, 20, 20);
                game.Instantiate(medkit);
            }
            //Si es el jefe final, activa victoria
            if (isFinalBoss && game != null)
            {
                game.OnBossDefeated();
            }

            game.Destroy(this);

        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            //Dibuja el sprite
            if (sprites[currentDirection] != null)
            {
                g.DrawImage(sprites[currentDirection], (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
            else
            {
                base.Draw(g, position, size);
            }
        }

        protected void UpdateDirection()
        {
            if (playerDetected)
            {
                //Calcula direccion hacia el jugador cuando lo detecta
                float dx = target.x - x;
                float dy = target.y - y;

                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    currentDirection = dx > 0 ? 3 : 2; //Derecha o izquierda
                }
                else
                {
                    currentDirection = dy > 0 ? 0 : 1; //Abajo o arriba
                }
            }
            else
            {
                //Mantener direccion segun movimiento en eje fijo
                if (movesHorizontally)
                {
                    currentDirection = movingPositive ? 3 : 2;
                }
                else
                {
                    currentDirection = movingPositive ? 0 : 1;
                }
            }
        }

        protected bool DetectPlayer() //Detecta al jugador con un radio
        {
            float distance = (float)Math.Sqrt(Math.Pow(target.x - x, 2) + Math.Pow(target.y - y, 2));
            playerDetected = distance <= detectionRadius;
            return playerDetected;
        }

        protected void MoveInFixedAxis(float deltaTime)
        {
            if (playerDetected)
            {
                MoveTowardsPlayer(deltaTime);
                return;
            }

            //Movimiento en eje fijo clasico
            if (movesHorizontally)
            {
                x += (movingPositive ? speed : -speed) * deltaTime;

                if (Math.Abs(x - initialPosition) > movementRange)
                {
                    movingPositive = !movingPositive;
                }
                if (x <= 0 || x >= 600 - sizeX)
                {
                    movingPositive = !movingPositive;
                }
            }
            else
            {
                y += (movingPositive ? speed : -speed) * deltaTime;

                if (Math.Abs(y - initialPosition) > movementRange)
                {
                    movingPositive = !movingPositive;
                }
                if (y <= 0 || y >= 600 - sizeY)
                {
                    movingPositive = !movingPositive;
                }
            }
        }

        protected void MoveTowardsPlayer(float deltaTime)
        {
            //Movimiento solo en un eje a la vez (como el jugador)
            float dx = target.x - x;
            float dy = target.y - y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                //Mueve solo en X
                if (dx > 0)
                {
                    x += speed * deltaTime;
                    currentDirection = 3; //Derecha
                }
                else if (dx < 0)
                {
                    x -= speed * deltaTime;
                    currentDirection = 2; //Izquierda
                }
            }
            else if (Math.Abs(dy) > 0)
            {
                //Mueve solo en Y
                if (dy > 0)
                {
                    y += speed * deltaTime;
                    currentDirection = 0; //Abajo
                }
                else if (dy < 0)
                {
                    y -= speed * deltaTime;
                    currentDirection = 1; //Arriba
                }
            }
        }

        private void ShootAtPlayer()
        {
            //Dispara una bala hacia el jugador
            float dx = target.x + target.sizeX / 2 - (x + sizeX / 2);
            float dy = target.y + target.sizeY / 2 - (y + sizeY / 2);
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            if (dist == 0) dist = 1;
            float speed = 200f;
            float vx = dx / dist * speed;
            float vy = dy / dist * speed;
            var bullet = new myEnemyBullet(x + sizeX / 2, y + sizeY / 2, 5, Color.Red, null, vy, vx);
            game.Instantiate(bullet);
        }

        public override void UpdateGameObject(float deltaTime)
        {
            base.UpdateGameObject(deltaTime);

            DetectPlayer();
            UpdateDirection();
            MoveInFixedAxis(deltaTime);

            //Mantiene dentro de limites (seguridad adicional)
            x = Clamp(x, 0, 800 - sizeX);
            y = Clamp(y, 0, 600 - sizeY);

            //Dispara si ve al jugador
            if (playerDetected && !IsDead)
            {
                shootCooldown -= deltaTime;
                if (shootCooldown <= 0)
                {
                    ShootAtPlayer();
                    shootCooldown = ENEMY_SHOOT_DELAY;
                }
            }
        }

        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
