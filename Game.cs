#region Header

// SDL2 >SDL2 >Game.cs\n Copyright (C) , 2023\nCreated 24 11, 2023

#endregion

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TestGame;
using TestGame.Colors;
using TestGame.GameObjects;
using TestGame.Win32Menu;

namespace SDL2; 

public delegate void EventHandler(object? sender, Event e);

public delegate void EventHandler<in T>(object? sender, T e);

public delegate void MouseMotionEventHandler(object? sender, MouseMotionEvent e);

public delegate void MouseButtonEventHandler(object? sender, MouseButtonEvent e);

internal class Game : Window {
        
    private const int LocationX = 100;
    private const int LocationY = 100;

    public static uint WindowId { get; private set; }
    public static Random Random { get; } = new();


    private Menu? _menu;
    private List<IGameObject>? _gameObjects;

    private bool _dead;



    private string? _motionMessage;
    private bool _debug;

    /// <summary>
    /// Creates a new Game
    /// </summary>
    /// <param name="width">The Width</param>
    /// <param name="height">The Height</param>
    /// <param name="title">The Title</param>
    public Game(int width, int height, string title)
        : base(
            title,
            new Point { X = 0x7FFFFFFF, Y = 0x7FFFFFFF },
            new Size(width, height),
            WindowFlags.AllowHighdpi | WindowFlags.Resizable | WindowFlags.Shown)
    {
        WindowId = SDL.GetWindowID(WindowPtr);
        Width = width;
        Height = height;

        InitializeComponents();
        _dead = false;
        IsRunning = true;

    }

    /// <summary>
    /// Creates a Random <see cref="Color"/>
    /// </summary>
    /// <param name="transparent">Allow Transparency</param>
    /// <returns>A completely random <see cref="Color"/></returns>
    public static Color GetRandomColor(bool transparent = false)
    {
        byte[] bytes = new byte[4];
        Random.NextBytes(bytes);

        Color c = new()
        {
            R = bytes[0],
            G = bytes[1],
            B = bytes[2],
            A = transparent ? bytes[3] : (byte)255
        };

        return c;
    }

    /// <summary>
    /// Initializes all components of the Game Window
    /// </summary>
    /// <exception cref="Exception">Failure to create the Window and Renderer</exception>
    private void InitializeComponents()
    {
        AttachListeners();

        _gameObjects = new List< IGameObject >();
        /*
        for (int y = 0; y < Height; y += 64)
        {
            for (int x = 0; x < Width; x += 64) {
                _gameObjects.Add(new Water(RendererPtr) { X = x, Y = y, Width = 64, Height = 64});
            }
        }*/

        _gameObjects.Add(new Clock(RendererPtr));

        _text = "";
        _textSize = new Size();
            
        //CreateMenu();

        // _gameObjects.Add(new Simple(RendererPtr));
        // _menu = new Menu(RendererPtr);
        // _menu.AddMenuItem("File", () => { },  new MenuItem(RendererPtr) { Text = "Do Something" } , new MenuItem(RendererPtr) { Text = "Exit", Action = () => IsRunning = false });
        // _menu.AddMenuItem("Edit", () => { });
        // _menu.AddMenuItem("Help", () => { });
        // _menu.AddMenuItem("Debug", () => { _debug = !_debug; });

        // The worst way to gather Debug Diagnostics.
        // TODO: Create a class for Diagnostics
        var sb = new StringBuilder();
        sb.AppendLine($"RAM: {SDL.GetSystemRAM()} MB");
        sb.AppendLine($"CPU Count: {SDL.GetCPUCount()}");
        sb.AppendLine($"Game Objects: {_gameObjects.Count}"); // Eventually, we want to change this.
        _motionMessage = sb.ToString();
    }

    private void CreateMenu()
    {
        _menu = new Menu(RendererPtr);
        _menu.AddMenuItem("File", 0, () => { });
        _menu.AddMenuItem("Edit", 1, () => { });
        _menu.AddMenuItem("Help", 2, () => { });
    }

    private void AttachListeners()
    {
        FirstEvent += OnFirstEvent;
        Quit += OnQuit;
        KeyDown += OnKeyDown;
        MouseMove += OnMouseMove;
        WindowEvent += OnWindowEvent;
    }

    private void OnKeyDown(object? sender, KeyboardEvent e)
    {
        switch (e.Keysym.Sym)
        {
            case Keycode.F3:
                _debug = !_debug;
                break;
            case Keycode.Escape:
                IsRunning = !IsRunning;
                // Pausa :D
                // Le Pause
                break;
            case Keycode.r:
                _dead = false;
                break;
            case Keycode.d:
                _dead = false;
                break;
            case Keycode.c:
                _dead = false;
                break;
        }
    }

