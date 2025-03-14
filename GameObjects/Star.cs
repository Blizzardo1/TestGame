using NLog;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using TestGame.Colors;

namespace TestGame.GameObjects
{
    internal class Star : GameObject
    {
        private Logger _log;

        public int Id { get; }
        public float Distance { get; }
        public int NumOfStars { get; }

        private Color color;
        
        public Star(int id, float distance, int ns, float x, float y, float z, int h, int s, int l, float a)
        {
            _log = LogManager.GetCurrentClassLogger();
            Id = id;
            Distance = distance;
            NumOfStars = ns;
            X = x;
            Y = y;
            Z = z;
            
            ColorConverter.HSL2RGB(h, s, l, out byte r, out byte g, out byte b);
            color = new Color() { R = r, G = g, B = b, A = (byte)(a * 255) };
            float angle = id * (2 * (float)Math.PI / 1000);
            frect = new()
            {
                X = X + (distance * (float)Math.Cos(angle)),
                Y = Y + (distance * (float)Math.Sin(angle)),
                W = Z,
                H = Z
            };
        }

        public override void Draw()
        {
            Core.SetRenderColor(RendererPtr, color);
            float offsetX = Z;
            float offsetY = 0;
            float d = Z - 1;
            while (offsetX >= offsetY)
            {
                _ = SDL.RenderDrawPointF(RendererPtr, X + offsetX, Y + offsetY);
                _ = SDL.RenderDrawPointF(RendererPtr, X + offsetY, Y + offsetX);
                _ = SDL.RenderDrawPointF(RendererPtr, X - offsetY, Y + offsetX);
                _ = SDL.RenderDrawPointF(RendererPtr, X - offsetX, Y + offsetY);
                _ = SDL.RenderDrawPointF(RendererPtr, X - offsetX, Y - offsetY);
                _ = SDL.RenderDrawPointF(RendererPtr, X - offsetY, Y - offsetX);
                _ = SDL.RenderDrawPointF(RendererPtr, X + offsetY, Y - offsetX);
                _ = SDL.RenderDrawPointF(RendererPtr, X + offsetX, Y - offsetY);

                if (d >= 2 * offsetY) {
                    d -= 2 * offsetY++ + 1;
                }
                else if (d < 2 * (Z - offsetX)) {
                    d += 2 * offsetX-- - 1;
                }
                else {
                    d += 2 * (offsetX-- - offsetY++ - 1);
                }
            }
        }

        public override void Update(Event e)
        {
            if (frect.W < 0) frect.W = 1;
            if (frect.H < 0) frect.H = 1;
        }
    }
}
