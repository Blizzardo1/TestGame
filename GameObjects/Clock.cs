using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using TestGame.Colors;
using TestGame.GameObjects.Textures;

namespace TestGame.GameObjects; 

internal class Clock : Renderer, IGameObject
{
    private const float Speed = 1.5f;
    private float _xSpeed = Speed;
    private float _ySpeed = Speed;
    private FRect _rect;

    #region Implementation of IGameObject

    /// <inheritdoc />
    public float X
    {
        get => _rect.X;
        set => _rect.X = value;
    }

    /// <inheritdoc />
    public float Y
    {
        get => _rect.Y;
        set => _rect.Y = value;
    }

    /// <inheritdoc />
    public int Width
    {
        get => (int)_rect.W;
        set => _rect.W = value;
    }

    /// <inheritdoc />
    public int Height
    {
        get => (int)_rect.H;
        set => _rect.H = value;
    }

    /// <summary>
    /// The foreground color of the clock
    /// </summary>
    public Color ForegroundColor { get; set; } = KnownColor.White.ToColor();

    /// <summary>
    /// The background color of the Clock
    /// </summary>
    public Color BackgroundColor { get; set; } = KnownColor.Black.ToColor();


    /// <inheritdoc />
    public string Name => "Clock";

    private readonly Color[] _hitSequence = {
        new() { R = 88, G = 120, B = 56, A = 255 },     // Moldy Green
        new() { R = 192, G = 24, B = 32, A = 255 },     // Firebrick
        new() { R = 120, G = 144, B = 248, A = 255 },   // Ice
        new() { R = 248, G = 112, B = 48, A = 255 },    // Orange Creme
        new() { R = 80, G = 112, B = 200, A = 255 }     // Royale
    };

    private readonly Color[] _colorSequence;

    private const int HitFrames = 30;
    private int _hitFrames = HitFrames;
    private bool _bounce;
    private Color _ogForeColor;
    
    public Clock(nint rendererPtr, string font = @"C:\Windows\Fonts\Arial.ttf", int x = 0, int y = 0)
    {
        Initialize(rendererPtr, font, 72);
        _rect = new FRect { X = x, Y = y, W = 0, H = 0 };
        (Width, Height) = MeasureString(DateTime.Now.ToString("HH:mm:ss"));
        ForegroundColor = Game.GetRandomColor();
        
        _ogForeColor = ForegroundColor;
        _colorSequence = _hitSequence;
    }

    /// <inheritdoc />
    public void Draw()
    {
        // _ = SDL.SetRenderDrawColor(RendererPtr, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);
        // _ = SDL.RenderFillRectF(RendererPtr, ref _rect);

        if (_bounce)
        {
            _hitFrames--;
            if (_hitFrames <= 0)
            {
                _hitFrames = HitFrames;
                _bounce = false;
                ForegroundColor = _ogForeColor;
            }
            else
            {
                Console.WriteLine(_hitFrames % _colorSequence.Length);
                ForegroundColor = _colorSequence[_hitFrames % _colorSequence.Length];
            }
        }

        RenderText(DateTime.Now.ToString("HH:mm:ss"), (int)X, (int)Y, ForegroundColor);

    }

    /// <inheritdoc />
    public void Update(Event e) {
        _ = SDL.GetRendererOutputSize(RendererPtr, out int rx, out int by);

        if (
            X <= 0 && Y <= 0 // Top left
            || X >= rx - Width && Y <= 0 // Top right
            || X <= 0 && Y >= by - Height // Bottom left
            || X >= rx - Width && Y >= by - Height // Bottom right
        )
        {
            // Change the Color
            ForegroundColor = Game.GetRandomColor();
            _ogForeColor = ForegroundColor;
        }

        if (X < 0 || X > rx - Width)
        {
            _xSpeed = -_xSpeed;
            _bounce = true;
        }
        if (Y < 0 || Y > by - Height)
        {
            _ySpeed = -_ySpeed;
            _bounce = true;
        }

        X += _xSpeed;
        Y += _ySpeed;
    }

    #endregion
}