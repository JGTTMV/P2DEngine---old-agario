using P2DEngine.Games;
using P2DEngine.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace P2DEngine.GameObjects
{
    public class myBullet : myPhysicsCircle
    {
        private float speedY;
        private float speedX;
        private Image sprite;
        private Vector2 direction;
        private float speed;

        public myBullet(float x, float y, float radius, Color color, Vector2 direction, float speed) : base(x, y, radius, color)
        {
            this.direction = direction;
            this.speed = speed;
        }

        public myBullet(float x, float y, float radius, Color color, Image sprite, float speedY, float speedX)
            : base(x, y, radius, color)
        {
            this.speedY = speedY;
            this.speedX = speedX;
            this.sprite = sprite;
            affectedByGravity = false;
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            //Dibujar el sprite en lugar de un circulo
            if (sprite != null)
            {
                g.DrawImage(sprite, (float)position.X, (float)position.Y, (float)size.X, (float)size.Y);
            }
            else
            {
                base.Draw(g, position, size); //Fallback si no hay sprite
            }
        }

        public override void UpdateGameObject(float deltaTime)
        {
            y += speedY * deltaTime;
            x += speedX * deltaTime;
            //La destruccion la manejara el juego principal
        }
    }

    public class myEnemyBullet : myBullet
    {
        public myEnemyBullet(float x, float y, float radius, Color color, Image sprite, float speedY, float speedX)
            : base(x, y, radius, color, sprite ?? myImageManager.Get("bullet"), speedY, speedX)
        {
        }
    }
}
