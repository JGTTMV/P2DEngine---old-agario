using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2DEngine
{
    public class Agario : myGame
    {
        Player player;
        List<Enemy> enemies = new List<Enemy>(); //Lista para manejar a los enemigos
        int enemyX;
        int enemyY;
        int enemySize; //Tamaño al azar para los enemigos
        int enemyCount = 50; //Numero de enemigos para espawnear

        myObjective Food;
        myObjective FoodA;
        myObjective FoodB;
        myObjective FoodC;

        Random rand = new Random();

        float playerSpeed = 0.6f;
        float baseStep = 30;
        float playerCenterX;
        float playerCenterY;

        float sizeFactor;
        float targetZoom;

        float oldX;
        float oldY;
        int newX, newY;
        int attempts = 0;
        bool validPosition = false;

        //Se definen variables para el modo debug
        private bool debugMode = true;
        private float debugTimer = 0.0f;
        private const float debugInterval = 0.5f;

        private float playerDistance = 0.0f;

        int pistaX = 100;
        int pistaY = 100;
        int pistaAncho = 4500;
        int pistaAlto = 4500;
        int grosorBorde = 20;
        int randomX;
        int randomY;

        int foodCount;
        int foodSpeciaACount;
        int foodSpeciaBCount;
        int foodSpeciaCCount;

        //Variables para terminar el juego
        private const int maxScore = 500;
        private bool isGameOver = false;

        //Se crean variables para los power ups
        private bool isInvulnerable = false;
        private float invulnerabilityTimer = 0.0f;
        private float speedBoostTimer = 0.0f;
        private float originalSpeed;

        private int playerScore = 0;
        private int enemiesEaten = 0;
        private int foodEaten = 0;
        private int deathsCount = 0;
        private float respawnTimer = 0f;
        private const float respawnDelay = 0.5f; // 2 segundos de espera antes de respawn

        public Agario(int width, int height, int FPS, myCamera c) : base(width, height, FPS, c)
        {
            currentCamera = new myCamera(0, 0, 800, 600, 1.0f);
            currentCamera.SetTargetPlayer(player);

            Color colorBorde = Color.DarkGray;

            player = new Player(pistaX + pistaAncho / 2, pistaY + pistaAlto / 2, 20, 20, Color.Red);

            Instantiate(new myBlock(pistaX, pistaY, pistaAncho, grosorBorde, colorBorde));
            Instantiate(new myBlock(pistaX, pistaY + pistaAlto - grosorBorde, pistaAncho, grosorBorde, colorBorde));
            Instantiate(new myBlock(pistaX, pistaY, grosorBorde, pistaAlto, colorBorde));
            Instantiate(new myBlock(pistaX + pistaAncho - grosorBorde, pistaY, grosorBorde, pistaAlto, colorBorde));

            //Se instauran distintas cantidades de cada comida

            foodCount = 500;

            for (int i = 0; i < foodCount; i++)
            {
                randomX = rand.Next(pistaX + grosorBorde, pistaX + pistaAncho - grosorBorde);
                randomY = rand.Next(pistaY + grosorBorde, pistaY + pistaAlto - grosorBorde);

                Food = new myObjective(randomX, randomY, 10, 10, Color.Yellow, true);
                Instantiate(Food);
            }

            foodSpeciaACount = 25;

            for (int i = 0; i < foodSpeciaACount; i++)
            {
                randomX = rand.Next(pistaX + grosorBorde, pistaX + pistaAncho - grosorBorde);
                randomY = rand.Next(pistaY + grosorBorde, pistaY + pistaAlto - grosorBorde);

                FoodA = new myObjective(randomX, randomY, 15, 15, Color.Green, true);
                Instantiate(FoodA);
            }
            foodSpeciaBCount = 50;

            for (int i = 0; i < foodSpeciaBCount; i++)
            {
                randomX = rand.Next(pistaX + grosorBorde, pistaX + pistaAncho - grosorBorde);
                randomY = rand.Next(pistaY + grosorBorde, pistaY + pistaAlto - grosorBorde);
                FoodB = new myObjective(randomX, randomY, 20, 20, Color.Red, true);
                Instantiate(FoodB);
            }

            foodSpeciaCCount = 50;

            for (int i = 0; i < foodSpeciaCCount; i++)
            {
                randomX = rand.Next(pistaX + grosorBorde, pistaX + pistaAncho - grosorBorde);
                randomY = rand.Next(pistaY + grosorBorde, pistaY + pistaAlto - grosorBorde);
                FoodC = new myObjective(randomX, randomY, 5, 5, Color.Blue, true);
                Instantiate(FoodC);
            }

            Instantiate(player);

            //Se inicializan multiples enemigos
            for (int i = 0; i < enemyCount; i++)
            {
                enemyX = rand.Next(pistaX + grosorBorde, pistaX + pistaAncho - grosorBorde);
                enemyY = rand.Next(pistaY + grosorBorde, pistaY + pistaAlto - grosorBorde);
                enemySize = rand.Next(10, 150); //Tamaño al azar para los enemigos
                Color enemyColor;

                //Se asegura que el enemigo no tenga el mismo color que el jugador
                do
                {
                    enemyColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
                } while (enemyColor == player.brush.Color);

                Enemy newEnemy = new Enemy(enemyX, enemyY, enemySize, enemySize, enemyColor);
                enemies.Add(newEnemy);
                Instantiate(newEnemy);
            }
        }

        //Funcion especificamente para la colision
        private bool IsColliding(myGameObject a, myGameObject b)
        {
            float aLeft = a.x;
            float aRight = a.x + a.sizeX;
            float aTop = a.y;
            float aBottom = a.y + a.sizeY;

            float bLeft = b.x;
            float bRight = b.x + b.sizeX;
            float bTop = b.y;
            float bBottom = b.y + b.sizeY;

            return (aRight > bLeft && aLeft < bRight && aBottom > bTop && aTop < bBottom);
        }

        private bool WillCollide(Player player, int deltaX, int deltaY)
        {
            float newX = player.x + deltaX;
            float newY = player.y + deltaY;

            Rectangle playerRect = new Rectangle((int)newX, (int)newY, (int)player.sizeX, (int)player.sizeY);

            foreach (var obj in gameObjects)
            {
                if (obj is myBlock block && !(block is Player) && !(block is myObstacle) && !(block is myObjective) && !(block is Enemy))
                {
                    Rectangle blockRect = new Rectangle((int)block.x, (int)block.y, (int)block.sizeX, (int)block.sizeY);
                    if (playerRect.IntersectsWith(blockRect))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void ProcessInput()
        {
            if (isGameOver)
                return;

            if (myInputManager.IsKeyPressed(Keys.W) && !WillCollide(player, 0, -(int)(baseStep * player.speed)))
            {
                player.y -= (int)(baseStep * player.speed);
            }
            if (myInputManager.IsKeyPressed(Keys.A) && !WillCollide(player, -(int)(baseStep * player.speed), 0))
            {
                player.x -= (int)(baseStep * player.speed);
            }
            if (myInputManager.IsKeyPressed(Keys.S) && !WillCollide(player, 0, (int)(baseStep * player.speed)))
            {
                player.y += (int)(baseStep * player.speed);
            }
            if (myInputManager.IsKeyPressed(Keys.D) && !WillCollide(player, (int)(baseStep * player.speed), 0))
            {
                player.x += (int)(baseStep * player.speed);
            }

            //Controles alternativos
            if (myInputManager.IsKeyPressed(Keys.Up) && !WillCollide(player, 0, -(int)(baseStep * player.speed)))
            {
                player.y -= (int)(baseStep * player.speed);
            }
            if (myInputManager.IsKeyPressed(Keys.Left) && !WillCollide(player, -(int)(baseStep * player.speed), 0))
            {
                player.x -= (int)(baseStep * player.speed);
            }
            if (myInputManager.IsKeyPressed(Keys.Down) && !WillCollide(player, 0, (int)(baseStep * player.speed)))
            {
                player.y += (int)(baseStep * player.speed);
            }
            if (myInputManager.IsKeyPressed(Keys.Right) && !WillCollide(player, (int)(baseStep * player.speed), 0))
            {
                player.x += (int)(baseStep * player.speed);
            }

            if (myInputManager.IsKeyPressed(Keys.P))
                debugMode = true;
            else
                debugMode = false;
        }

        void PlayerGrowth(float amount = 1f)
        {
            player.sizeX += amount;
            player.sizeY += amount;
            baseStep -= 0.05f * amount;

            if (baseStep < 1) baseStep = 1;

            //Aumenta puntaje al comer
            foodEaten++;
            playerScore += (int)(amount * 10);
        }

        protected override void RenderGame(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, windowWidth, windowHeight);

            //Dibuja objetos del juego
            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(g,
                    currentCamera.GetViewPosition(gameObject.x, gameObject.y),
                    currentCamera.GetViewSize(gameObject.sizeX, gameObject.sizeY));
            }

            //Dibuja la interfaz
            Font uiFont = new Font("Arial", 12);
            string scoreText = $"Puntaje: {playerScore} | Enemigos: {enemiesEaten} | Comida: {foodEaten} | Muertes: {deathsCount}";
            g.DrawString(scoreText, uiFont, Brushes.White, 10, 10);

            //Muestra el mensaje de Game Over si es necesario
            if (isGameOver)
            {
                Font gameOverFont = new Font("Arial", 24, FontStyle.Bold);
                string gameOverText = "GAME OVER";
                SizeF textSize = g.MeasureString(gameOverText, gameOverFont);
                g.DrawString(gameOverText, gameOverFont, Brushes.Red,
                             (windowWidth - textSize.Width) / 2,
                             (windowHeight - textSize.Height) / 2);
            }

            //Muestra los efectos de power up si estan activos
            if (isInvulnerable)
            {
                string invulnText = $"INVULNERABLE: {invulnerabilityTimer:F1}s";
                g.DrawString(invulnText, uiFont, Brushes.Gold, 10, 40);
            }

            if (speedBoostTimer > 0)
            {
                string speedText = $"VELOCIDAD: {speedBoostTimer:F1}s";
                g.DrawString(speedText, uiFont, Brushes.Cyan, 10, 70);
            }
        }

        //Funcion de la clase MathHelper, solo se instancia como funcion para ahorrar tiempo
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        private void GameOver()
        {
            isGameOver = true;
            deathsCount++;

            //Muestra el mensaje de Game Over
            Console.WriteLine($"GAME OVER! El jugador fue comido. Muertes totales: {deathsCount}");
            Console.WriteLine($"Puntaje total: {playerScore} | Enemigos comidos: {enemiesEaten} | Comida consumida: {foodEaten}");

            player.SetColor(Color.Gray); //Se cambia color a gris para indicar la muerte
        }

        private void ResetGame()
        {
            //Guarda posicion actual para efecto visual
            oldX = player.x;
            oldY = player.y;

            //Resetea al jugador
            Destroy(player);

            //Se reinician variables
            attempts = 0;
            validPosition = false;

            do
            {
                newX = rand.Next(pistaX + grosorBorde, pistaX + pistaAncho - grosorBorde - 20);
                newY = rand.Next(pistaY + grosorBorde, pistaY + pistaAlto - grosorBorde - 20);
                attempts++;

                //Verifica que no esté demasiado cerca de enemigos
                validPosition = true;
                foreach (var enemy in enemies)
                {
                    float distance = (float)Math.Sqrt(Math.Pow(newX - enemy.x, 2) + Math.Pow(newY - enemy.y, 2));
                    if (distance < 150) //150 píxeles de distancia mínima
                    {
                        validPosition = false;
                        break;
                    }
                }

            } while (!validPosition && attempts < 20);

            player = new Player(newX, newY, 20, 20, Color.Red);
            player.speed = playerSpeed; //Resetea la velocidad
            Instantiate(player);

            //Resetea el puntaje y tamaño
            playerScore = 0;
            enemiesEaten = 0;
            foodEaten = 0;

            Console.WriteLine($"Jugador renacido en ({newX}, {newY})");
        }

        protected override void Update()
        {
            //Se reinicia el jugador al morir
            if (isGameOver)
            {
                respawnTimer += deltaTime;
                if (respawnTimer >= respawnDelay)
                {
                    ResetGame();
                    isGameOver = false;
                    respawnTimer = 0f;
                }
                return;
            }

            debugTimer += deltaTime;

            //Maneja el timer de invulnerabilidad
            if (isInvulnerable)
            {
                invulnerabilityTimer -= deltaTime;
                if (invulnerabilityTimer <= 0)
                {
                    isInvulnerable = false;
                    player.SetColor(Color.Red); //Resetea el color del jugador
                    Console.WriteLine("La invulnerabilidad ha terminado.");
                }
            }

            //Maneja el timer de velocidad
            if (speedBoostTimer > 0)
            {
                speedBoostTimer -= deltaTime;
                if (speedBoostTimer <= 0)
                {
                    player.speed = originalSpeed; //Resetea velocidad
                    player.SetColor(Color.Red); //Resetea el color del jugador
                    Console.WriteLine("Efecto de velocidad termino.");
                }
            }

            sizeFactor = (player.sizeX + player.sizeY) / 40.0f;
            targetZoom = Math.Max(0.5f, Math.Min(1.5f, 1.0f / sizeFactor));

            currentCamera.zoom = Lerp(currentCamera.zoom, targetZoom, 0.05f);

            playerCenterX = player.x + player.sizeX / 2;
            playerCenterY = player.y + player.sizeY / 2;

            currentCamera.x = playerCenterX - (windowWidth / (2 * currentCamera.zoom));
            currentCamera.y = playerCenterY - (windowHeight / (2 * currentCamera.zoom));

            List<myObjective> foodToRemove = new List<myObjective>();

            //Primero revisa alimentos especiales y los prioritiza
            foreach (var gameObject in gameObjects)
            {
                if (gameObject is myObjective food && 
                    (food.brush.Color == Color.Green || food.brush.Color == Color.Red || food.brush.Color == Color.Blue))
                {
                    if (IsColliding(player, food))
                    {
                        //Aplica los efectos basado en el color de la comida
                        if (food.brush.Color == Color.Green) // FoodA - Teleportacion
                        {
                            //Limita la teleportacion para prevenir teleportaciones extremas
                            int maxTeleportDistance = 300;
                            int currentX = (int)player.x;
                            int currentY = (int)player.y;
                            
                            //Genera una posicion controlada usando maxTeleportDistance
                            int newX = currentX + rand.Next(-maxTeleportDistance, maxTeleportDistance);
                            int newY = currentY + rand.Next(-maxTeleportDistance, maxTeleportDistance);
                            
                            //Se asegura que la posicion generada no este fuera de los bordes
                            newX = Math.Max(pistaX + grosorBorde, Math.Min(pistaX + pistaAncho - grosorBorde - (int)player.sizeX, newX));
                            newY = Math.Max(pistaY + grosorBorde, Math.Min(pistaY + pistaAlto - grosorBorde - (int)player.sizeY, newY));
                            
                            player.x = newX;
                            player.y = newY;
                            Console.WriteLine($"Jugador teletransportado desde ({currentX},{currentY}) hasta ({newX},{newY})");
                        }
                        else if (food.brush.Color == Color.Red) // FoodB - Invulnerabilidad
                        {
                            //Reduce la duracion de invulnerabilidad de 5 a 3 segundos
                            isInvulnerable = true;
                            invulnerabilityTimer = 3.0f; 
                            player.SetColor(Color.Gold);
                            Console.WriteLine("El jugador tiene invulnerabilidad acortada (3s) gracias a FoodB.");
                        }
                        else if (food.brush.Color == Color.Blue) // FoodC - Velocidad
                        {
                            //Reduce el boost de velocidad de 50% a 30% y su duracion de 5 a 3 seconds
                            originalSpeed = player.speed;
                            player.speed *= 1.2f;
                            speedBoostTimer = 3.0f;
                            player.SetColor(Color.Cyan);
                            Console.WriteLine("El jugador tiene tiene un boost de velocidad mas corto (20% por 3s) gracias a FoodC.");
                        }

                        //El jugador aun crece, pero es reducido comparado al de la comida normal
                        player.sizeX += 0.5f;
                        player.sizeY += 0.5f;
                        
                        foodToRemove.Add(food);
                    }
                }
            }

            //Revisa la comida normal
            if (foodToRemove.Count == 0) //Solo procesa comida normal si no se consumio comida especial
            {
                foreach (var gameObject in gameObjects)
                {
                    if (gameObject is myObjective food && food.brush.Color == Color.Yellow)
                    {
                        if (IsColliding(player, food))
                        {
                            PlayerGrowth(); //Aplica el crecimiento reducido
                            foodToRemove.Add(food);
                        }
                    }
                }
            }

            foreach (var food in foodToRemove)
            {
                Destroy(food);
                Console.WriteLine($"Alimento consumido: {food.brush.Color}");

                do
                {
                    randomX = rand.Next(pistaX + grosorBorde, pistaX + pistaAncho - grosorBorde);
                    randomY = rand.Next(pistaY + grosorBorde, pistaY + pistaAlto - grosorBorde);
                } while (Math.Abs(randomX - player.x) < player.sizeX * 2 && Math.Abs(randomY - player.y) < player.sizeY * 2);

                Instantiate(new myObjective(randomX, randomY, (int)food.sizeX, (int)food.sizeY, food.brush.Color, true));
            }

            //Actualiza los enemigos
            List<Enemy> enemiesToRemove = new List<Enemy>();
            List<Enemy> enemyCollisionCheck = new List<Enemy>(enemies);

            foreach (var enemy in enemies)
            {
                //Actualiza el movimiento del enemigo
                enemy.Update(deltaTime);

                //Revisa las colisiones con los bordes y su rebote
                if (enemy.x <= pistaX + grosorBorde)
                {
                    enemy.x = pistaX + grosorBorde;
                    enemy.ReverseXDirection();
                }
                else if (enemy.x + enemy.sizeX >= pistaX + pistaAncho - grosorBorde)
                {
                    enemy.x = pistaX + pistaAncho - grosorBorde - enemy.sizeX;
                    enemy.ReverseXDirection();
                }

                if (enemy.y <= pistaY + grosorBorde)
                {
                    enemy.y = pistaY + grosorBorde;
                    enemy.ReverseYDirection();
                }
                else if (enemy.y + enemy.sizeY >= pistaY + pistaAlto - grosorBorde)
                {
                    enemy.y = pistaY + pistaAlto - grosorBorde - enemy.sizeY;
                    enemy.ReverseYDirection();
                }

                //Revisa la colision con el jugador
                if (IsColliding(player, enemy))
                {
                    float playerArea = player.sizeX * player.sizeY;
                    float enemyArea = enemy.sizeX * enemy.sizeY;

                    if (playerArea > enemyArea * 1.05) //El jugador es más grande
                    {
                        PlayerGrowth();
                        enemiesEaten++;
                        playerScore += (int)(enemyArea / 100); //Puntaje basado en tamaño del enemigo
                        enemiesToRemove.Add(enemy);
                        Console.WriteLine($"Player ate an enemy! Score: {playerScore}");
                    }
                    else if (enemyArea > playerArea * 1.05 && !isInvulnerable) //El enemigo es más grande
                    {
                        GameOver();
                        return;
                    }
                }

                //Quita el enemigo del check de clolisiones para evitar chequeos duplicados
                enemyCollisionCheck.Remove(enemy);
            }

            //Quita a los enemigos consumidos y los reemplaza
            foreach (var enemy in enemiesToRemove)
            {
                Destroy(enemy);
                enemies.Remove(enemy);
                SpawnNewEnemy();
            }

            if (debugTimer >= debugInterval && debugMode)
            {
                Console.Clear();

                Console.WriteLine($"Tamaño Jugador: {player.sizeX:F1} x {player.sizeY:F1}");
                Console.WriteLine($"Velocidad Jugador: {baseStep:F2} (modificador: {player.speed:F2})");
                Console.WriteLine($"Invulnerabilidad: {(isInvulnerable ? $"Activo ({invulnerabilityTimer:F1}s restantes)" : "Inactivo")}");
                Console.WriteLine($"Velocidad: {(speedBoostTimer > 0 ? $"Activo ({speedBoostTimer:F1}s restantes)" : "Inactivo")}");
                Console.WriteLine($"Posicion: ({player.x:F0}, {player.y:F0})");

                debugTimer = 0.0f;
            }
        }

        private void SpawnNewEnemy()
        {
            //Establece tamaños minimos
            int minSize = 10;
            int maxSize = 150;
 
            //Genera una variedad de tamaños para el enemigo
            int enemySize;
            double sizeChance = rand.NextDouble();
            //Encuentra una posicion de spawn lejos del jugador
            int enemyX, enemyY;
            float minDistanceFromPlayer = 200; //Distancia minima con el jugador
            int attempts = 0;
            int maxAttempts = 20; //Evita loops infinitos

            if (sizeChance < 0.6) //60% de probabilidad de enemigos pequeños
            {
                enemySize = rand.Next(minSize, (minSize + maxSize) / 2);
            }
            else if (sizeChance < 0.9) //30% de probabilidad de enemigos medianos
            {
                enemySize = rand.Next((minSize + maxSize) / 2, (3 * maxSize + minSize) / 4);
            }
            else //10% de probabilidad de enemigos grandes
            {
                enemySize = rand.Next((3 * maxSize + minSize) / 4, maxSize);
            }
            
            do
            {
                enemyX = rand.Next(pistaX + grosorBorde + 10, pistaX + pistaAncho - grosorBorde - enemySize - 10);
                enemyY = rand.Next(pistaY + grosorBorde + 10, pistaY + pistaAlto - grosorBorde - enemySize - 10);
                attempts++;
                
                //Si ya van demasiados intentos, se usa esta posicion
                if (attempts >= maxAttempts)
                    break;
                    
            } while (Math.Sqrt(Math.Pow(enemyX - player.x, 2) + Math.Pow(enemyY - player.y, 2)) < minDistanceFromPlayer);
            
            //Se genera un color visualmente distinto al jugador
            Color enemyColor;
            Color playerColor = player.brush.Color;

            int colorDiff;
            do
            {
                //Genera colores brillantes para mejor visibilidad
                int r = rand.Next(100, 256); //Evita colores oscuros
                int g = rand.Next(100, 256);
                int b = rand.Next(100, 256);
                enemyColor = Color.FromArgb(r, g, b);
                
                //Revisa si los colores se asemejan mucho a los del jugador
                colorDiff = Math.Abs(enemyColor.R - playerColor.R) + 
                           Math.Abs(enemyColor.G - playerColor.G) + 
                           Math.Abs(enemyColor.B - playerColor.B);
                           
            } while (colorDiff < 150); //Se asegura que los colores sean distintos
            
            Enemy newEnemy = new Enemy(enemyX, enemyY, enemySize, enemySize, enemyColor);
            enemies.Add(newEnemy);
            Instantiate(newEnemy);
            
            Console.WriteLine($"Un nuevo enemigo aparece: Tamaño = {enemySize}, Posicion = ({enemyX},{enemyY})");
        }
    }
}
