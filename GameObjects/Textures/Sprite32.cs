using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.GameObjects.Textures {
    /// <summary>
    /// A Transparent 32-Bit Texture
    /// </summary>
    public class Sprite32 : Texture {
        private readonly string _imagePath;

        public string ImagePath => _imagePath;

        private Rect _source;
        private Rect _dest;

        public Sprite32(nint parentTexture, nint rendererPtr, int x, int y, int width, int height)
            : base(rendererPtr, width, height) {
            _imagePath = "";
            _source = new() {
                X = x,
                Y = y,
                W = width,
                H = height
            };

            _dest = new() {
                X = 0,
                Y = 0,
                W = width,
                H = height
            };
        }

        /// <inheritdoc />
        public Sprite32(IntPtr rendererPtr, int width, int height, string imagePath)
            : base(rendererPtr, width, height) {
            _imagePath = imagePath;

            nint surface = Image.Load(_imagePath);
            if (surface == IntPtr.Zero) {
                throw new Exception($"Failed to load image at {_imagePath}");
            }

            TexturePtr = SDL.CreateTextureFromSurface(rendererPtr, surface);
            SDL.FreeSurface(surface);
        }

        public void RenderTile() {
            _ = SDL.RenderCopy(RendererPtr, TexturePtr, ref _source, ref _dest);
        }
    }
}