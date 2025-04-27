using SDL2;

namespace TestGame.GameObjects.Textures; 
public class Sprite : Texture {
    private readonly string _imagePath;

    public string ImagePath => _imagePath;

    /// <inheritdoc />
    public Sprite(IntPtr rendererPtr, int width, int height, string imagePath)
        : base(rendererPtr, width, height) {
        _imagePath = imagePath;

        nint surface = SDL.LoadBMP(_imagePath);
        if (surface == IntPtr.Zero) {
            throw new Exception($"Failed to load image at {_imagePath}");
        }

        TexturePtr = SDL.CreateTextureFromSurface(rendererPtr, surface);
        SDL.FreeSurface(surface);
    }
}