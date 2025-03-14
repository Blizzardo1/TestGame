using NLog;
using SDL2;
using SDL2.TTF;
using System.Reflection.Metadata;
using Color = SDL2.Color;

namespace TestGame;

public abstract class Renderer {
    protected nint RendererPtr;

    private static Dictionary<string, Font> LoadedFonts = [];

    private const string FontName = "consolas";
    private const string FontPath = @"C:\Windows\Fonts\consola.ttf";
    private const int FontSize = 18;

    private string _renderingFont = "";

    private static Logger? _log = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// Initializes the Rendering Engine for a class
    /// </summary>
    /// <param name="renderer">The renderer to pass through</param>
    /// <param name="fontPath">The font file to load, else Consolas</param>
    /// <param name="fontName">The name of the font to load, else default</param>
    /// <param name="fontSize">A real number depicting the size of the font</param>
    public void Initialize(nint renderer, string fontPath = FontPath, string fontName = FontName, int fontSize = FontSize) {
        if (RendererPtr != nint.Zero) {
            _log?.Error("Renderer already initialized.");
            return;
        }

        
        Font f = TTF.OpenFont(fontPath, fontSize);


        if (f.Pointer == nint.Zero)
        {
            _log?.Error($"Failed to load font: {SDL.GetError()}");
        }

        _renderingFont = fontPath;
        LoadedFonts.TryAdd(fontName, f);
        RendererPtr = renderer;
    }

    public static Font GetFont(string fontName = FontName, string fontPath = FontPath, int size = FontSize) {
        string font = $"{fontName}-{size}";

        if (LoadedFonts.TryGetValue(font, out Font value)) {
            return value;
        }

        Font f = TTF.OpenFont(fontPath, size);

        LoadedFonts.Add(font, f);
        _log?.Info($"Loaded font: {font}:{fontPath}");
        return LoadedFonts[font];
    }

    public Font GetFont(string fontName = FontName, int size = FontSize) => GetFont(fontName, _renderingFont, size);

    protected static void CloseFonts() {
        foreach ((var k, var v) in LoadedFonts) {
            TTF.CloseFont(v);
            LoadedFonts.Remove(k);
        }
    }

    private void RenderText(string? text, int x, int y, Color color, Font font) {
        // _log.Debug($"Rendering text: {text}");
        nint surface = TTF.RenderTextSolid(font, text, color);
        if (surface == nint.Zero) {
            _log?.Error($"Error rendering text: {SDL.GetError()}");
            return;
        }

        nint texture = SDL.CreateTextureFromSurface(RendererPtr, surface);

        if (texture == nint.Zero) {
            _log?.Error($"Failed to create texture for Text [\"{text}\"]: {SDL.GetError()}");
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
        if (result != 0) {
            _log?.Error($"Error rendering text: {SDL.GetError()}");
        }

        SDL.FreeSurface(surface);
        SDL.DestroyTexture(texture);
    }

    /// <summary>
    /// Draws a string to the screen
    /// </summary>
    /// <param name="text">The text to be drawn</param>
    /// <param name="x">Absolute X Coordinate</param>
    /// <param name="y">Absolute Y Coordinate</param>
    /// <param name="color">The BackgroundColor to use</param>
    public void RenderText(string? text, int x, int y, Color color) {
        // TODO: Might break if either no font is loaded, or the wrong font is loaded first.
        RenderText(text, x, y, color, GetFont(FontName, 12));
    }

    public void RenderText(string? text, int size, int x, int y, Color color) {
        RenderText(text, x, y, color, GetFont(FontName, size));
    }

    public void RenderText(string? text, int size, string fontName, int x, int y, Color c) {
        RenderText(text, x, y, c, GetFont(fontName, size));
    }

    protected Size MeasureString(Font font, string text) {
        _ = TTF.SizeText(font, text, out int width, out int height);
        return new Size { Width = width, Height = height };
    }
}