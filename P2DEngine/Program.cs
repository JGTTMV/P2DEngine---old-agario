using P2DEngine.Games;
using P2DEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace P2DEngine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Ancho y alto de la ventana.
            int windowWidth = 800;
            int windowHeight = 600;

            // Ancho y alto de la cámara.
            int camWidth = 800;
            int camHeight = 600;

            // Frames por segundo.
            int FPS = 60;


            // Cargado de recursos. Recuerden que como base se les va a caer ya que no existen estos recursos de ejemplo
            // por eso está comentado. 

            //Se cargan los assets del juego
            myImageManager.Load("Background.jpg", "Background");
            myImageManager.Load("bullet.png", "bullet");
            myImageManager.Load("gas-front.png", "gas-front");
            myImageManager.Load("gas-back.png", "gas-back");
            myImageManager.Load("gas-right.png", "gas-right");
            myImageManager.Load("gas-left.png", "gas-left");
            myImageManager.Load("enemy1-front.png", "enemy1-front");
            myImageManager.Load("enemy1-back.png", "enemy1-back");
            myImageManager.Load("enemy1-left.png", "enemy1-left");
            myImageManager.Load("enemy1-right.png", "enemy1-right");
            myImageManager.Load("enemy2-front.png", "enemy2-front");
            myImageManager.Load("enemy2-back.png", "enemy2-back");
            myImageManager.Load("enemy2-left.png", "enemy2-left");
            myImageManager.Load("enemy2-right.png", "enemy2-right");
            myImageManager.Load("enemy3-front.png", "enemy3-front");
            myImageManager.Load("enemy3-back.png", "enemy3-back");
            myImageManager.Load("enemy3-left.png", "enemy3-left");
            myImageManager.Load("enemy3-right.png", "enemy3-right");

            // Load audio assets
            myAudioManager.Load("gameover.mp3", "gameover");
            myAudioManager.Load("gunshot.mp3", "gunshot");
            myAudioManager.Load("win.mp3", "win");
            myAudioManager.Load("Ikebukuro - SMT NINE.wav", "music");
            myAudioManager.Load("boss-music.mp3", "boss-music");
            myAudioManager.Load("Cruch.wav", "cruch");
            myAudioManager.Load("Metal 1.wav", "boss-defeated");
            myAudioManager.Load("Curtis - Point Score.wav", "health");

            myFontManager.Load("CourierPrime-Regular.ttf", "CourierPrime-Regular");

            CopperGear game = new CopperGear(windowWidth, windowHeight, FPS, new myCamera(0, 0, camWidth, camHeight, 
                (float)windowWidth/(float)camWidth));

            game.Start();
            
            // Esto es propio de WinForms, es básicamente para que la ventana fluya.
            Application.Run();
        }
    }
}
