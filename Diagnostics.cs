using SharpSDL3;
using SharpSDL3.Structs;

using System.Diagnostics;

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
    public float Width { get; }

    /// <inheritdoc />
    public float Height { get; }

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

    public bool Shown { get; set; }

    public Diagnostics(nint rendererPtr, int width, int height) {
        Initialize(rendererPtr);
        Width = width;
        Height = height;
        _rect = new FRect { X = X, Y = Y, W = Width, H = Height };
        MouseData = new MouseData(new(), 0, 0);
    }

    public void UpdateDiagnostics(Scene currentScene) {
        _diagnostics.Clear();
        if (!Shown) {
            return;
        }

        // #TODO: Add more diagnostics, and be better at reserving space for them.
        // Using List<T> it reallocates the larger it gets. Maybe use a fixed array?
        
        
        _diagnostics.Add($"FPS: {Engine.CurrentFPS}");
        _diagnostics.Add($"Current Scene: {currentScene.Name}");
        _diagnostics.Add($"Paused? {(Core.IsPaused ? "Yes" : "No")}");
        _diagnostics.Add($"RAM: {CpuInfo.GetSystemRAM()} MB");
        _diagnostics.Add($"Allocated: {_process.PrivateMemorySize64 / 1024 / 1024} MB");
        _diagnostics.Add($"Garbage Collector: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
        _diagnostics.Add($"CPU: {_process.TotalProcessorTime.TotalMilliseconds - _lastTime} ms");
        _diagnostics.Add($"CPU Count: {CpuInfo.GetNumLogicalCPUCores()}");
        _diagnostics.Add($"Mouse: {MouseData.Position.X}, {MouseData.Position.Y}");
        _diagnostics.Add($"Mouse Button: {MouseData.Button}");
        _diagnostics.Add($"Mouse Wheel Direction: {MouseData.WheelDirection}");
        _diagnostics.Add($"Game Objects: {currentScene.GameObjects.Count}"); // Eventually, we want to change this.
    }

    /// <inheritdoc />
    public void Draw() {
        string[] a = [.. _diagnostics];
        _ = Sdl.SetRenderDrawColor(RendererPtr, 0, 0, 0, 128);
        _ = Sdl.RenderFillRect(RendererPtr, ref _rect);
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