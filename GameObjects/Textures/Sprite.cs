
using SharpSDL3;

namespace TestGame.GameObjects.Textures;
/// <inheritdoc />
public  class Sprite(GameContext context, string imagePath) : Texture(context.RendererPtr, (int)context.Width, (int)context.Height) {
    private string _imagePath = "";

    public string ImagePath => _imagePath;

    public override void Initialize() {
        _imagePath = imagePath;

        nint surface = Sdl.LoadBmp(_imagePath);
        if (surface == nint.Zero) {
            throw new FileNotFoundException($"Failed to load image at {_imagePath}");
        }

        TexturePtr = Sdl.CreateTextureFromSurface(context.RendererPtr, surface);
        Sdl.DestroySurface(surface);
    }
}