using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2DEngine
{
    //Juan José Valdebenito Pérez
    public class Indy500 : myGame
    {
        Player player;
        Player player2;

        Random rand = new Random();

        bool previousPressedSpace = false;

        float playerSpeed = 0.6f;
        float player2Speed = 0.6f;

        private float debugTimer = 0.0f; // Temporizador para rastrear el tiempo transcurrido para mensajes de depuracion
        private const float debugInterval = 0.5f; // Intervalo en segundos para mensajes de depuracion

        //Agrega un interruptor para el modo de depuracion
        private bool debugMode = false;

        //Agrega variables para rastrear la distancia recorrida y la puntuacion
        private float playerDistance = 0.0f;
        private float player2Distance = 0.0f;
        private int playerScore = 0; //Marca la posicion para la puntuacion del Jugador 1
        private int player2Score = 0; //Marca la posicion para la puntuacion del Jugador 2
        int randomX;
        int randomY;
        //La medidas de la pista se definen de manera global
        int pistaX = 100;
        int pistaY = 100;
        int pistaAncho = 5000;
        int pistaAlto = 900;
        int grosorBorde = 20;
        int margenInterno = 220;
        int grosorInterno = 25;

        //Agrega un nuevo objeto FinishLine
        private myBlock finishLine;

        //Agrega variables para la gestion de objetivos
        private myObjective currentObjective;
        private float objectiveTimer = 0.0f; //Temporizador para rastrear intervalos de 10 segundos
        private const float objectiveInterval = 10.0f; //Intervalo en segundos para la reubicacion del objetivo
        private const int objectiveSize = 30; //Tamaño del objetivo
        private const float speedBoostDuration = 3.0f; //Duracion del aumento de velocidad en segundos
        private const float speedBoostMultiplier = 1.5f; //Multiplicador de velocidad durante el aumento
        private const int trackInnerMargin = 220; //Margen interior de la pista
        private const int trackWidth = 5000; //Ancho de la pista
        private const int trackHeight = 900; //Altura de la pista
        private float playerBoostTimer = 0.0f; //Temporizador para el aumento de velocidad del Jugador 1
        private float player2BoostTimer = 0.0f; //Temporizador para el aumento de velocidad del Jugador 2

        //Agrega variables para las condiciones de fin del juego
        private const int maxScore = 25; //Puntuacion maxima para terminar el juego
        private const float maxTime = 120.0f; //Tiempo maximo en segundos (2 minutos)
        private float elapsedTime = 0.0f; //Temporizador para rastrear el tiempo transcurrido
        private bool isGameOver = false; //Indicador para verificar si el juego ha terminado

        public Indy500(int width, int height, int FPS, myCamera c) : base(width, height, FPS, c)
        {
            

            //Configura la cámara para que inicialmente siga al jugador 1
            currentCamera = new myCamera(0, 0, 800, 600, 1.0f);
            currentCamera.SetTargetPlayer(player); //La cámara sigue al jugador 1 por defecto

            //Crea bordes y pista (sin cambios)
            Color colorBorde = Color.Gray;

            //Bordes exteriores
            Instantiate(new myBlock(pistaX, pistaY, pistaAncho, grosorBorde, colorBorde));
            Instantiate(new myBlock(pistaX, pistaY + pistaAlto - grosorBorde, pistaAncho, grosorBorde, colorBorde));
            Instantiate(new myBlock(pistaX, pistaY, grosorBorde, pistaAlto, colorBorde));
            Instantiate(new myBlock(pistaX + pistaAncho - grosorBorde, pistaY, grosorBorde, pistaAlto, colorBorde));

            //Bordes interiores (formando el carril)
            Color colorInterno = Color.DarkGray;

            Instantiate(new myBlock(pistaX + margenInterno, pistaY + margenInterno, pistaAncho - 2 * margenInterno, grosorInterno, colorInterno));
            Instantiate(new myBlock(pistaX + margenInterno, pistaY + pistaAlto - margenInterno - grosorInterno, pistaAncho - 2 * margenInterno, grosorInterno, colorInterno));
            Instantiate(new myBlock(pistaX + margenInterno, pistaY + margenInterno, grosorInterno, pistaAlto - 2 * margenInterno, colorInterno));
            Instantiate(new myBlock(pistaX + pistaAncho - margenInterno - grosorInterno, pistaY + margenInterno, grosorInterno, pistaAlto - 2 * margenInterno, colorInterno));

            //Crea la linea de meta en el centro de la seccion superior de la pista
            finishLine = new myBlock(pistaX + pistaAncho / 2 - 10, pistaY + grosorBorde, 20, 200, Color.White);
            Instantiate(finishLine);

            //Agrega obstaculos a la pista
            int obstacleWidth = 150;
            int obstacleHeight = 150;
            Color obstacleColor = Color.Brown;

            //Ejemplo de obstaculos
            Instantiate(new myObstacle(200, 300, obstacleWidth, obstacleHeight, obstacleColor, true));
            Instantiate(new myObstacle(4850, 500, obstacleWidth, obstacleHeight, obstacleColor, true));
            Instantiate(new myObstacle(3500, 800, obstacleWidth, obstacleHeight, obstacleColor, true));
            Instantiate(new myObstacle(1500, 900, obstacleWidth, obstacleHeight, obstacleColor, true));
            Instantiate(new myObstacle(1500, 250, obstacleWidth, obstacleHeight, obstacleColor, true));

            //Ajusta las posiciones de aparicion de los jugadores detras de la linea de salida
            player = new Player(pistaX + pistaAncho / 2 - 50, pistaY + 80, 15, 15, Color.Red); // Jugador 1
            player2 = new Player(pistaX + pistaAncho / 2 - 50, pistaY + 140 , 15, 15, Color.Blue); // Jugador 2

            Instantiate(player2);
            Instantiate(player);

            // Instanciar Texto
            Instantiate(new myText(10, 10, () => $"Player 1: {playerScore} | Player 2: {player2Score}", "", "", new Font("Arial", 16)));
            Instantiate(new myText(650, 10, () => $"Time: {(int)elapsedTime} / 120", "", "", new Font("Arial", 16)));
        }

        private bool IsColliding(myGameObject a, myGameObject b)
        {
            //Rehace la deteccion de colisiones para garantizar calculos precisos de las cajas de impacto
            float aLeft = a.x;
            float aRight = a.x + a.sizeX;
            float aTop = a.y;
            float aBottom = a.y + a.sizeY;

            float bLeft = b.x;
            float bRight = b.x + b.sizeX;
            float bTop = b.y;
            float bBottom = b.y + b.sizeY;

            //Verifica superposicion entre los dos rectangulos
            return (aRight > bLeft && aLeft < bRight && aBottom > bTop && aTop < bBottom);
        }

        private bool WillCollide(Player player, int deltaX, int deltaY)
        {
            //Predice la nueva posicion del jugador
            float newX = player.x + deltaX;
            float newY = player.y + deltaY;

            //Crea un rectangulo temporal para la nueva posicion del jugador
            Rectangle playerRect = new Rectangle((int)newX, (int)newY, (int)player.sizeX, (int)player.sizeY);

            //Verifica colisiones con todos los objetos myBlock
            foreach (var obj in gameObjects)
            {
                if (obj is myBlock block && !(block is Player) && !(block is myObstacle) && !(block is myObjective) && block.x != finishLine.x && block.y != finishLine.y)
                {
                    Rectangle blockRect = new Rectangle((int)block.x, (int)block.y, (int)block.sizeX, (int)block.sizeY);
                    if (playerRect.IntersectsWith(blockRect))
                    {
                        return true; //Colision detectada
                    }
                }
            }

            return false; //Sin colision
        }

        protected override void ProcessInput()
        {
            if (isGameOver)
                return; //Deshabilita los inputs al terminar el juego

            int baseStep = 50;

            //Movimiento de player
            if (currentCamera.currentMode != CameraMode.Free) //Usa el modo de camara global
            {
                //Revisa las colisiones antes de actualizar al jugador
                if (myInputManager.IsKeyPressed(Keys.W) && !WillCollide(player, 0, -(int)(baseStep * player.speed)))
                {
                    player.y -= (int)(baseStep * player.speed);
                    playerDistance += baseStep * player.speed;
                }
                if (myInputManager.IsKeyPressed(Keys.A) && !WillCollide(player, -(int)(baseStep * player.speed), 0))
                {
                    player.x -= (int)(baseStep * player.speed);
                    playerDistance += baseStep * player.speed;
                }
                if (myInputManager.IsKeyPressed(Keys.S) && !WillCollide(player, 0, (int)(baseStep * player.speed)))
                {
                    player.y += (int)(baseStep * player.speed);
                    playerDistance += baseStep * player.speed;
                }
                if (myInputManager.IsKeyPressed(Keys.D) && !WillCollide(player, (int)(baseStep * player.speed), 0))
                {
                    player.x += (int)(baseStep * player.speed);
                    playerDistance += baseStep * player.speed;
                }

                //Movimiento de player2
                if (myInputManager.IsKeyPressed(Keys.Up) && !WillCollide(player2, 0, -(int)(baseStep * player2.speed)))
                {
                    player2.y -= (int)(baseStep * player2.speed);
                    player2Distance += baseStep * player2.speed;
                }
                if (myInputManager.IsKeyPressed(Keys.Left) && !WillCollide(player2, -(int)(baseStep * player2.speed), 0))
                {
                    player2.x -= (int)(baseStep * player2.speed);
                    player2Distance += baseStep * player2.speed;
                }
                if (myInputManager.IsKeyPressed(Keys.Down) && !WillCollide(player2, 0, (int)(baseStep * player2.speed)))
                {
                    player2.y += (int)(baseStep * player2.speed);
                    player2Distance += baseStep * player2.speed;
                }
                if (myInputManager.IsKeyPressed(Keys.Right) && !WillCollide(player2, (int)(baseStep * player2.speed), 0))
                {
                    player2.x += (int)(baseStep * player2.speed);
                    player2Distance += baseStep * player2.speed;
                }
            }

            var pressedSpace = myInputManager.IsKeyPressed(Keys.Space);

            if (pressedSpace && !previousPressedSpace)
            {
                //Cambia el modo de la cámara utilizando el método SwitchMode
                currentCamera.SwitchMode();
            }

            //Restablece el juego al presionar la tecla 'R'
            if (myInputManager.IsKeyPressed(Keys.R))
            {
                ResetGame();
            }

            if (myInputManager.IsKeyPressed(Keys.P))
                debugMode = true;
            else 
                debugMode = false;

            //Hace zoom y mover la camara en modo libre
            if (currentCamera.currentMode == CameraMode.Free) //Usa CameraMode global
            {
                if (myInputManager.IsKeyPressed(Keys.W)) currentCamera.y -= baseStep;
                if (myInputManager.IsKeyPressed(Keys.A)) currentCamera.x -= baseStep;
                if (myInputManager.IsKeyPressed(Keys.S)) currentCamera.y += baseStep;
                if (myInputManager.IsKeyPressed(Keys.D)) currentCamera.x += baseStep;

                if (myInputManager.IsKeyPressed(Keys.Z))
                {
                    currentCamera.zoom += 0.01f;
                }
                if (myInputManager.IsKeyPressed(Keys.X))
                {
                    currentCamera.zoom -= 0.01f;
                }
            }

            previousPressedSpace = pressedSpace;
        }

        protected override void RenderGame(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, windowWidth, windowHeight);
            //Añade las variables de mainCamera a la logica de dibujado. Estamos
            //Dibuja con respecto a la camara.

            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw(g,
                    currentCamera.GetViewPosition(gameObject.x, gameObject.y),
                    currentCamera.GetViewSize(gameObject.sizeX, gameObject.sizeY));

                //Depuracion: Dibujar cajas de colision si el modo de depuracion esta habilitado
                if (debugMode)
                {
                    g.DrawRectangle(Pens.Red,
                        (float)currentCamera.GetViewPosition(gameObject.x, gameObject.y).X,
                        (float)currentCamera.GetViewPosition(gameObject.x, gameObject.y).Y,
                        (float)currentCamera.GetViewSize(gameObject.sizeX, gameObject.sizeY).X,
                        (float)currentCamera.GetViewSize(gameObject.sizeX, gameObject.sizeY).Y);
                }
            }
        }

        private void DrawBoundingBox(Graphics g, myGameObject obj)
        {
            //Dibuja un rectangulo alrededor de la caja de colision del objeto para depuracion
            var pen = new Pen(Color.Yellow, 1);
            g.DrawRectangle(pen, obj.x, obj.y, obj.sizeX, obj.sizeY);
        }

        protected override void Update()
        {
            if (isGameOver)
                return; //Termina de actualizar al terminar el juego

            elapsedTime += deltaTime; //Incrementa la variable elapsed time

            //Revisa si el juego debe terminar
            if (playerScore >= maxScore || player2Score >= maxScore || elapsedTime >= maxTime)
            {
                isGameOver = true;
                ShowGameOverPopup();
                return;
            }

            bool playerOnObstacle = false;
            bool player2OnObstacle = false;

            debugTimer += deltaTime; //incrementa el timer

            foreach (var obj in gameObjects)
            {
                if (obj is myObstacle obstacle && obstacle.isObstacle)
                {
                    if (IsColliding(player, obstacle))
                    {
                        playerOnObstacle = true;
                    }
                    if (IsColliding(player2, obstacle))
                    {
                        player2OnObstacle = true;
                    }
                }
            }

            //Ajusta las velocidades de los jugadores segun si estan en obstaculos
            playerSpeed = playerOnObstacle ? 0.3f : .6f;
            player2Speed = player2OnObstacle ? 0.3f : .6f;

            player.SetSpeed(playerSpeed);
            player2.SetSpeed(player2Speed);

            //Verifica si los jugadores completan una vuelta
            if (playerDistance >= 10000 && IsColliding(player, finishLine))
            {
                playerScore+= 5;
                playerDistance = 0; //Restablece la distancia despues de completar una vuelta
            }

            if (player2Distance >= 10000 && IsColliding(player2, finishLine))
            {
                player2Score+= 5;
                player2Distance = 0; //Restablece la distancia despues de completar una vuelta
            }

            //Actualiza el temporizador del objetivo
            objectiveTimer += deltaTime;

            //Verifica si es hora de generar o reubicar el objetivo
            if (objectiveTimer >= objectiveInterval)
            {
                objectiveTimer = 0.0f;

                // Si ya existe un objetivo, destruirlo
                if (currentObjective != null)
                {
                    Destroy(currentObjective);
                }

                //Define márgenes
                int left = pistaX + grosorBorde;
                int top = pistaY + grosorBorde;
                int right = pistaX + pistaAncho - grosorBorde - objectiveSize;
                int bottom = pistaY + pistaAlto - grosorBorde - objectiveSize;

                int innerLeft = pistaX + margenInterno;
                int innerTop = pistaY + margenInterno;
                int innerRight = pistaX + pistaAncho - margenInterno - objectiveSize;
                int innerBottom = pistaY + pistaAlto - margenInterno - objectiveSize;

                bool validPosition = false;

                while (!validPosition)
                {
                    randomX = rand.Next(left, right);
                    randomY = rand.Next(top, bottom);

                    //Verifica si está fuera del rectángulo interno
                    if (!(randomX > innerLeft && randomX < innerRight && randomY > innerTop && randomY < innerBottom))
                    {
                        validPosition = true;
                    }
                }

                currentObjective = new myObjective(randomX, randomY, objectiveSize, objectiveSize, Color.Yellow, true);
                Instantiate(currentObjective);
            }

            //Revisa las colisiones con el objetivo
            if (currentObjective != null)
            {
                if (IsColliding(player, currentObjective))
                {
                    playerScore++;
                    playerBoostTimer = speedBoostDuration; //Inicia el boost para el jugador 1
                    Destroy(currentObjective);
                    currentObjective = null;
                }
                else if (IsColliding(player2, currentObjective))
                {
                    player2Score++;
                    player2BoostTimer = speedBoostDuration; //Inicia el boost para el jugador 2
                    Destroy(currentObjective);
                    currentObjective = null;
                }
            }

            //Actualiza los boosts de velocidad
            if (playerBoostTimer > 0)
            {
                playerBoostTimer -= deltaTime;
                player.SetSpeed(playerSpeed * speedBoostMultiplier);
            }
            else
            {
                player.SetSpeed(playerSpeed);
            }

            if (player2BoostTimer > 0)
            {
                player2BoostTimer -= deltaTime;
                player2.SetSpeed(player2Speed * speedBoostMultiplier);
            }
            else
            {
                player2.SetSpeed(player2Speed);
            }

            //Imprime la informacion del debug cada 0,5 segundos
            if (debugTimer >= debugInterval && debugMode)
            {
                Console.Clear(); //Limpia la consola

                if(currentCamera.currentMode == CameraMode.FollowPlayer1)
                {
                    Console.WriteLine($"Espectando al jugador 1 - Score: {playerScore}");
                }
                else if(currentCamera.currentMode == CameraMode.FollowPlayer2)
                {
                    Console.WriteLine($"Espectando al jugador 2 - Score: {player2Score}");
                }
                else if(currentCamera.currentMode == CameraMode.Free)
                {
                    Console.WriteLine($"Espectando la camara libre - Jugador 1 Score: {playerScore} | Jugador 2 Score: {player2Score}");
                }

                debugTimer = 0.0f; //Resetea el timer del debug
            }

            //Actualiza la camara dependiendo del modo
            switch (currentCamera.currentMode)
            {
                case CameraMode.FollowPlayer1:
                    currentCamera.SetTargetPlayer(player);
                    currentCamera.x = player.x - (windowWidth / (2 * currentCamera.zoom));
                    currentCamera.y = player.y - (windowHeight / (2 * currentCamera.zoom));
                    break;

                case CameraMode.FollowPlayer2:
                    currentCamera.SetTargetPlayer(player2);
                    currentCamera.x = player2.x - (windowWidth / (2 * currentCamera.zoom));
                    currentCamera.y = player2.y - (windowHeight / (2 * currentCamera.zoom));
                    break;

                case CameraMode.Free:
                    //No se cambia la logica del modo free
                    break;
            }
        }

        private void ShowGameOverPopup()
        {
            string winner = playerScore > player2Score ? "Player 1" : "Player 2";
            if (playerScore == player2Score)
                winner = "EMPATE!!!";

            MessageBox.Show($"Game Over!\nWinner: {winner}\nPlayer 1 Score: {playerScore}\nPlayer 2 Score: {player2Score}", "Game Over");
        }

        private void ResetGame()
        {
            //Restablece variables del juego
            playerScore = 0;
            player2Score = 0;
            playerDistance = 0.0f;
            player2Distance = 0.0f;
            elapsedTime = 0.0f;
            isGameOver = false;

            //Restablece posiciones de los jugadores
            player.x = trackWidth / 2 - 50;
            player.y = 100 + 80;
            player2.x = trackWidth / 2 - 50;
            player2.y = 100 + 140;

            //Restablece velocidad
            player.SetSpeed(0.6f);
            player2.SetSpeed(0.6f);

            //Destruye cualquier objetivo existente
            if (currentObjective != null)
            {
                Destroy(currentObjective);
                currentObjective = null;
            }

            //Restablece camara
            currentCamera.SetTargetPlayer(player);
        }
    }
}
