using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P2DEngine
{
    public class NewGame : myGame
    {

        public NewGame(int width, int height, int FPS, myCamera c) : base(width, height, FPS, c)
        {
        }

        protected override void ProcessInput()
        {
        }

        protected override void RenderGame(Graphics g)
        {
        }

        protected override void Update()
        {
        }
    }
}
