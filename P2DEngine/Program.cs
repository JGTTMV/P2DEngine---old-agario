using P2DEngine.Games;
using P2DEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //Sprites legales del jugador
            myImageManager.Load("Mariano_L_Idle.png", "Mariano_L_Idle");
            myImageManager.Load("Mariano_R_Idle.png", "Mariano_R_Idle");
            myImageManager.Load("Mariano_L_Jump.png", "Mariano_L_Jump");
            myImageManager.Load("Mariano_R_Jump.png", "Mariano_R_Jump");
            myImageManager.Load("Mariano_L_Walk_1.png", "Mariano_L_Walk_1");
            myImageManager.Load("Mariano_L_Walk_2.png", "Mariano_L_Walk_2");
            myImageManager.Load("Mariano_L_Walk_3.png", "Mariano_L_Walk_3");
            myImageManager.Load("Mariano_R_Walk_1.png", "Mariano_R_Walk_1");
            myImageManager.Load("Mariano_R_Walk_2.png", "Mariano_R_Walk_2");
            myImageManager.Load("Mariano_R_Walk_3.png", "Mariano_R_Walk_3");

            //Sprites del enemigo
            myImageManager.Load("Turtle_L_1.png", "Turtle_L_1");
            myImageManager.Load("Turtle_L_2.png", "Turtle_L_2");
            myImageManager.Load("Turtle_L_3.png", "Turtle_L_3");
            myImageManager.Load("Turtle_R_1.png", "Turtle_R_1");
            myImageManager.Load("Turtle_R_2.png", "Turtle_R_2");
            myImageManager.Load("Turtle_R_3.png", "Turtle_R_3");

            //Sprites del entorno
            myImageManager.Load("Pipe_R.png", "Pipe_R");
            myImageManager.Load("Pipe_L.png", "Pipe_L");
            myImageManager.Load("Block.png", "Block");

            //Sonidos del juego
            myAudioManager.Load("No More Heroes theme 8-bit.mp3", "Game Music");
            myAudioManager.Load("smb_jump-small.wav", "Jump");
            myAudioManager.Load("smb_mario-die.wav", "Die");
            myAudioManager.Load("smb_bump.wav", "Bump");
            myAudioManager.Load("smb_gameover.wav", "Game Over");
            myAudioManager.Load("smb_pipe.wav", "Pipe");
            myAudioManager.Load("smb_stomp.wav", "Stomp");


            //Font del juego
            myFontManager.Load("CourierPrime-Regular.ttf", "courierFont");


            Mariano game = new Mariano(windowWidth, windowHeight, FPS, new myCamera(0, 0, camWidth, camHeight, 
                (float)windowWidth/(float)camWidth));

            game.Start();
            
            // Esto es propio de WinForms, es básicamente para que la ventana fluya.
            Application.Run();
        }
    }
}
