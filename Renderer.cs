using System.Text.Json.Serialization;
using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.TTF;

namespace TestGame;

public abstract class Renderer {
    protected nint RendererPtr;

    private bool _initialized;

    private static readonly Log Log = Log.GetCurrentClassLogger(LogCategory.Video);

    private const string FontName = "default";
    private const string FontPath = "default.ttf";

    [JsonIgnore]
    protected Font Font { get; set; }

    private const int FontSize = 18;

    [JsonIgnore]
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

        Font = OpenFont(fontName, fontPath);

        if (Font.Handle == nint.Zero) {
            Log.Error($"Font has no handle... {fontPath}: {Sdl.GetError()}");
            return;
        }

        if(renderer == nint.Zero) {
            Log.Error("Renderer is not initialized");
            return;
        }
        
        RendererPtr = renderer;
        _initialized = true;
    }

    public nint GetRenderer() {
        if (RendererPtr != nint.Zero) return RendererPtr;
        Log.Error("Renderer is not initialized");
        return nint.Zero;
    }

    public static Font OpenFont(string fontName = FontName,
        string fontPath = FontPath) {

        if (fontName.IsEmpty()) {
            Log.Warn($"Font path \"{fontName}\" is empty. Using Default: {FontName}");
            fontPath = FontPath;
        }

        if (fontPath.IsEmpty()) {
            Log.Warn($"Font path \"{fontPath}\" is empty. Using Default: {FontPath}");
            fontPath = FontPath;
        }

        Font font = Ttf.OpenFont(fontPath, FontSize);

        Log.Debug($"Loaded font: {fontName}:{fontPath}");
        return font;
    }

    private void RenderText(string? text, float x, float y, Color color, Font font) {
        if (text is null || text.IsEmpty()) {
            return;
        }

        if (RendererPtr == nint.Zero) {
            Log.Error("Renderer is not initialized");
            return;
        }
        if (_textEngine.Handle == nint.Zero) {
            _textEngine = Ttf.CreateRendererTextEngine(RendererPtr);
            if (_textEngine.Handle == nint.Zero) {
                Log.Error($"Error creating text engine: {Sdl.GetError()}");
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
        RenderText(text, x, y, color, Font);
    }
}