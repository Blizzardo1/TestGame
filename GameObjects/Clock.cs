using SDL2;
using TestGame.Colors;

namespace TestGame.GameObjects;

internal class Clock : GameObject {
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

    private readonly Color[] _colorSequence;

    private const int HitFrames = 30;
    private int _hitFrames = HitFrames;
    private bool _bounce;
    private Color _ogForeColor;

    Rect _rect;

    public Clock(GameContext context, string font = @"C:\Windows\Fonts\Arial.ttf", bool animate = true,
        int fontSize = 72) {
        Initialize(context.RendererPtr, font, "clock", FontSize);
        Name = "object/clock";
        frect = new FRect {
            X = context.Rect.X,
            Y = context.Rect.Y,
            W = context.Rect.W,
            H = context.Rect.H
        };
        ( Width, Height ) = MeasureString(GetFont("clock", FontSize), DateTime.Now.ToString("HH:mm:ss"));
        ForegroundColor = Core.GetRandomColor();
        FontSize = fontSize;
        _ogForeColor = ForegroundColor;
        _colorSequence = _hitSequence;
        _animate = animate;
        _xSpeed = animate ? Speed : 0;
        _ySpeed = animate ? Speed : 0;
        _rect = frect.ToRect();
    }

    private Color CalculateShadow(float x, float y) {
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
                ForegroundColor = _colorSequence[ _hitFrames % _colorSequence.Length ];
            }
        }

        RenderText(_time, FontSize, "clock", (int)( X + FontSize / ShadowDivisor ),
            (int)( Y + FontSize / ShadowDivisor ),
            CalculateShadow(FontSize / ShadowDivisor, FontSize / ShadowDivisor));
        RenderText(_time, FontSize, "clock", (int)X, (int)Y, ForegroundColor);

        Core.SetRenderColor(RendererPtr, KnownColor.Red.ToColor());
        _ = SDL.RenderDrawRect(RendererPtr, ref _rect);
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        // rx - right-most x or width
        // by - bottom-most y or height
        _ = SDL.GetRendererOutputSize(RendererPtr, out int rx, out int by);
        _time = DateTime.Now.ToString("HH:mm:ss");
        ( Width, Height ) = MeasureString(GetFont("clock", FontSize), DateTime.Now.ToString("HH:mm:ss"));
        _rect = frect.ToRect();
        _rect.X += 8;
        _rect.Y += 10;
        _rect.W -= 8;
        _rect.H -= 14;
        Width = _rect.W;
        Height = _rect.H;

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