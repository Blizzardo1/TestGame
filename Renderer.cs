using NLog;
using SDL2;
using SDL2.TTF;
using Color = SDL2.Color;

namespace TestGame;

public abstract class Renderer {
    protected nint RendererPtr;
    protected Font Font;

    private const int FontSize = 18;

    private Logger _log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Initializes the Rendering Engine for a class
    /// </summary>
    /// <param name="renderer">The renderer to pass through</param>
    /// <param name="font">The font file to load, else Arial</param>
    /// <param name="fontSize">A real number depicting the size of the font</param>
    public void Initialize(nint renderer, string font = @"C:\Windows\Fonts\Arial.ttf", int fontSize = FontSize) {
        Font = TTF.OpenFont(font, fontSize);

        if (Font.Pointer == nint.Zero)
        {
            _log.Error($"Failed to load font: {SDL.GetError()}");
        }

        RendererPtr = renderer;
    }

    /// <summary>
    /// Draws a string to the screen
    /// </summary>
    /// <param name="text">The text to be drawn</param>
    /// <param name="x">Absolute X Coordinate</param>
    /// <param name="y">Absolute Y Coordinate</param>
    /// <param name="color">The BackgroundColor to use</param>
    public void RenderText(string? text, int x, int y, Color color) {
        nint surface = TTF.RenderTextSolid(Font, text, color);
        if (surface == nint.Zero) {
            _log.Error($"Error rendering text: {SDL.GetError()}");
            return;
        }

        nint texture = SDL.CreateTextureFromSurface(RendererPtr, surface);
        
        if (texture == nint.Zero) {
            _log.Error($"Failed to create texture: {SDL.GetError()}");
            return;
        }
        
        _ = SDL.QueryTexture(
            texture,
            out _,
            out _,
            out int textureWidth,
            out int textureHeight
        );
        var rect = new Rect { X = x, Y = y, W = textureWidth, H = textureHeight };
        int result = SDL.RenderCopy(RendererPtr, texture, nint.Zero, ref rect);
        if (result != 0)
        {
            _log.Error($"Error rendering text: {SDL.GetError()}");
        }
        
        SDL.FreeSurface(surface);
        SDL.DestroyTexture(texture);
    }

    protected Size MeasureString(string text) {
        _ = TTF.SizeText(Font, text, out int width, out int height);
        return new Size { Width = width, Height = height };
    }
}