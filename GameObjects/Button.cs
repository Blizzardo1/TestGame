using SDL2;

namespace TestGame.GameObjects;

public class Button : Renderer, IGameObject {
    public event SDL2.EventHandler< MouseButtonEvent >? Click;
    public event SDL2.EventHandler< MouseButtonEvent >? RightClick;
    public event SDL2.EventHandler< MouseMotionEvent >? MouseEnter;
    public event SDL2.EventHandler< MouseMotionEvent >? MouseLeave;

    private FRect _rect;
    private bool _inverse;

    public ButtonState State { get; set; }

    public bool Flat { get; set; }

    public float X
    {
        get => _rect.X;
        set => _rect.X = value;
    }

    public float Y
    {
        get => _rect.Y;
        set => _rect.Y = value;
    }

    public int Width
    {
        get => (int)_rect.W;
        set => _rect.W = value;
    }

    public int Height
    {
        get => (int)_rect.H;
        set => _rect.H = value;
    }

    /// <inheritdoc />
    public string Name { get; set; } = "Button";

    public string Text { get; set; } = "Text";

    public Point TextPosition { get; set; } = new() { X = 10, Y = 6 };

    private Color _selectedColor;

    public Color BackgroundColor { get; set; } = new() { R = 240, G = 240, B = 240, A = 255 };
    public Color HighlightColor { get; set; } = new() { R = 64, G = 150, B = 255, A = 255 };
    public Color ForegroundColor { get; set; } = new() { R = 255, G = 255, B = 255, A = 255 }; // White
    public Color DisabledColor { get; set; } = new() { R = 100, G = 100, B = 100, A = 255 };
    public Color ClickedColor { get; set; } = new() { R = 32, G = 75, B = 128, A = 255 };

    /// <summary>
    /// Creates a new Button
    /// </summary>
    /// <param name="rendererPtr">A pointer to a rendererPtr</param>
    public Button(nint rendererPtr) {
        Initialize(rendererPtr);
        _rect = new FRect { X = 0, Y = 0, W = 75, H = 23 };
        _inverse = false;
        // Cheap hacky way to preserve original background color
        _selectedColor = BackgroundColor;
        ForegroundColor = ForegroundColor.SetInverseBasedOn(_selectedColor);
    }

    public virtual void OnClick(object? sender, MouseButtonEvent e) {
        switch (e.Button) {
            case (int)MouseButton.Left:
                if (!_rect.Intersects(e.X, e.Y)) break;
                State = ButtonState.Clicked;
                Click?.Invoke(sender, e);
                break;
            case (int)MouseButton.Right:
                if (!_rect.Intersects(e.X, e.Y)) break;
                RightClick?.Invoke(sender, e);
                break;
        }
    }

    public virtual void OnMouseEnter(object? sender, MouseMotionEvent e) {
        if (State == ButtonState.Disabled) return;
        State = ButtonState.Highlighted;
        MouseEnter?.Invoke(sender, e);
    }

    public virtual void OnMouseLeave(object? sender, MouseMotionEvent e) {
        if (State == ButtonState.Disabled) return;
        State = ButtonState.Default;
        MouseLeave?.Invoke(sender, e);
    }

    #region Implementation of IGameObject

    /// <inheritdoc />
    public virtual void Draw() {
        _ = SDL.SetRenderDrawColor(
            RendererPtr,
            _selectedColor.R,
            _selectedColor.G,
            _selectedColor.B,
            _selectedColor.A
        );

        // Draw Untouched Square
        _ = SDL.RenderFillRectF(RendererPtr, ref _rect);
        _ = SDL.SetRenderDrawColor(RendererPtr, 255, 255, 255, 16);
        if (!Flat) {
            if (_inverse) {
                _ = SDL.RenderDrawLineF(
                    RendererPtr, X + Width, Y + Height, X, Y + Height);
                _ = SDL.RenderDrawLineF(
                    RendererPtr, X + Width, Y, X + Width, Y + Height);
            }
            else {
                _ = SDL.RenderDrawLineF(
                    RendererPtr, X, Y, X + Width, Y);
                _ = SDL.RenderDrawLineF(
                    RendererPtr, X, Y, X, Y + Height);
            }
        }

        // Draw Number
        RenderText(Text, (int)X + TextPosition.X, (int)Y + TextPosition.Y, ForegroundColor);
    }

    /// <inheritdoc />
    public virtual void Update(Event e) {
        if (e.Button is { Clicks: 1, Type: EventType.MouseButtonDown }) {
            OnClick(this, e.Button);
        }

        if (e.Type is EventType.MouseMotion) {
            if (e.Motion.X >= X && e.Motion.X <= X + Width && e.Motion.Y >= Y && e.Motion.Y <= Y + Height) {
                OnMouseEnter(this, e.Motion);
                _selectedColor = HighlightColor;
            }
            else {
                OnMouseLeave(this, e.Motion);
                _selectedColor = BackgroundColor;
            }
        }

        // It's Untouched, Highlight or not
        _selectedColor = State switch {
            ButtonState.Disabled => DisabledColor,
            ButtonState.Default => BackgroundColor,
            ButtonState.Highlighted => HighlightColor,
            ButtonState.Clicked => ClickedColor,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion
}