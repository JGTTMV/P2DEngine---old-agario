using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace P2DEngine
{
    // Clase que tiene la lógica de la ventana.
    public class myWindow : Form
    {
        // Para el doble búfer.
        BufferedGraphicsContext GraphicsManager;
        BufferedGraphics managedBackBuffer;

        private myCamera camera; //Añade una instancia de camara

        public myWindow(int width, int height)
        {
            ClientSize = new Size(width, height); //El cliente, vendría siendo la parte de la ventana dentro de los márgenes.
            MaximizeBox = false; //Permite que se maximice la ventana.
            FormBorderStyle = FormBorderStyle.FixedSingle; //Decide si se puede cambiar el tamaño de la ventana.

            // Véalo como un búfer fantasma, le cargamos en memoria todo lo que dibujamos
            // y luego lo mostramos en pantalla.
            GraphicsManager = BufferedGraphicsManager.Current;
            GraphicsManager.MaximumBuffer = new Size(width, height);
            managedBackBuffer = GraphicsManager.Allocate(CreateGraphics(), ClientRectangle);

            //Se inicia la camara
            camera = new myCamera(0, 0, width, height, 1.0f);

            // Necesitamos añadirlos para que la ventana "escuche" a las presiones del teclado. Sin esto el programa no sabría
            // como recibir los inputs.
            KeyDown += _KeyDown;
            KeyUp += _KeyUp;

            MouseDown += _MouseDown;
            MouseUp += _MouseUp;
            MouseMove += _MouseMove;
            MouseWheel += _MouseWheel; //Se añade control para la rueda del mouse
        }

        public Graphics GetGraphics()
        {
            managedBackBuffer.Graphics.Clear(Color.Black);
            return managedBackBuffer.Graphics;
        }

        // Dibujar.
        public void Render()
        {
            try
            {
                //Se actualiza la camara antes del renderizado
                camera.Update();

                //Se renderiza el BackBuffer
                managedBackBuffer.Render();
            }
            catch (Exception ex)
            {
                Environment.Exit(0);
            }
        }

        //Presiona un botón del mouse.
        public void _MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                camera.SwitchMode();
            }
            else
            {
                myInputManager.MouseDown(e.Button);
            }
        }

        //Levanta un botón del mouse.
        public void _MouseUp(object sender, MouseEventArgs e)
        {
            myInputManager.MouseUp(e.Button);
        }

        //Mueve el mouse.
        public void _MouseMove(object sender, MouseEventArgs e)
        {
            myInputManager.MouseMove(e.Location);
        }

        //Scroll del mouse.
        public void _MouseWheel(object sender, MouseEventArgs e)
        {
            camera.HandleZoom(e.Delta);
        }

        //Presiona una tecla.
        public void _KeyDown(object sender, KeyEventArgs e)
        {
            camera.HandleInput(e.KeyCode, true); // Pass input to the camera.
            myInputManager.KeyDown(e.KeyCode);
        }

        //Levanta una tecla.
        public void _KeyUp(object sender, KeyEventArgs e)
        {
            camera.HandleInput(e.KeyCode, false); // Pass input to the camera.
            myInputManager.KeyUp(e.KeyCode);
        }
    }
}
