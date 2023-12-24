using SDL2;
using TestGame.GameObjects;

namespace TestGame.Scenes
{
    internal class PauseScene : Scene
    {
        /// <inheritdoc />
        public PauseScene(IntPtr rendererPtr, int width, int height) : base(rendererPtr, width, height) { }

        #region Overrides of Scene

        /// <inheritdoc />
        public override string Name => "Pause";

        /// <inheritdoc />
        public override void Initialize() {
            AddGameObject(new Clock(RendererPtr));
        }

        /// <inheritdoc />
        public override void Draw() {
            base.Draw();
            _ = SDL.SetRenderDrawColor(RendererPtr, 0, 0, 0, 255);
            _ = SDL.RenderClear(RendererPtr);
            RenderText("Paused", Width / 2 - 50, Height / 2 - 50, new Color { A = 255, B = 255, G = 255, R = 255 });
        }

        #endregion
    }
}
