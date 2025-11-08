using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;

namespace TestGame.GameObjects.Textures; 
/// <summary>
/// A Transparent 32-Bit Texture
/// </summary>
public  class Sprite32(GameContext context, nint? parentTexture = null,
    string? imagePath = null, FRect? childRect = null)
    : Texture(context.RendererPtr, context.Width, context.Height) {

    public string? ImagePath => imagePath;

    private FRect _source;
    private FRect _destination;

    private static readonly Log Log = Log.GetCurrentClassLogger(LogCategory.Video);

    /// <summary>
    /// Is set if this <see cref="Sprite32"/> is loaded by an image, else, it's strictly from memory
    /// </summary>
    public bool LoadedByImage => imagePath != null;

    public override void Initialize() {
        nint surface = Sdl.CreateSurface (
          (int)context.Width,
          (int)context.Height,
          PixelFormat.Bgra8888 // ARGB Formatted
      );

        _source = context.Rect;

        if (childRect is not null) {
            _destination = childRect.Value with {
                W = context.Width,
                H = context.Height
            };
        }

        TexturePtr = UnknownTexture(RendererPtr, (int)context.Width, (int)context.Height);
        // If we have an image path, we can start opening that image if found.
        if (imagePath is not null) {
            Sdl.Free(TexturePtr); // Destroy the unknown texture
            Sdl.DestroySurface(surface); // Since we have an image path, we can free this and load the image.
            IOStream io = Sdl.IOFromFile(imagePath, "r");
            surface = Sdl.LoadPngIo(io.Handle);// needs to pass io instead of the handle. Fix the libreary
        }

        if (surface == nint.Zero) {
            Log.Error($"Failed to create surface");
            return;
        }

        if (parentTexture is not null) {
            Log.Debug($"Rendering to parent texture {parentTexture}");
            TexturePtr = parentTexture.Value;
        }

        if (TexturePtr == nint.Zero) {
            TexturePtr = Sdl.CreateTextureFromSurface(context.RendererPtr, surface);
        }

        Sdl.DestroySurface(surface);
    }

    private static nint UnknownTexture(nint rendererPtr, int width, int height) {
        nint surface = Sdl.CreateSurface(
            width,
            height,
            PixelFormat.Bgra8888 // RGBA Formatted
        );
        
        nint texture = Sdl.CreateTextureFromSurface(rendererPtr, surface);
        _ = Sdl.GetTextureSize(texture, out _, out float h);
        _ = Sdl.GetTextureAlphaMod(texture, out byte alpha);
        _ = Sdl.SetTextureAlphaMod(texture, (byte)( alpha - 25 ));
        _ = Sdl.SetTextureBlendMode(texture, (uint)BlendMode.Add);
        _ = Sdl.LockTexture(texture, nint.Zero, out nint pixels, out int pitch);
        for (int i = 0; i < pitch * h; i++) {
            Marshal.WriteByte(pixels, 0x23);
        }

        Sdl.UnlockTexture(texture);

        return texture;
    }

    public void RenderTile() {
        _ = Sdl.RenderTexture(RendererPtr, TexturePtr, ref _source, ref _destination);
    }
}