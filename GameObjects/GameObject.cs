
using System.Text.Json.Serialization;
using SharpSDL3.Structs;
using SharpSDL3.TTF;

namespace TestGame.GameObjects;

public abstract class GameObject : Renderer, IRenderer {
    protected FRect Frect;

    #region Implementation of IRenderable

    /// <inheritdoc />
    public float X
    {
        get => Frect.X;
        set => Frect.X = value;
    }

    /// <inheritdoc />
    public float Y
    {
        get => Frect.Y;
        set => Frect.Y = value;
    }

    /// <inheritdoc />
    public float Z { get; set; }

    /// <inheritdoc />
    public float Width
    {
        get => Frect.W;
        set => Frect.W = value;
    }

    /// <inheritdoc />
    public float Height
    {
        get => Frect.H;
        set => Frect.H = value;
    }

    [JsonIgnore]
    public Font Font {get; set;}

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