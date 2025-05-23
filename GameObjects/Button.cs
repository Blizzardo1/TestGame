using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
using TestGame.Colors;
using TestGame.GameObjects.Textures;

namespace TestGame.GameObjects;

/// <summary>
/// Creates a new Button
/// </summary>
/// <param name="context">A <see cref="GameContext"/> record containing information about what to do with <see cref="Button"/></param>
public class Button(GameContext context) : GameObject {
    public event EventHandler< MouseButtonEvent >? Click;
    public event EventHandler< MouseButtonEvent >? RightClick;
    public event EventHandler< MouseButtonEvent >? DoubleClick;
    public event EventHandler< MouseButtonEvent >? RightDoubleClick;
    public event EventHandler< MouseMotionEvent >? MouseEnter;
    public event EventHandler< MouseMotionEvent >? MouseLeave;

    private const float DefaultFontSize = 12f;
    private readonly TextString _text = new(GetFontStatic(context.FontName), DefaultFontSize, "Button");

    private bool _inverse;
    
    public ButtonState State { get; set; }

    public bool Flat { get; set; }

    private FRect sfRect;
    public bool ShadowEnabled { get; set; }
    public int ShadowDepth { get; set; }
    public float ShadowOrientation { get; set; }
    public Color ShadowColor { get; set; }

    public Sprite32? Image { get; set; }

    public string Text
    {
        get => _text.Text ?? "";
        set
        {
            if(_text.FontSize <= 0.01) {
                _text.FontSize = DefaultFontSize;
            }
            if (Font.Handle == nint.Zero) {
                Font f = GetFontStatic();
                f.Size = _text.FontSize;
                Font = f;
            }

            _text.Text = value;
            _text.CenterText();
        }
    }

    private Color _selectedColor;

    private ButtonState _previousState;

    public Color BackgroundColor { get; set; } = new() { R = 240, G = 240, B = 240, A = 255 };
    public Color HighlightColor { get; set; } = new() { R = 64, G = 150, B = 255, A = 255 };
    public Color ForegroundColor {
        get => _text.ForegroundColor;
        set => _text.ForegroundColor = value;
    }
    public Color DisabledColor { get; set; } = new() { R = 100, G = 100, B = 100, A = 255 };
    public Color ClickedColor { get; set; } = new() { R = 32, G = 75, B = 128, A = 255 };

    public override void Initialize() {
        Initialize(context.RendererPtr);
        Name = "Button";
        // #TODO: This is a horrible hack and exists elsewhere around the code. I need to fix this.
        // Essentially just use FRect unless you need to use Rect.
        frect = new FRect { X = context.Rect.X, Y = context.Rect.Y, W = context.Width, H = context.Height };
        Width = (int)context.Width;
        Height = (int)context.Height;
        _inverse = false;
        // Cheap hacky way to preserve original background color
        _selectedColor = BackgroundColor;
        _previousState = ButtonState.Default;
        State = _previousState;
        ForegroundColor = ColorConverter.SetInverseBasedOn(_selectedColor);
        _text.FontSize = DefaultFontSize;
        _text.CenterText(X, Y, Width, Height);
    }

    public virtual void OnClick(object? sender, MouseButtonEvent e) {
        if (!frect.Intersects(e.X, e.Y))
            return;

        _previousState = State;
        State = ButtonState.Clicked;

        switch (e.Button) {
            case (int)MouseButton.Left:
                Click?.Invoke(sender, e);
                break;
            case (int)MouseButton.Right:
                RightClick?.Invoke(sender, e);
                break;
            default:
                break;
        }
    }

    public virtual void OnDoubleClick(object? sender, MouseButtonEvent e) {
        if (!frect.Intersects(e.X, e.Y))
            return;

        _previousState = State;
        State = ButtonState.Clicked;

        switch (e.Button) {
            case (int)MouseButton.Left:
                DoubleClick?.Invoke(sender, e);
                break;
            case (int)MouseButton.Right:
                RightDoubleClick?.Invoke(sender, e);
                break;
        }
    }

