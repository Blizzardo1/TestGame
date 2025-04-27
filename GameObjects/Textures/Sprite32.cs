using SDL2;
using System.Runtime.InteropServices;
// using System.Windows.Media.Media3D;

namespace TestGame.GameObjects.Textures; 
/// <summary>
/// A Transparent 32-Bit Texture
/// </summary>
public class Sprite32 : Texture {
    private readonly string _imagePath;

    public string ImagePath => _imagePath;

    private Rect _source;
    private Rect _destination;

    /// <summary>
    /// Is set if this <see cref="Sprite32"/> is loaded by an image, else, it's strictly from memory
    /// </summary>
    public bool LoadedByImage => _imagePath != null;

    public Sprite32(nint parentTexture, nint rendererPtr, int x, int y, int width, int height, Rect childRect)
        : base(rendererPtr, width, height) {
        _imagePath = "";
        _source = new() {
            X = x,
            Y = y,
            W = width,
            H = height
        };

        _destination = childRect with {
            W = width,
            H = height
        };

        _ = SDL.RenderCopy(rendererPtr, parentTexture, ref _source, ref _destination);
    }

    public static nint UnknownTexture(nint rendererPtr, int width, int height) {
        nint surface = SDL.CreateRGBSurfaceWithFormat(
            0, // Always Zero
            width,
            height,
            32, // 32-Bit
            PixelFormats.PIXELFORMAT_BGRA8888 // RGBA Formatted
        );

        nint texture = SDL.CreateTextureFromSurface(rendererPtr, surface);
        _ = SDL.QueryTexture(texture, out uint format, out int access, out int w, out int h);
        _ = SDL.GetTextureAlphaMod(texture, out byte alpha);
        _ = SDL.SetTextureAlphaMod(texture, (byte)( alpha - 25 ));
        _ = SDL.SetTextureBlendMode(texture, BlendMode.Add);
        _ = SDL.LockTexture(texture, nint.Zero, out nint pixels, out int pitch);
        for (int i = 0; i < pitch * h; i++) {
            Marshal.WriteByte(pixels, 0x23);
        }

        SDL.UnlockTexture(texture);

        return texture;
    }

    /// <inheritdoc />
    public Sprite32(nint rendererPtr, int width, int height, string? imagePath)
        : base(rendererPtr, width, height) {
        nint surface = SDL.CreateRGBSurfaceWithFormat(
            0, // Always Zero
            width,
            height,
            32, // 32-Bit
            PixelFormats.PIXELFORMAT_BGRA8888 // ARGB Formatted
        );
        _imagePath = ""; // set to empty for now.
        TexturePtr = UnknownTexture(RendererPtr, width, height);
        // If we have an image path, we can start opening that image if found.
        if (imagePath is not null) {
            _imagePath = imagePath;
            SDL.DestroyTexture(TexturePtr); // Destroy the unknown texture
            SDL.FreeSurface(surface); // Since we have an image path, we can free this and load the image.
            surface = Image.Load(_imagePath);
        }

        if (surface == nint.Zero) {
            throw new Exception($"Failed to load image at {_imagePath}");
        }

        TexturePtr = SDL.CreateTextureFromSurface(rendererPtr, surface);
        SDL.FreeSurface(surface);
    }

    public void RenderTile() {
        _ = SDL.RenderCopy(RendererPtr, TexturePtr, ref _source, ref _destination);
    }
}