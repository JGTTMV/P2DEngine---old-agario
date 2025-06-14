using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace P2DEngine
{
    public enum CameraMode
    {
        FollowPlayer1,
        FollowPlayer2,
        Free
    }
    public class myCamera
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public float zoom;
        public CameraMode currentMode { get; private set; }

        private Player targetPlayer;

        public myCamera(int x, int y, float width, float height, float zoom)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.zoom = zoom;

            currentMode = CameraMode.FollowPlayer1; //Modo default
        }

        public void SetTargetPlayer(Player player)
        {
            targetPlayer = player;
        }

        public void Update()
        {
            switch (currentMode)
            {
                case CameraMode.FollowPlayer1:
                    if (targetPlayer != null)
                    {
                        x = targetPlayer.x - width / 2;
                        y = targetPlayer.y - height / 2;
                    }
                    break;
                case CameraMode.FollowPlayer2:
                    if (targetPlayer != null)
                    {
                        x = targetPlayer.x - width / 2;
                        y = targetPlayer.y - height / 2;
                    }
                    break;
                case CameraMode.Free:
                    //No se cambia la logica de camara libre
                    break;
            }
        }

        public void HandleInput(Keys key, bool isKeyDown)
        {
            if (currentMode == CameraMode.Free)
            {
                if (isKeyDown)
                {
                    switch (key)
                    {
                        case Keys.W: y -= 10; break;
                        case Keys.S: y += 10; break;
                        case Keys.A: x -= 10; break;
                        case Keys.D: x += 10; break;
                        case Keys.Add: zoom += 0.1f; break;
                        case Keys.Subtract: zoom -= 0.1f; break;
                        
                    }
                }
            }
        }

        public void HandleZoom(int delta)
        {
            if (currentMode == CameraMode.Free)
            {
                zoom += delta > 0 ? 0.1f : -0.1f;
            }
        }

        public void SwitchMode()
        {
            switch (currentMode)
            {
                case CameraMode.FollowPlayer1:
                    currentMode = CameraMode.FollowPlayer2;
                    break;
                case CameraMode.FollowPlayer2:
                    currentMode = CameraMode.Free;
                    break;
                case CameraMode.Free:
                    currentMode = CameraMode.FollowPlayer1;
                    break;
            }
        }
        public Vector GetViewPosition(float worldX, float worldY)
        {
            return new Vector(
                (worldX - x) * zoom,
                (worldY - y) * zoom
            );
        }

        public Vector GetViewSize(float worldWidth, float worldHeight)
        {
            return new Vector(
                worldWidth * zoom,
                worldHeight * zoom
            );
        }
    }

}
