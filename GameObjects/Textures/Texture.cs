using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace TestGame.GameObjects.Textures {
    public abstract class Texture : Renderer, IGameObject {
        private FRect _rect;

        protected FRect Rect => _rect;

        public nint TexturePtr { get; protected set; }

        #region Implementation of IGameObject

        /// <inheritdoc />
        public float X
        {
            get => _rect.X;
            set => _rect.X = value;
        }

        /// <inheritdoc />
        public float Y
        {
            get => _rect.Y;
            set => _rect.Y = value;
        }

        /// <inheritdoc />
        public int Width
        {
            get => (int)_rect.W;
            set => _rect.W = value;
        }

        /// <inheritdoc />
        public int Height
        {
            get => (int)_rect.H;
            set => _rect.H = value;
        }

        public string Name => "Texture";

        ~Texture() {
            SDL.DestroyTexture(TexturePtr);
        }

        public Texture(nint rendererPtr, int width, int height) {
            RendererPtr = rendererPtr;
            Width = width;
            Height = height;
        }

        /// <inheritdoc />
        public virtual void Draw() {
            _ = SDL.RenderCopyF(RendererPtr, TexturePtr, nint.Zero, ref _rect);
        }

        /// <inheritdoc />
        public virtual void Update(Event e) { }

        #endregion
    }
}