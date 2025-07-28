
using SharpSDL3;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
using TestGame.Colors;

namespace TestGame.GameObjects;

public class Clock(GameContext context, string font = @"default.ttf", bool animate = true, bool rotate = false,
    int fontSize = 72) : GameObject {
    private const float Speed = 1.5f;
    private const string TimeFormat = "HH:mm:ss";

    private float _xSpeed;
    private float _ySpeed;
    private readonly bool _animate = animate;
    private bool _rotate = rotate;

    /// <summary>
    /// The foreground color of the clock
    /// </summary>
    public Color ForegroundColor { get; set; }

    /// <summary>
    /// The Shadow color of the clock
    /// </summary>
    public Color ShadowColor { get; set; } = KnownColor.Black.ToColor();

    /// <summary>
    /// The background color of the Clock
    /// </summary>
    public Color BackgroundColor { get; set; } = KnownColor.Black.ToColor();

    public bool ShowShadow { get; set; }

    private readonly TextString _time = new(OpenFont(context.FontName), fontSize, "00:00:00");

    private readonly Color[] _hitSequence = [
        new() { R = 88, G = 120, B = 56, A = 255 }, // Moldy Green
            new() { R = 192, G = 24, B = 32, A = 255 }, // Firebrick
            new() { R = 120, G = 144, B = 248, A = 255 }, // Ice
            new() { R = 248, G = 112, B = 48, A = 255 }, // Orange Creme
            new() { R = 80, G = 112, B = 200, A = 255 } // Royale
    ];

    private const int HitFrames = 30;
    private int _hitFrames = HitFrames;
    private bool _bounce;
    private Color _ogForeColor;

    public override void Initialize() {
        RendererPtr = context.RendererPtr;
        Name = "object/clock";
        frect = new FRect {
            X = context.Rect.X,
            Y = context.Rect.Y,
            W = context.Width,
            H = context.Height
        };
        Font f = OpenFont();
        f.Size = fontSize;
        Font = f;

        Size size = Font.GetTextSize(System.DateTime.Now.ToString(TimeFormat));

        _time.Text = System.DateTime.Now.ToString(TimeFormat);

        frect = new() {
            X = _time.X,
            Y = _time.Y,
            W = _time.Width,
            H = _time.Height
        };

        Width = size.Width;
        Height = size.Height;
        ForegroundColor = Core.GetRandomColor(false, Colors.Colors.Black);
        _ogForeColor = ForegroundColor;
        _xSpeed = _animate ? Speed : 0;
        _ySpeed = _animate ? Speed : 0;
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
            } else {
                // "NPC" being "attacked"
                ForegroundColor = _hitSequence[_hitFrames % _hitSequence.Length];
            }
        }

        if (_rotate) {
            _time.DrawRotated(-35);
        } else {
            _time.Draw();
        }
        Core.SetRenderColor(RendererPtr, KnownColor.Red.ToColor());
        if (Core.IsDebugging) {
            _ = Sdl.RenderRect(RendererPtr, ref frect);
        }
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        // rx - right-most x or width
        // by - bottom-most y or height
        _ = Sdl.GetRenderOutputSize(RendererPtr, out int rx, out int by);
        _time.Update(e);
        _time.ForegroundColor = ForegroundColor;
        _time.BackgroundColor = BackgroundColor;
        _time.ShadowColor = ShadowColor;
        _time.ShowShadow = ShowShadow;
        _time.Text = System.DateTime.Now.ToString(TimeFormat);
        Size size = Font.GetTextSize(System.DateTime.Now.ToString(TimeFormat));

        Width = size.Width;
        Height = size.Height;

        if (!_animate) {
            return;
        }
        
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
