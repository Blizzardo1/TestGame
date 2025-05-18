
using SharpSDL3.Structs;
using SharpSDL3.TTF;

namespace TestGame.GameObjects;

public abstract class GameObject : Renderer, IRenderer {
    protected FRect frect;
    private Font _font;

    #region Implementation of IRenderable

    /// <inheritdoc />
    public float X
    {
        get => frect.X;
        set => frect.X = value;
    }

    /// <inheritdoc />
    public float Y
    {
        get => frect.Y;
        set => frect.Y = value;
    }

    /// <inheritdoc />
    public float Z { get; set; }

    /// <inheritdoc />
    public float Width
    {
        get => frect.W;
        set => frect.W = value;
    }

    /// <inheritdoc />
    public float Height
    {
        get => frect.H;
        set => frect.H = value;
    }

    public Font Font
    {
        get => _font;
        set => _font = value;
    }

    /// <inheritdoc />
    public string? Name { get; protected set; }

    public abstract void Initialize();

    /// <inheritdoc />
    public abstract void Draw();

    /// <inheritdoc />
    public abstract void Update(Event e);

    public bool Collides(IRenderer other) {
        return X < other.X + other.Width &&
               X + Width > other.X &&
               Y < other.Y + other.Height &&
               Y + Height > other.Y;
    }

    #endregion
}