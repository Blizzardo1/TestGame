using SDL2;

namespace TestGame.GameObjects;

internal class Simple : GameObject {
    private const float Speed = 0.1f;
    private float _xSpeed;
    private float _ySpeed;
    private Color _color;

    #region Implementation of IRenderable

    public Simple(GameContext context) {
        Name = "Simple";
        Width = 96;
        Height = 64;
        X = 0;
        Y = 0;
        Z = 0;
        RendererPtr = context.RendererPtr;
        _xSpeed = Speed;
        _ySpeed = Speed;
        _color = Core.GetRandomColor();
    }

    /// <inheritdoc />
    public override void Draw() {
        FRect rect = new() { X = X, Y = Y, W = Width, H = Height };
        // _ = SDL.SetRenderDrawColor(RendererPtr, 0, 0, 0, 255);
        // _ = SDL.RenderClear(RendererPtr);
        _ = SDL.SetRenderDrawColor(RendererPtr, _color.R, _color.G, _color.B, _color.A);
        _ = SDL.RenderDrawRectF(RendererPtr, ref rect);
        SDL.RenderPresent(RendererPtr);
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        _ = SDL.GetRendererOutputSize(RendererPtr, out int rx, out int by);

        if (
            X <= 0 && Y <= 0 // Top left
            || X >= rx - Width && Y <= 0 // Top right
            || X <= 0 && Y >= by - Height // Bottom left
            || X >= rx - Width && Y >= by - Height // Bottom right
        ) {
            // Change the Color
            _color = Core.GetRandomColor();
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