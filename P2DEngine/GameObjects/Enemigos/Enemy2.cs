using P2DEngine.Games;
using P2DEngine.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace P2DEngine.GameObjects
{
    public class Enemy2 : EnemyBase
    {

        public Enemy2(float x, float y, float sizeX, float sizeY, Color color,
                 myPlayer target, CopperGear game, float speed, bool movesHorizontally)
        : base(x, y, sizeX, sizeY, color, target, game, 150f, movesHorizontally, false)
        {
            detectionRadius = 300f;
            movementRange = 200f; //Mayor rango que el enemigo1
        }

        protected override void LoadSprites()
        {
            sprites[0] = myImageManager.Get("enemy2-front");
            sprites[1] = myImageManager.Get("enemy2-back");
            sprites[2] = myImageManager.Get("enemy2-left");
            sprites[3] = myImageManager.Get("enemy2-right");
        }

    }
}
