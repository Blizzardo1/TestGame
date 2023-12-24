using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SDL2;
using TestGame.GameObjects;
using TestGame.Scenes;

namespace TestGame
{
    internal class Diagnostics : Renderer, IGameObject
    {
        #region Implementation of IGameObject

        /// <inheritdoc />
        public float X => 10f;

        /// <inheritdoc />
        public float Y => 10f;

        /// <inheritdoc />
        public int Width { get; }

        /// <inheritdoc />
        public int Height { get; }

        /// <inheritdoc />
        public string Name => "Diagnostics";

        private readonly List<string> _diagnostics = new();
        private readonly Process _process = Process.GetCurrentProcess();
        
        public Diagnostics(nint rendererPtr, int width, int height)
        {
            Initialize(rendererPtr);
            Width = width;
            Height = height;
        }

        public void UpdateDiagnostics(Scene currentScene)
        {
            // TODO: Add more diagnostics, and be better at reserving space for them.
            // Using List<T> it reallocates the larger it gets. Maybe use a fixed array?
            _diagnostics.Clear();
            _diagnostics.Add($"Current Scene: {currentScene.Name}");
            _diagnostics.Add($"RAM: {SDL.GetSystemRAM()} MB");
            _diagnostics.Add($"Allocated: {_process.PrivateMemorySize64/1024/1024} MB");
            _diagnostics.Add($"CPU Count: {SDL.GetCPUCount()}");
            _diagnostics.Add($"Game Objects: {currentScene.GameObjects.Count}"); // Eventually, we want to change this.
        }

        /// <inheritdoc />
        public void Draw() {
            string[] a = _diagnostics.ToArray();
            for (int y = 0; y < a.Length; y++)
            {
                if (a[y].IsEmpty()) continue;
                RenderText(a[y], (int)X, ((int)Y + 8) * y, new Color { A = 255, B = 255, G = 255, R = 255 });
            }
        }

        /// <inheritdoc />
        public void Update(Event e) {
        
        }

        #endregion
    }
}
