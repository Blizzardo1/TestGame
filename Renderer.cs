using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
using System.Runtime.InteropServices;
using Tex = SharpSDL3.Textures;
namespace TestGame;

public  abstract class Renderer {
    protected nint RendererPtr;

    private static readonly Log? _log = Log.GetCurrentClassLogger(LogCategory.Video);

    private static readonly Dictionary< string, Font > LoadedFonts = [];

    private const string FontName = "default";
    private const string FontPath = "default.ttf";

    private const int FontSize = 18;

    private string _renderingFont = "";

    private FRect _bodyRect;
    private FRect _rect;
    private bool _result;

    ~Renderer() {
        _rect = default;
    }

    /// <summary>
    /// Initializes the Rendering Engine for a class
    /// </summary>
    /// <param name="renderer">The renderer to pass through</param>
    /// <param name="fontPath">The font file to load, else Consolas</param>
    /// <param name="fontName">The name of the font to load, else default</param>
    /// <param name="fontSize">A real number depicting the size of the font</param>
    public void Initialize(nint window, nint renderer, string fontPath = FontPath, string fontName = FontName,
        float fontSize = FontSize) {
        if (RendererPtr != nint.Zero) {
            return;
        }


        Font f = Ttf.OpenFont(fontPath, fontSize);

        if(f.Ascent == 0) {
            _log?.Error($"Failed to load font {fontPath}: {Sdl.GetError()}");
            return;
        }

        _renderingFont = fontPath;
        _rect = new FRect { X = 0, Y = 0, W = 0, H = 0 };

        if(renderer == nint.Zero) {
            _log?.Error("Renderer is not initialized");
            return;
        }

        Sdl.GetWindowSize(window, out int width, out int height);
        _bodyRect = new FRect { X = 0, Y = 0, W = width, H = height };

        LoadedFonts.TryAdd(fontName, f);
        RendererPtr = renderer;
    }

    public static Font GetFontStatic(string fontName = FontName,
        string fontPath = FontPath,
        int size = FontSize) {
        string font = $"{fontName}-{size}";

        if (LoadedFonts.TryGetValue(font, out Font value)) {
            return value;
        }

        if(fontPath.IsEmpty()) {
            _log?.Warn($"Font path \"{fontPath}\" is empty: {Sdl.GetError()}");
            fontPath = FontPath;
        }

        Font f = Ttf.OpenFont(fontPath, size);

        LoadedFonts.Add(font, f);
        _log?.Debug($"Loaded font: {font}:{fontPath}");
        return LoadedFonts[ font ];
    }

    public Font GetFont(string fontName = FontName, int size = FontSize) =>
        GetFontStatic(fontName, _renderingFont, size);

    protected static void CloseFonts() {
        foreach ((string? k, Font font ) in LoadedFonts) {
            Ttf.CloseFont(font);
            LoadedFonts.Remove(k);
        }
    }

    private void RenderText(string? text, float x, float y, Color color, Font font) {
        if (text is null || text.IsEmpty()) {
            return;
        }

        if (RendererPtr == nint.Zero) {
            _log?.Error("Renderer is not initialized");
            return;
        }

        FSize size = MeasureString(RendererPtr, font, text);

        nint surface = Ttf.RenderTextSolid(font, text, (Size)size, color);
        
        if (surface == nint.Zero) {
            _log?.Error($"Error rendering text for \"{text}\": {Sdl.GetError()}");
            return;
        }
        nint texture = Tex.CreateTextureFromSurface(RendererPtr, surface);

        if (texture == nint.Zero) {
            _log?.Error($"Error creating Texture for text \"{text}\": {Sdl.GetError()}");
            return;
        }

        Vector2 tS = Tex.GetTextureSize(texture);
        _rect = _rect with { X = x, Y = y, W = tS.X, H = tS.Y};
        _result = Render.RenderTexture(RendererPtr, texture, nint.Zero, ref _rect);
        if (!_result) {
            _log?.Error($"Error copying text for \"{text}\": {Sdl.GetError()}");
        }

        Render.DestroyTexture(texture);
        Sdl.DestroySurface(surface);
    }

    /// <summary>
    /// Draws a string to the screen
    /// </summary>
    /// <param name="text">The text to be drawn</param>
    /// <param name="x">Absolute X Coordinate</param>
    /// <param name="y">Absolute Y Coordinate</param>
    /// <param name="color">The BackgroundColor to use</param>
    public void RenderText(string? text, float x, float y, Color color) {
        // #TODO: Might break if either no font is loaded, or the wrong font is loaded first.
        RenderText(text, x, y, color, GetFont(FontName, 12));
    }

    public void RenderText(string? text, int size, float x, float y, Color color) {
        RenderText(text, x, y, color, GetFont(FontName, size));
    }

    public void RenderText(string? text, int size, string fontName, float x, float y, Color c) {
        RenderText(text, x, y, c, GetFont(fontName, size));
    }

    protected static FSize MeasureString(nint renderer, Font font, string text) {
        
        if(font.Name == null) {
            _log?.Error($"Font is not loaded: {Sdl.GetError()}");
            return new FSize();
        }

        TextEngine engine = Ttf.CreateRendererTextEngine(renderer);
        if (engine.Handle == nint.Zero) {
            _log?.Error($"Error creating text engine: {Sdl.GetError()}");
            return new FSize();
        }
        Ttf.MeasureString(font, text, 0, out Size measuredSize);

        try {
            Text tText = Ttf.CreateText(engine, font, text);
            try {
                _ = Ttf.GetTextSize(tText, out int width, out int height);
                return new FSize { Width = width, Height = height };
            } catch {
                _log?.Error($"Error measuring text \"{text}\": {Sdl.GetError()}");
                return new FSize();
            } finally {
                if (tText.Handle != nint.Zero) {
                    Ttf.DestroyText(tText);
                }
            }
        } finally {
            Ttf.DestroyRendererTextEngine(engine);
        }
    }
}