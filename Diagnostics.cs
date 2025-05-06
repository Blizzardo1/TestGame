using System.Diagnostics;
// using System.DirectoryServices;
using System.Numerics;
using SDL2;
using TestGame.Colors;
using TestGame.GameObjects;
using TestGame.Scenes;

namespace TestGame; 
internal class Diagnostics : Renderer, IRenderer {
    #region Implementation of IRenderable

    /// <inheritdoc />
    public float X => 0f;

    /// <inheritdoc />
    public float Y => 0f;

    /// <inheritdoc />
    public float Z { get; }

    /// <inheritdoc />
    public int Width { get; }

    /// <inheritdoc />
    public int Height { get; }

    /// <summary>
    /// Mouse Position
    /// </summary>
    public MouseData MouseData { get; set; }

    /// <inheritdoc />
    public string Name => "Diagnostics";

    private FRect _rect;
    private readonly List< string > _diagnostics = [];
    private readonly Process _process = Process.GetCurrentProcess();

    private double _lastTime;

    private bool _shown = false;
    public bool Shown { get => _shown; set => _shown = value; }

    public Diagnostics(nint rendererPtr, int width, int height) {
        Initialize(rendererPtr);
        Width = width;
        Height = height;
        _rect = new FRect { X = X, Y = Y, W = Width, H = Height };
        MouseData = new MouseData(Vector2.Zero, 0, 0);
    }

    public void UpdateDiagnostics(Scene currentScene) {
        _diagnostics.Clear();
        if (!_shown) {
            return;
        }

        // #TODO: Add more diagnostics, and be better at reserving space for them.
        // Using List<T> it reallocates the larger it gets. Maybe use a fixed array?
        int rendererInfoRes = SDL.GetRendererInfo(RendererPtr, out RendererInfo info);
        
        _diagnostics.Add($"FPS: {Engine.CurrentFPS}");
        _diagnostics.Add($"Current Scene: {currentScene.Name}");
        _diagnostics.Add($"Paused? {(Core.IsPaused ? "Yes" : "No")}");
        switch(rendererInfoRes) {
            case 0:
                _diagnostics.Add($"Rendering: {(((RendererFlags)info.Flags).HasFlag(RendererFlags.Accelerated) ? "Hardware" : "Software")}");
                break;
            default:
                _diagnostics.Add($"Renderer Error: {SDL.GetError()}");
                break;
        }
        _diagnostics.Add($"RAM: {SDL.GetSystemRAM()} MB");
        _diagnostics.Add($"Allocated: {_process.PrivateMemorySize64 / 1024 / 1024} MB");
        _diagnostics.Add($"Garbage Collector: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
        _diagnostics.Add($"CPU: {_process.TotalProcessorTime.TotalMilliseconds - _lastTime} ms");
        _diagnostics.Add($"CPU Count: {SDL.GetCPUCount()}");
        _diagnostics.Add($"Mouse: {MouseData.Position.X}, {MouseData.Position.Y}");
        _diagnostics.Add($"Mouse Button: {MouseData.Button}");
        _diagnostics.Add($"Mouse Wheel Direction: {MouseData.WheelDirection}");
        _diagnostics.Add($"Game Objects: {currentScene.GameObjects.Count}"); // Eventually, we want to change this.
    }

    /// <inheritdoc />
    public void Draw() {
        string[] a = [.. _diagnostics];
        _ = SDL.SetRenderDrawColor(RendererPtr, 0, 0, 0, 128);
        _ = SDL.RenderFillRectF(RendererPtr, ref _rect);
        for (int y = 0; y < a.Length; y++) {
            if (a[ y ].IsEmpty()) continue;

            RenderText(a[ y ], (int)X + 7, ( (int)Y + 20 ) * y + 3, KnownColor.Black.ToColor());
            RenderText(a[ y ], (int)X + 4, ( (int)Y + 20 ) * y + 3, KnownColor.White.ToColor());
        }
    }

    /// <inheritdoc />
    public void Update(Event e) {
        _lastTime = _process.TotalProcessorTime.TotalMilliseconds;
    }

    #endregion
}