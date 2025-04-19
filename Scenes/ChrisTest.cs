using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGame.GameObjects;

namespace TestGame.Scenes {
    // Chris Nicol (Kippy)
    internal class ChrisTest(GameContext context, string name) : Scene(context, name) {
        float x = 0;
        float y = 0;
        float radius = 100;

        public override void Initialize() {
            x = ( Width / 2 ) - ( radius / 2 );
            y = ( Height / 2 ) - ( radius / 2 );
        }

        public override void Cleanup() { }

        /// <inheritdoc />
        public override void Draw() {
            base.Draw();
            _ = SDL.SetRenderDrawColor(RendererPtr, 0, 0, 0, 255);
            _ = SDL.RenderClear(RendererPtr);
            _ = SDL.SetRenderDrawColor(RendererPtr, 255, 255, 255, 255);

            float offsetX = radius;
            float offsetY = 0;
            float d = radius - 1;
            while (offsetX >= offsetY) {
                _ = SDL.RenderDrawPointF(RendererPtr, x + offsetX, y + offsetY);
                _ = SDL.RenderDrawPointF(RendererPtr, x + offsetY, y + offsetX);
                _ = SDL.RenderDrawPointF(RendererPtr, x - offsetY, y + offsetX);
                _ = SDL.RenderDrawPointF(RendererPtr, x - offsetX, y + offsetY);
                _ = SDL.RenderDrawPointF(RendererPtr, x - offsetX, y - offsetY);
                _ = SDL.RenderDrawPointF(RendererPtr, x - offsetY, y - offsetX);
                _ = SDL.RenderDrawPointF(RendererPtr, x + offsetY, y - offsetX);
                _ = SDL.RenderDrawPointF(RendererPtr, x + offsetX, y - offsetY);

                if (d >= 2 * offsetY) {
                    d -= 2 * offsetY + 1;
                    offsetY += 1;
                }
                else if (d < 2 * ( radius - offsetX )) {
                    d += 2 * offsetX - 1;
                    offsetX -= 1;
                }
                else {
                    d += 2 * ( offsetX - offsetY - 1 );
                    offsetX -= 1;
                    offsetY += 1;
                }
            }
        }
    }
}