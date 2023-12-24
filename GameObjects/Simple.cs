using SDL2;

namespace TestGame.GameObjects;

internal class Simple : IGameObject {
    private const float Speed = 0.1f;
    private float _xSpeed;
    private float _ySpeed;
    private Color _color;

    #region Implementation of IGameObject

    /// <inheritdoc />
    public float X { get; set; }

    /// <inheritdoc />
    public float Y { get; set; }

    /// <inheritdoc />
    public int Width => 96;

    /// <inheritdoc />
    public int Height => 64;

    /// <inheritdoc />
    public string Name => "Simple";

    public Simple(nint rendererPtr) {
        RendererPtr = rendererPtr;
        _xSpeed = Speed;
        _ySpeed = Speed;
        _color = Game.GetRandomColor();
    }

    public IntPtr RendererPtr { get; }

    /// <inheritdoc />
    public void Draw() {
        FRect rect = new() { X = X, Y = Y, W = Width, H = Height };
        // _ = SDL.SetRenderDrawColor(RendererPtr, 0, 0, 0, 255);
        // _ = SDL.RenderClear(RendererPtr);
        _ = SDL.SetRenderDrawColor(RendererPtr, _color.R, _color.G, _color.B, _color.A);
        _ = SDL.RenderDrawRectF(RendererPtr, ref rect);
        SDL.RenderPresent(RendererPtr);
    }

    /// <inheritdoc />
    public void Update(Event e) {
        _ = SDL.GetRendererOutputSize(RendererPtr, out int rx, out int by);

        if (
            X <= 0 && Y <= 0 // Top left
            || X >= rx - Width && Y <= 0 // Top right
            || X <= 0 && Y >= by - Height // Bottom left
            || X >= rx - Width && Y >= by - Height // Bottom right
        ) {
            // Change the Color
            _color = Game.GetRandomColor();
        }

        if (X < 0 || X > rx - Width) {
            _xSpeed = -_xSpeed;
        }

        if (Y < 0 || Y > by - Height) {
            _ySpeed = -_ySpeed;
        }

        X += _xSpeed;
        Y += _ySpeed;
    }

    #endregion
}