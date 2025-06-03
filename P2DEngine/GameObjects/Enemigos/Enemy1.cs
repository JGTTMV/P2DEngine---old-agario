using P2DEngine.GameObjects;
using P2DEngine.Games;
using P2DEngine.Managers;
using System.Drawing;

public class Enemy1 : EnemyBase
{
    public Enemy1(float x, float y, float sizeX, float sizeY, Color color,
                 myPlayer target, CopperGear game, float speed, bool movesHorizontally)
        : base(x, y, sizeX, sizeY, color, target, game, speed, movesHorizontally, false)
    {
        detectionRadius = 200f;
        movementRange = 200f;
    }

    protected override void LoadSprites()
    {
        sprites[0] = myImageManager.Get("enemy1-front");
        sprites[1] = myImageManager.Get("enemy1-back");
        sprites[2] = myImageManager.Get("enemy1-left");
        sprites[3] = myImageManager.Get("enemy1-right");
    }
}
