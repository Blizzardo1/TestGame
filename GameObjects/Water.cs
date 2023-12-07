using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using TestGame.GameObjects.Textures;

namespace TestGame.GameObjects
{
    public class Water : AnimatedSprite {
        private const int PSize = 16;
        /// <inheritdoc />
        public Water(IntPtr rendererPtr)
            : base(rendererPtr, 64, 64, Path.Combine(Program.StartupPath, "Water.bmp"),
                new Rect[]
                {
                    new() {
                        W = PSize,
                        H = PSize,
                        X = 1,
                        Y = 1
                    },
                    new() {
                        W = PSize,
                        H = PSize,
                        X = 18,
                        Y = 1
                    },
                    new() {
                        W = PSize,
                        H = PSize,
                        X = 35,
                        Y = 1
                    },
                    new() {
                        W = PSize,
                        H = PSize,
                        X = 52,
                        Y = 1
                    }

                }
                ) {
        }
    }
}
