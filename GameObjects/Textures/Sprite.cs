using SDL2;

namespace TestGame.GameObjects.Textures;
/// <inheritdoc />
public class Sprite(GameContext context, string imagePath) : Texture(context.RendererPtr, (int)context.Width, (int)context.Height) {
    private string _imagePath;

    public string ImagePath => _imagePath;

    public override void Initialize() {
        _imagePath = imagePath;

        nint surface = SDL.LoadBMP(_imagePath);
        if (surface == IntPtr.Zero) {
            throw new Exception($"Failed to load image at {_imagePath}");
        }

        TexturePtr = SDL.CreateTextureFromSurface(context.RendererPtr, surface);
        SDL.FreeSurface(surface);
    }
}