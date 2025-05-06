using SDL2;

namespace TestGame.GameObjects.Textures; 
public abstract class Texture : GameObject {
    protected FRect Rect => frect;

    public nint TexturePtr { get; protected set; }

    #region Implementation of IRenderable

    ~Texture() {
        SDL.DestroyTexture(TexturePtr);
    }

    public Texture(nint rendererPtr, int width, int height) {
        RendererPtr = rendererPtr;
        Name = "Texture";
        Width = width;
        Height = height;
    }

    public Texture(nint rendererPtr, float width, float height) {
        RendererPtr = rendererPtr;
        Name = "Texture";
        Width = (int)width;
        Height = (int)height;
    }

    /// <inheritdoc />
    public override void Draw() {
        _ = SDL.RenderCopyF(RendererPtr, TexturePtr, nint.Zero, ref frect);
    }

    /// <inheritdoc />
    public override void Update(Event e) { }

    #endregion

    public static implicit operator nint(Texture texture) {
        return texture.TexturePtr;
    }
}