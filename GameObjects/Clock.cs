
using SharpSDL3;
using SharpSDL3.Structs;
using TestGame.Colors;

namespace TestGame.GameObjects;

public class Clock(GameContext context, string font = @"default.ttf", bool animate = true,
    int fontSize = 72) : GameObject {
    private const float Speed = 1.5f;
    private const int ShadowDivisor = 10;

    private float _xSpeed;
    private float _ySpeed;
    private bool _animate;

    /// <summary>
    /// The foreground color of the clock
    /// </summary>
    public Color ForegroundColor { get; set; }

    /// <summary>
    /// The background color of the Clock
    /// </summary>
    public Color BackgroundColor { get; set; } = KnownColor.Black.ToColor();

    public int FontSize { get; set; }

    private string _time = "";

    private readonly Color[] _hitSequence = {
        new() { R = 88, G = 120, B = 56, A = 255 }, // Moldy Green
        new() { R = 192, G = 24, B = 32, A = 255 }, // Firebrick
        new() { R = 120, G = 144, B = 248, A = 255 }, // Ice
        new() { R = 248, G = 112, B = 48, A = 255 }, // Orange Creme
        new() { R = 80, G = 112, B = 200, A = 255 } // Royale
    };

    private const int HitFrames = 30;
    private const string FontName = "clock";
    private int _hitFrames = HitFrames;
    private bool _bounce;
    private Color _ogForeColor;

    public override void Initialize() {
        Initialize(context.WindowPtr, context.RendererPtr, font, FontName, FontSize);
        Name = "object/clock";
        frect = new FRect {
            X = context.Rect.X,
            Y = context.Rect.Y,
            W = context.Width,
            H = context.Height
        };
        FSize size = MeasureString(RendererPtr, GetFont(FontName, FontSize), System.DateTime.Now.ToString("HH:mm:ss"));
        Width = size.Width;
        Height = size.Height;
        ForegroundColor = Core.GetRandomColor(false, Colors.Colors.Black);
        FontSize = fontSize;
        _ogForeColor = ForegroundColor;
        _animate = animate;
        _xSpeed = animate ? Speed : 0;
        _ySpeed = animate ? Speed : 0;
    }

    private static Color CalculateShadow(float x, float y) {
        var c = KnownColor.Black.ToColor();
        c.A = (byte)( 255 - ( x + y ) * 2 );
        return c;
    }

    /// <inheritdoc />
    public override void Draw() {
        // If the object has bounced from an edge, and we're animating,
        // then let's show off some flashing.
        if (_bounce && _animate) {
            _hitFrames++;
            if (_hitFrames >= HitFrames) {
                _hitFrames = 0;
                _bounce = false;
                ForegroundColor = _ogForeColor;
            }
            else {
                // "NPC" being "attacked"
                ForegroundColor = _hitSequence[ _hitFrames % _hitSequence.Length ];
            }
        }

        RenderText(_time, FontSize, FontName, (int)( X + (float) FontSize / ShadowDivisor ),
            (int)( Y + FontSize / ShadowDivisor ),
            CalculateShadow(FontSize / ShadowDivisor, (float)FontSize / ShadowDivisor));
        RenderText(_time, FontSize, FontName, (int)X, (int)Y, ForegroundColor);

        Core.SetRenderColor(RendererPtr, KnownColor.Red.ToColor());
        if (Core.IsDebugging) {
            _ = Render.RenderRect(RendererPtr, ref frect);
        }
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        // rx - right-most x or width
        // by - bottom-most y or height
        _ = Render.GetRenderOutputSize(RendererPtr, out int rx, out int by);
        _time = System.DateTime.Now.ToString("HH:mm:ss");
        FSize size = MeasureString(RendererPtr, GetFont(FontName, FontSize), System.DateTime.Now.ToString("HH:mm:ss"));
        Width = size.Width;
        Height = size.Height;
        frect.X += 8;
        frect.Y += 10;
        frect.W -= 8;
        frect.H -= 14;
        Width = frect.W;
        Height = frect.H;

        if (
            X <= 0 && Y <= 0 // Top left
            || X >= rx - Width && Y <= 0 // Top right
            || X <= 0 && Y >= by - Height // Bottom left
            || X >= rx - Width && Y >= by - Height // Bottom right
        ) {
            // Change the Color
            ForegroundColor = Core.GetRandomColor();
            _ogForeColor = ForegroundColor;
        }

        // X refers to the current location and Width is the Object Width
        if (X < 0 || X > rx - Width) {
            _xSpeed = -_xSpeed;
            _bounce = true;
        }

        // Y refers to the current location and Height is the Object Height
        if (Y < 0 || Y > by - Height) {
            _ySpeed = -_ySpeed;
            _bounce = true;
        }

        X += _xSpeed;
        Y += _ySpeed;
    }
}