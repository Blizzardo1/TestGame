using SDL2;
using TestGame.Colors;

namespace TestGame.GameObjects;

internal class Clock : GameObject {
    private const float Speed = 1.5f;
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

    public Clock(GameContext context, string font = @"C:\Windows\Fonts\Arial.ttf", bool animate = true) {
        Initialize(context.RendererPtr, font, "clock", 72);
        Name = "object/clock";
        frect = new FRect { X = context.Rect.X, Y = context.Rect.Y, W = context.Rect.W, H = context.Rect.H };
        ( Width, Height ) = MeasureString(GetFont("clock", 72), DateTime.Now.ToString("HH:mm:ss"));
        ForegroundColor = Core.GetRandomColor();

        _ogForeColor = ForegroundColor;
        _colorSequence = _hitSequence;
        _animate = animate;
        _xSpeed = animate ? Speed : 0;
        _ySpeed = animate ? Speed : 0;
    }

    private Color CalculateShadow(float x, float y)
    {
        var c = KnownColor.Black.ToColor();
        c.A = (byte)(255 - ( x + y ) * 2);
        return c;
    }
    
    // private float tXSpeed = Speed * 1.15f;
    // private float tYSpeed = Speed * 1.15f;
    private float tX = 6;
    private float tY = 6;

    /// <inheritdoc />
    public override void Draw() {
        // _ = SDL.SetRenderDrawColor(RendererPtr, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);
        // _ = SDL.RenderFillRectF(RendererPtr, ref _rect);

        if (_bounce && _animate) {
            _hitFrames--;
            if (_hitFrames <= 0) {
                _hitFrames = HitFrames;
                _bounce = false;
                ForegroundColor = _ogForeColor;
            }
            else {
                //Console.WriteLine(_hitFrames % _colorSequence.Length);
                ForegroundColor = _colorSequence[ _hitFrames % _colorSequence.Length ];
            }
        }

        RenderText(_time, (int)( X + tX ), (int)( Y + tY ), CalculateShadow(tX, tY));
        RenderText(_time, (int)X, (int)Y, ForegroundColor);
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        _ = SDL.GetRendererOutputSize(RendererPtr, out int rx, out int by);
        _time = DateTime.Now.ToString("HH:mm:ss");
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

        if (X < 0 || X > rx - Width) {
            _xSpeed = -_xSpeed;
            _bounce = true;
        }

        if (Y < 0 || Y > by - Height) {
            _ySpeed = -_ySpeed;
            _bounce = true;
        }

        X += _xSpeed;
        Y += _ySpeed;
    }
}