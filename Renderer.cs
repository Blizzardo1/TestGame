using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
using System.Runtime.InteropServices;
using Tex = SharpSDL3.Textures;
namespace TestGame;

public  abstract class Renderer {
    protected nint RendererPtr;

    private bool _initialized;

    private static readonly Log? _log = Log.GetCurrentClassLogger(LogCategory.Video);

    private static readonly Dictionary< string, Font > LoadedFonts = [];

    private const string FontName = "default";
    private const string FontPath = "default.ttf";

    private const int FontSize = 18;

    private string _renderingFont = "";

    private TextEngine _textEngine;

    /// <summary>
    /// Initializes the Rendering Engine for a class
    /// </summary>
    /// <param name="renderer">The renderer to pass through</param>
    /// <param name="fontPath">The font file to load, else Consolas</param>
    /// <param name="fontName">The name of the font to load, else default</param>
    /// <param name="fontSize">A real number depicting the size of the font</param>
    public void Initialize(nint renderer, string fontPath = FontPath, string fontName = FontName) {

        if (_initialized) {
            return;
        }

        Font f = GetFontStatic(fontName, fontPath);

        if (f.Handle == nint.Zero) {
            _log?.Error($"Font has no handle... {fontPath}: {Sdl.GetError()}");
            return;
        }

        _renderingFont = fontPath;
        if(renderer == nint.Zero) {
            // Who called?
            _log?.Error("Renderer is not initialized");
            return;
        }
        
        RendererPtr = renderer;
        _initialized = true;
    }

    public static Font GetFontStatic(string fontName = FontName,
        string fontPath = FontPath) {

        if (LoadedFonts.TryGetValue(fontName, out Font value)) {
            return value;
        }

        if (fontName.IsEmpty()) {
            _log?.Warn($"Font path \"{fontName}\" is empty. Using Default: {FontName}");
            fontPath = FontPath;
        }

        if (fontPath.IsEmpty()) {
            _log?.Warn($"Font path \"{fontPath}\" is empty. Using Default: {FontPath}");
            fontPath = FontPath;
        }

        Font f = Ttf.OpenFont(fontPath, FontSize);

        LoadedFonts.Add(fontName, f);
        _log?.Debug($"Loaded font: {fontName}:{fontPath}");
        return LoadedFonts[ fontName ];
    }

    public Font GetFont(string fontName = FontName) =>
        GetFontStatic(fontName, _renderingFont);

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
        if (_textEngine.Handle == nint.Zero) {
            _textEngine = Ttf.CreateRendererTextEngine(RendererPtr);
            if (_textEngine.Handle == nint.Zero) {
                _log?.Error($"Error creating text engine: {Sdl.GetError()}");
                return;
            }
        }

        Text txt = Ttf.CreateText(_textEngine, font, text);

        Ttf.SetTextColor(txt, color);
        
        Ttf.DrawRendererText(txt, x, y);

        Ttf.DestroyText(txt);
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
        RenderText(text, x, y, color, GetFont(FontName));
    }

    public void RenderText(string? text, string fontName, float x, float y, Color c) {
        RenderText(text, x, y, c, GetFont(fontName));
    }

    protected static FSize MeasureString(Font font, string text) {
        
        if(font.Handle == nint.Zero) {
            _log?.Error($"Font is not loaded: {Sdl.GetError()}");
            return new FSize();
        }

        if(!Ttf.MeasureString(font, text, 0, out Size measuredSize)) {
            _log?.Error($"Error measuring text \"{text}\": {Sdl.GetError()}");
            return new FSize();
        }

        return new(measuredSize.Width, measuredSize.Height);
    }
}