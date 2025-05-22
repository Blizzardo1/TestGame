

using SharpSDL3;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
using TestGame.Colors;

namespace TestGame.GameObjects; 
public class TextString : GameObject {
    private const int ShadowDivisor = 10;
    private readonly GameContext _context;

    public TextString(GameContext context, Font font, float size, string text) {
        _context = context;
        RendererPtr = _context.RendererPtr;
        Text = text;
        font.Size = size;
        Font = font;
    }

    public Color ForegroundColor { get; set; }

    public Color BackgroundColor { get; set; }

    public Color ShadowColor { get; set; }

    public float TextSize {
        get {
            if (Font.Handle == nint.Zero) {
                Font = GetFontStatic();
            }
            return Font.Size;
        }
        set {
            Font f = Font;
            f.Size = value;
            Font = f;
        }
    }

    public bool ShowShadow { get; set; }

    public int Length => Text?.Length ?? 0;

    public string Text { get; set; }

    public override void Initialize() {
        RendererPtr = _context.RendererPtr;
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
        X = point.X;
        Y = point.Y;
    }

    public void CenterText(float width, float height) {
        if (Font.Handle == nint.Zero) {
            Font = GetFontStatic();
        }
        FSize size = MeasureString(Font, Text);
        X = width / 2 - size.Width / 2;
        Y = height / 2 - size.Height / 2;
    }

    public override void Draw() {
        if (ShowShadow) {
            RenderText(Text, Font.Name, (int)(X + Font.Size / ShadowDivisor),
                (int)(Y + Font.Size / ShadowDivisor),
                CalculateShadow(Font.Size / ShadowDivisor, Font.Size / ShadowDivisor));
        }
        RenderText(Text, Font.Name, (int)X, (int)Y, ForegroundColor);
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