using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using TestGame.Colors;
using TestGame.GameObjects.Textures;

namespace TestGame.GameObjects
{
    public class AnimatedSprite : Sprite
    {
        private int _frame;
        
        private Rect[] _sourceRects;
        private Rect _currentFrame;

        /// <inheritdoc />
        public AnimatedSprite(IntPtr rendererPtr, int width, int height, string imagePath, Rect[] frames)
            : base(rendererPtr, width, height, imagePath) {
            _frame = 0;
            _sourceRects = frames;
            _currentFrame = _sourceRects[_frame];
            AddDelay(6);
        }

        private void AddDelay(int delay) {
            Rect[] clone = new Rect[_sourceRects.Length];
            // Console.WriteLine($"Original Rect Length: {clone.Length}");
            Array.Copy(_sourceRects, clone, _sourceRects.Length);
            Array.Resize(ref _sourceRects, delay * _sourceRects.Length);
            // Console.WriteLine($"Frame Array Length: {_sourceRects.Length}");
            for(int y = 0; y < clone.Length; y++) {
                for (int x = 0; x < delay; x++) {
                    // Console.WriteLine($"{x} + {y} * {delay} = {x + y * delay}");
                    _sourceRects[ x + ( y * delay ) ] = clone[ y ];
                }
            }
        }

        #region Overrides of Texture

        /// <inheritdoc />
        public override void Draw() {
            var rect = Rect.ToRect();
            _ = SDL.RenderCopy(RendererPtr, TexturePtr, ref _currentFrame, ref rect);
            RenderText($"Current Frame: X: {_currentFrame.X}, Y: {_currentFrame.Y}, W: {_currentFrame.W}, H: {_currentFrame.H}", 5, 200, KnownColor.White.ToColor());
        }

        /// <inheritdoc />
        public override void Update(Event e) {
            base.Update(e);
            _frame = ++_frame % _sourceRects.Length;
            _currentFrame = _sourceRects[_frame];
            
        }

        #endregion
    }
}