    public virtual void OnMouseEnter(object? sender, MouseMotionEvent e) {
        if (State == ButtonState.Disabled)
            return;

        _previousState = State;
        State = ButtonState.Highlighted;
        _selectedColor = HighlightColor;

        MouseEnter?.Invoke(sender, e);
    }

    public virtual void OnMouseLeave(object? sender, MouseMotionEvent e) {
        if (State == ButtonState.Disabled) return;
        _previousState = State;
        State = ButtonState.Default;
        _selectedColor = BackgroundColor;
        MouseLeave?.Invoke(sender, e);
    }

    #region Implementation of IRenderable

    /// <inheritdoc />
    public override void Draw() {
        // If shadow is enabled, draw the shadow
        // DUH! kekw
        if (ShadowEnabled) {
            _ = Sdl.SetRenderDrawColor(RendererPtr,
                ShadowColor.R,
                ShadowColor.G,
                ShadowColor.B,
                ShadowColor.A
            );
            _ = Sdl.RenderFillRect(RendererPtr, ref sfRect);
        }

        _ = Sdl.SetRenderDrawColor(
            RendererPtr,
            _selectedColor.R,
            _selectedColor.G,
            _selectedColor.B,
            _selectedColor.A
        );

        // If shadow is enabled and our depth is more than 0
        if (ShadowEnabled && ShadowDepth > 0 && State == ButtonState.Clicked) {
            // We "move" the button to the depth of the shadow.
            _ = Sdl.RenderFillRect(RendererPtr, ref sfRect);
            if (Image is not null) {
                _ = Sdl.RenderTexture(RendererPtr, Image.TexturePtr, ref sfRect, ref sfRect);
            }

            _text.Draw();

            // We don't need to render the rest of the button.
            return;
        }

        // Draw Untouched Square
        _ = Sdl.RenderFillRect(RendererPtr, ref frect);
        _ = Sdl.SetRenderDrawColor(RendererPtr, 255, 255, 255, 16);
        if (!Flat) {
            if (_inverse) {
                _ = Sdl.RenderLine(
                    RendererPtr, X + Width, Y + Height, X, Y + Height);
                _ = Sdl.RenderLine(
                    RendererPtr, X + Width, Y, X + Width, Y + Height);
            }
            else {
                _ = Sdl.RenderLine(
                    RendererPtr, X, Y, X + Width, Y);
                _ = Sdl.RenderLine(
                    RendererPtr, X, Y, X, Y + Height);
            }
        }

        if (Image is not null) {
            _ = Sdl.RenderTexture(RendererPtr, Image.TexturePtr, ref frect, ref frect);
        }
        _text.Draw();
    }

    private bool InRange(MouseMotionEvent mme) {
        // Check if the mouse is within the bounds of the button
        return mme.X >= X
               && mme.X <= X + Width
               && mme.Y >= Y
               && mme.Y <= Y + Height;
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        sfRect = frect with { X = X + ShadowDepth, Y = Y + ShadowDepth };

        _selectedColor = State switch {
            ButtonState.Disabled => DisabledColor,
            ButtonState.Default => BackgroundColor,
            ButtonState.Highlighted => HighlightColor,
            ButtonState.Clicked => ClickedColor,
            _ => throw new ArgumentOutOfRangeException("ButtonState out of range!", new Exception())
        };

        _text.CenterText(X, Y, Width, Height);

        ForegroundColor = ColorConverter.SetInverseBasedOn(_selectedColor);

        if (e.Button is { Type: EventType.MouseButtonUp }) {
            State = _previousState;
            return;
        }

        if (e.Button is { Clicks: 2, Type: EventType.MouseButtonDown }
            && InRange(e.Motion)) {
            OnDoubleClick(this, e.Button);
            return;
        }

        if (e.Button is { Clicks: >= 1, Type: EventType.MouseButtonDown }
            && InRange(e.Motion)) {
            OnClick(this, e.Button);
            return;
        }

        if (e.Type is EventType.MouseMotion) {
            if (InRange(e.Motion)) {
                OnMouseEnter(this, e.Motion);
            }
            else {
                OnMouseLeave(this, e.Motion);
            }
        }
    }

    #endregion
}