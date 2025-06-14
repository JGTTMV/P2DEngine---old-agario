using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

namespace P2DEngine
{
    public class myText : myGameObject
    {
        private Func<string> variableProvider;
        private string textBefore;
        private string textAfter;
        private Font font;

        public myText(float x, float y, Func<string> variableProvider, string textBefore = "", string textAfter = "", Font font = null)
            : base(x, y, 1, 1, Color.White)
        {
            this.variableProvider = variableProvider ?? throw new ArgumentNullException(nameof(variableProvider));
            this.textBefore = textBefore;
            this.textAfter = textAfter;
            this.font = font ?? new Font("Arial", 12);

            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                string fullText = $"{textBefore}{variableProvider()}{textAfter}";
                SizeF textSize = g.MeasureString(fullText, this.font);
                this.sizeX = textSize.Width;
                this.sizeY = textSize.Height;
            }
        }

        public override void Update(float deltaTime)
        {
        }

        public override void Draw(Graphics g, Vector position, Vector size)
        {
            string fullText = $"{textBefore}{variableProvider()}{textAfter}";
            SizeF textSize = g.MeasureString(fullText, font);

            sizeX = textSize.Width;
            sizeY = textSize.Height;

            g.DrawString(fullText, font, brush, x, y);
        }
    }
}
