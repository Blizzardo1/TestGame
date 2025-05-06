using SDL2;
using System.Runtime.InteropServices;
// using System.Windows.Media.Media3D;

namespace TestGame.GameObjects.Textures; 
/// <summary>
/// A Transparent 32-Bit Texture
/// </summary>
public class Sprite32(GameContext context, nint? parentTexture = null,
    string? imagePath = null, Rect? childRect = null)
    : Texture(context.RendererPtr, context.Width, context.Height) {

    public string? ImagePath => imagePath;

    private Rect _source;
    private Rect _destination;

    private static readonly Logger? _log = Logger.GetCurrentClassLogger(LogCategory.Video);

    /// <summary>
    /// Is set if this <see cref="Sprite32"/> is loaded by an image, else, it's strictly from memory
    /// </summary>
    public bool LoadedByImage => imagePath != null;

    public override void Initialize() {
        nint surface = SDL.CreateRGBSurfaceWithFormat(
          0, // Always Zero
          (int)context.Width,
          (int)context.Height,
          32, // 32-Bit
          PixelFormats.PIXELFORMAT_BGRA8888 // ARGB Formatted
      );

        _source = context.Rect;

        if (childRect is not null) {
            _destination = childRect.Value with {
                W = (int)context.Width,
                H = (int)context.Height
            };
        }

        TexturePtr = UnknownTexture(RendererPtr, (int)context.Width, (int)context.Height);
        // If we have an image path, we can start opening that image if found.
        if (imagePath is not null) {
            SDL.DestroyTexture(TexturePtr); // Destroy the unknown texture
            SDL.FreeSurface(surface); // Since we have an image path, we can free this and load the image.
            surface = Image.Load(imagePath);
        }

        if (surface == nint.Zero) {
            _log?.Error($"Failed to create surface");
            return;
        }

        if (parentTexture is not null) {
            _log?.Debug($"Rendering to parent texture {parentTexture}");
            TexturePtr = parentTexture.Value;
        }

        if (TexturePtr == nint.Zero) {
            TexturePtr = SDL.CreateTextureFromSurface(context.RendererPtr, surface);
        }

        SDL.FreeSurface(surface);
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
        _ = SDL.QueryTexture(texture, out _, out _, out _, out int h);
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

    public void RenderTile() {
        _ = SDL.RenderCopy(RendererPtr, TexturePtr, ref _source, ref _destination);
    }
}