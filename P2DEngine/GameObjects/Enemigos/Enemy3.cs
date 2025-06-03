using P2DEngine.GameObjects;
using P2DEngine.Games;
using P2DEngine.Managers;
using System.Drawing;
using System.Numerics;
using System.Windows;

namespace P2DEngine.GameObjects.Enemigos
{
    public class Enemy3 : EnemyBase
    {
        private new CopperGear game;
        private int maxHealth = 10; //Mas vida que los enemigos normales
        private int currentHealth;

        public Enemy3(float x, float y, float sizeX, float sizeY, Color color,
                     myPlayer target, CopperGear game, float speed, bool movesHorizontally,
                     bool isFinalBoss = false)
            : base(x, y, sizeX, sizeY, color, target, game, speed, movesHorizontally, isFinalBoss)
        {
            this.game = game;
            currentHealth = maxHealth;
            detectionRadius = 700f;
            movementRange = 250f;
        }

        protected override void LoadSprites()
        {

            sprites[0] = myImageManager.Get("enemy3-front");
            sprites[1] = myImageManager.Get("enemy3-back");
            sprites[2] = myImageManager.Get("enemy3-left");
            sprites[3] = myImageManager.Get("enemy3-right");

        }
    }

    public class BossBullet : myBullet
    {
        public BossBullet(float x, float y, float radius, Color color, Vector2 direction, float speed)
            : base(x, y, radius, color, direction, speed)
        {
            // Configuraciones especiales para las balas del jefe
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            // Dibujar bala especial del jefe
            g.FillEllipse(Brushes.Red, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            g.DrawEllipse(Pens.DarkRed, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
        }
    }
}
