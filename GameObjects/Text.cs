

using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.TTF;

namespace TestGame.GameObjects; 
public class TextString : GameObject, IDisposable {
    private const int ShadowDivisor = 10;

    private static readonly Log _log = Log.GetCurrentClassLogger(LogCategory.Custom, "Game");

    private bool _disposed;

    private TextEngine _textEngine;
    private Text _sText;

    ~TextString() {
        Dispose(false);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected Dispose method to handle cleanup
    protected virtual void Dispose(bool disposing) {
        if(_disposed) {
            return;
        }

        if (disposing) {
            // No managed resources to free
        }

        if (Font.Handle != nint.Zero) {
            Font.Close();
        }
        if (_sText.Handle != nint.Zero) {
            Ttf.DestroyText(_sText);
            _sText = default;
        }
        if (_textEngine.Handle != nint.Zero) {
            Ttf.DestroyRendererTextEngine(_textEngine);
            _textEngine = default;
        }
        _disposed = true;
    }

    public TextString(Font font, float size, string text) {
        RendererPtr = Engine.Game!.GetRenderer();

        if (font.Handle == nint.Zero) {
            _log.Error($"Font is not loaded: {Sdl.GetError()}");
            return;
        }
        font.Size = size;
        Font = font;

        _textEngine = Ttf.CreateRendererTextEngine(RendererPtr);
        _sText = Ttf.CreateText(_textEngine, Font, text);
        Text = text;

        CenterText(Width, Height);

        uint props = Ttf.GetTextProperties(_sText.Handle);
        _log.Info($"Initialized new TextString({text}) using Font {font.Name}-{font.Size} with Dimensions({X}, {Y}, {Width}, {Height})");
    }

    public Color ForegroundColor {
        get => Ttf.GetTextColor(_sText);
        set => Ttf.SetTextColor(_sText, value);
    }

    public Color BackgroundColor { get; set; }

    public Color ShadowColor { get; set; }

    public string Text {
        get => _sText.TextStr;
        set {
            if (_sText.TextStr == value)
                return;
            _sText.TextStr = value;
            Size size = Ttf.GetTextSize(_sText);
            Width = size.Width;
            Height = size.Height;
            if(!Ttf.UpdateText(_sText)) {
                _log.Error($"Failed to update text: {Sdl.GetError()}");
            }
        }
    }

    public float FontSize {
        get {
            if (Font.Handle == nint.Zero) {
                Font = OpenFont();
            }
            return Font.Size;
        }
        set {
            const float Tolerance = 0.0001f;
            if (Math.Abs(Font.Size - value) < Tolerance)
                return;
            var f = Font;
            f.Size = value;
            Font = f;
        }
    }

    public bool ShowShadow { get; set; }

    public int Length => _sText.TextStr?.Length ?? 0;

    public override void Initialize() { }

    public static implicit operator string(TextString text) => text.Text ?? string.Empty;

    private Color CalculateShadow(float x, float y) {
        var c = ShadowColor;
        int alpha = 255 - (int)((x + y) * 2);

        if (alpha < 0) {
            c.A = 0;
        } else if (alpha > 255) {
            c.A = 255;
        } else {
            c.A = (byte)alpha;
        }

        return c;
    }

    public void SetRelativePosition(float x, float y) {
        X = x;
        Y = y;
        Ttf.SetTextPosition(_sText, (int)x, (int)y);
    }

    public void SetRelativePosition(FPoint point) => SetRelativePosition(point.X, point.Y);

    /// <summary>
    /// Centers the text in the given rectangle.
    /// </summary>
    public void CenterText() => CenterText(Width, Height);

    /// <summary>
    /// Centers the text in the given rectangle.
    /// </summary>
    /// <param name="width">Absolute Width of the Frame</param>
    /// <param name="height">Absolute Height of the Frame</param>
    public void CenterText(float width, float height) => CenterText(0, 0, width, height);

    /// <summary>
    /// Centers the text in the given rectangle.
    /// </summary>
    /// <param name="x">Relative X</param>
    /// <param name="y">Relative Y</param>
    /// <param name="w">Absolute Width of the Frame</param>
    /// <param name="h">Absolute Height of the Frame</param>
    public void CenterText(float x, float y, float w, float h) {
        if (Font.Handle == nint.Zero) {
            Font = OpenFont();
        }
        Size size = Font.GetTextSize(Text);
        X = (w / 2) - (size.Width / 2) + x;
        Y = (h / 2) - (size.Height / 2) + y;
        //SetPosition((int)X, (int)Y);
    }

    public override void Draw() {
        if (ShowShadow) {
            float shadowOffset = Font.Size / ShadowDivisor;
            RenderText(Text, (int)(X + shadowOffset),
                (int)(Y + shadowOffset),
                CalculateShadow(shadowOffset, shadowOffset));
        }
        Ttf.DrawRendererText(_sText, X, Y);
        Sdl.SetRenderDrawColor(RendererPtr, Colors.Colors.Green);
        Sdl.RenderRect(RendererPtr, ref frect);
    }

    public override void Update(Event e) {
        Size size = Font.GetTextSize(Text);
        frect = new FRect {
            X = X,
            Y = Y,
            W = size.Width,
            H = size.Height
        };
    }
}
