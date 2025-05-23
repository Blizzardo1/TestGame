

using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
using TestGame.Colors;

namespace TestGame.GameObjects; 
public class TextString : GameObject {
    private const int ShadowDivisor = 10;

    private static readonly Log _log = Log.GetCurrentClassLogger(LogCategory.Custom, "Game");

    private TextEngine _textEngine;
    private Text _sText;

    ~TextString() {
        if (_sText.Handle != nint.Zero) {
            Ttf.DestroyText(_sText);
        }
        if (_textEngine.Handle != nint.Zero) {
            Ttf.DestroyRendererTextEngine(_textEngine);
        }
    }

    public TextString(Font font, float size, string text) {
        RendererPtr = GetRenderer();

        if (font.Handle == nint.Zero) {
            _log.Error($"Font is not loaded: {Sdl.GetError()}");
            return;
        }

        _textEngine = Ttf.CreateRendererTextEngine(RendererPtr);
        _sText = Ttf.CreateText(_textEngine, Font, text);
        _sText.TextStr = text;

        Text = text;
        font.Size = size;
        Font = font;
        FSize oSize = MeasureString(Font, text);
        Width = oSize.Width;
        Height = oSize.Height;
        uint props = Ttf.GetTextProperties(_sText.Handle);
        _log.Info($"Initialized new TextString({text}) using Font {font.Name}-{font.Size} with Dimensions({X}, {Y}, {Width}, {Height}) and Text Properties {props}.");
    }

    public Color ForegroundColor {
        get => Ttf.GetTextColor(_sText);
        set {
            Ttf.SetTextColor(_sText, value);
        }
    }

    public Color BackgroundColor { get; set; }

    public Color ShadowColor { get; set; }

    public string Text {
        get => _sText.TextStr;
        set {
            _sText.TextStr = value;
            FSize size = MeasureString(Font, value);
            Width = size.Width;
            Height = size.Height;
        }
    }

    public float FontSize {
        get {
            if (Font.Handle == nint.Zero) {
                Font = GetFontStatic();
            }
            return Font.Size;
        }
        set {
            // This I believe copies the font while preserving the handle.
            Font f = Font;
            f.Size = value;
            Font = f;
        }
    }

    public bool ShowShadow { get; set; }

    public int Length => Text?.Length ?? 0;

    public override void Initialize() {
    }

    public static implicit operator string(TextString text) {
        return text.Text ?? string.Empty;
    }

    private Color CalculateShadow(float x, float y) {
        var c = ShadowColor;
        c.A = (byte)(255 - (x + y) * 2);
        return c;
    }

    public void SetPosition(float x, float y) {
        X = x;
        Y = y;
    }

    public void SetPosition(FPoint point) {
        SetPosition(point.X, point.Y);
    }

    public void CenterText() {
        CenterText(Width, Height);
    }

    public void CenterText(float width, float height) {
        CenterText(X, Y, width, height);
    }

    public void CenterText(float x, float y, float w, float h) {
        if (Font.Handle == nint.Zero) {
            Font = GetFontStatic();
        }
        FSize size = MeasureString(Font, Text);
        X = x + w / 2 - size.Width / 2;
        Y = y + h / 2 - size.Height / 2;
        Ttf.SetTextPosition(_sText, (int)X, (int)Y);
    }

    public override void Draw() {
        if (ShowShadow) {
            RenderText(Text, Font.Name, (int)(X + Font.Size / ShadowDivisor),
                (int)(Y + Font.Size / ShadowDivisor),
                CalculateShadow(Font.Size / ShadowDivisor, Font.Size / ShadowDivisor));
        }
        Ttf.DrawRendererText(_sText, X, Y);
        Sdl.SetRenderDrawColor(RendererPtr, Colors.Colors.Red);
        Sdl.RenderRect(RendererPtr, ref frect);
    }

    public override void Update(Event e) {
        FSize size = MeasureString(Font, Text);
        frect = new FRect {
            X = X,
            Y = Y,
            W = size.Width,
            H = size.Height
        };
    }
}