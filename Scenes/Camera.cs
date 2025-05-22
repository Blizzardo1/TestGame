
using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using TestGame.GameObjects;
using TestGame.GameObjects.Characters;

namespace TestGame.Scenes; 
public class Camera(GameContext context) : Renderer {
    private static Log? _log = Log.GetCurrentClassLogger(LogCategory.Custom, "Game");

    public float Width { get; private set; } = context.Width;
    public float Height { get; private set; } = context.Height;

    private FRect _cameraRect;
    public FRect CameraRect => _cameraRect;

    public Entity? Target { get; set; } = null;

    public void Initialize() {
        RendererPtr = context.RendererPtr;
        _cameraRect = new () {
            X = 0,
            Y = 0,
            W = Width,
            H = Height
        };
    }

    public void SetScale(float scale) {
        if (scale <= 0) {
            _log?.Error("Scale must be greater than 0");
            return;
        }
        // Zoom in/out by dividing the width and height by the scale
        _ = Sdl.SetRenderScale(RendererPtr, scale, scale);
    }

    /// <summary>
    /// Set the camera to the specified position.
    /// </summary>
    /// <remarks>Requires <see cref="Target"/> to be set</remarks>
    /// <param name="x">Offset X</param>
    /// <param name="y">Offset Y</param>
    public void MoveCamera(int x, int y) {
        if(Target is null) {
            _log?.Error("Target is not set");
            return;
        }

        _cameraRect.X += Target.X - x;
        _cameraRect.Y += Target.Y - y;
    }
}


// #TODO: Rebase, Set global variables for Width and Height in Engine.cs