    private void OnWindowEvent(object? sender, WindowEvent e)
    {
        try
        {
            switch (e.Event)
            {
                case WindowEventID.None: break;
                case WindowEventID.Shown: break;
                case WindowEventID.Hidden: break;
                case WindowEventID.Exposed: break;
                case WindowEventID.Moved:
                    UpdatePosition(WindowPtr);
                    break;
                case WindowEventID.Resized:
                    UpdateSize(WindowPtr);
                    if (_menu != null) _menu.Width = Width;
                    break;
                case WindowEventID.SizeChanged: break;
                case WindowEventID.Minimized: break;
                case WindowEventID.Maximized: break;
                case WindowEventID.Restored: break;
                case WindowEventID.Enter: break;
                case WindowEventID.Leave: break;
                case WindowEventID.FocusGained: break;
                case WindowEventID.FocusLost: break;
                case WindowEventID.Close: break; // Handled elsewhere
                case WindowEventID.TakeFocus: break;
                case WindowEventID.HitTest: break;
                case WindowEventID.ICCProfileChanged: break;
                case WindowEventID.DisplayChanged: break;
                default:
                    throw new ArgumentOutOfRangeException(e.Event.ToString());
            }
        }
        catch (ArgumentOutOfRangeException exception)
        {
            Debug.WriteLine(exception);
        }
    }

    private void OnFirstEvent(object? sender, Event e) { }

    /// <summary>
    /// Whether or not the game is running
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// Start the Game
    /// </summary>
    public void Start()
    {
        IsRunning = true;
    }

    /// <summary>
    /// Set the Draw BackgroundColor
    /// </summary>
    /// <param name="color"></param>
    private void SetColor(Color color)
    {
        _ = SDL.SetRenderDrawColor(RendererPtr, color.R, color.G, color.B, color.A);
    }

    private void SetColorBasedOnTime()
    {
        // Daytime/Nighttime Cycles
        SetColor(DateTime.Now.Hour switch {
            < 6 or >= 18 => KnownColor.Black.ToColor(),
            >= 6 and < 8 or >= 16 and < 18 => KnownColor.DeepSkyBlue.ToColor(),
            _ => KnownColor.SkyBlue.ToColor()
        });
    }
    #region Implementation of IGameObject

    /// <inheritdoc />
    public override string Name => "Game";

    private string _text;
    private Size _textSize;

    /// <inheritdoc />
    public override void Draw()
    {
        // SetColor(new Color { A = 255, B = 23, G = 23, R = 23 });
        SetColorBasedOnTime();
        _ = SDL.RenderClear(RendererPtr);
        //_menu.Draw();
        foreach (IGameObject? obj in _gameObjects!)
        {
            obj.Draw();
        }

        if (_debug) {
            string[] a = _motionMessage.Split('\r', '\n');
            for (int y = 0; y < a.Length; y++) {
                RenderText(a[ y ], 10, 10 * y, new Color { A = 255, B = 255, G = 255, R = 255 });
            }
        }
        _menu?.Draw();

        SDL.RenderPresent(RendererPtr);
    }


    #region Event Methods

    /// <summary>
    /// Handles Quit Procedures
    /// </summary>
    public void OnQuit(object? sender, QuitEvent e)
    {
        TTF.TTF.CloseFont(Font);
        TTF.TTF.Quit();

        SDL.DestroyRenderer(RendererPtr);
        SDL.DestroyWindow(WindowPtr);
        SDL.Quit();
        IsRunning = false;
    }

    /// <summary>
    /// Handles Mouse Move Events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnMouseMove(object? sender, MouseMotionEvent e)
    {
        // Mouse Absolute Coordinates
        int mX = e.X;
        int mY = e.Y;
    }

    #endregion

    /// <inheritdoc />
    public override void Update(Event e)
    {
        base.Update(e);

        // Death Check has not countered the InvalidOpEx:
        //      "Collection was modified, enumeration operation may not execute."
        if (_dead)
        {
            return;
        }

        _menu?.Update(e);
        _text = DateTime.Now.ToString("HH:mm:ss");
        _textSize = MeasureString(_text);
        try
        {
            foreach (IGameObject? obj in _gameObjects!)
            {
                obj.Update(e);
            }
        }
        catch (InvalidOperationException exception)
        {
            Debug.WriteLine(exception);
        }
    }

    #endregion
}