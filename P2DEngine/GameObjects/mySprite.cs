using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P2DEngine.GameObjects
{
    public class mySprite
    {
        private List<Image> frames; // La animación se guarda en esta lista.
        private int frame_index; // Índice para el frame de animación.

        private float elapsedTime; // Timer.
        private float targetTime; // Cuanto dura cada frame.

        private int startFrame;
        private int frameCount;

        public mySprite(float targetTime)
        {
            this.frames = new List<Image>();
            this.frame_index = 0;
            this.targetTime = targetTime; // Esta línea faltaba
            this.startFrame = 0;
            this.frameCount = 0;
        }

        // Añadimos una imagen a la lista. Si ud. quiere, puede implementar una versión
        // donde se le pasa la lista de imágenes.
        public void AddFrame(Image image, int frameCount = 1)
        {
            for (int i = 0; i < frameCount; i++)
            {
                frames.Add(image);
            }
        }

        public void SetAnimationRange(int startFrame, int frameCount)
        {
            this.startFrame = startFrame;
            this.frameCount = frameCount;
            this.frame_index = startFrame;
        }

        // Obtenemos el frame actual de animación.
        public Image GetCurrentFrame()
        {
            return frames[frame_index];
        }

        public void Update(float deltaTime)
        {
            elapsedTime += deltaTime;
            if (elapsedTime >= targetTime)
            {
                elapsedTime = 0.0f;
                frame_index++;

                // Si frameCount es 0, usa toda la animación
                int endFrame = frameCount > 0 ? startFrame + frameCount : frames.Count;

                if (frame_index >= endFrame)
                {
                    frame_index = startFrame;
                }
            }
        }
    }
}
