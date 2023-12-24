using SDL2;
using TestGame.Colors;
using TestGame.GameObjects;

namespace TestGame.Scenes
{
    internal class SampleWorld : Scene {
        #region Overrides of Scene

        /// <inheritdoc />
        public override string Name => "World";

        #endregion

        public SampleWorld(nint rendererPtr, int width, int height) : base(rendererPtr, width, height) {
        }

        private void SetColorBasedOnTime()
        {
            // Daytime/Nighttime Cycles
            SetColor(DateTime.Now.Hour switch
            {
                < 6 or >= 18 => KnownColor.Black.ToColor(),
                >= 6 and < 8 or >= 16 and < 18 => KnownColor.DeepSkyBlue.ToColor(),
                _ => KnownColor.SkyBlue.ToColor()
            });
        }

        #region Overrides of Scene

        /// <inheritdoc />
        public override void Initialize() {
            for (int y = 0; y < Height; y += 64)
            {
                for (int x = 0; x < Width; x += 64) {
                    if (y < 128 && x < 256) continue;
                    AddGameObject(new Water(RendererPtr) { X = x, Y = y, Width = 64, Height = 64});
                }
            }

            AddGameObject(new Clock(RendererPtr));
        }

        /// <inheritdoc />
        public override void Update(Event e) {
            base.Update(e);
            SetColorBasedOnTime();
        }

        #endregion
    }
}
