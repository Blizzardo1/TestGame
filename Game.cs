#region Header

// SDL2 >SDL2 >Game.cs\n Copyright (C) , 2023\nCreated 24 11, 2023

#endregion

using System.Diagnostics;
using System.Runtime.InteropServices;
using NLog;
using TestGame;
using TestGame.GameObjects;
using TestGame.Scenes;

namespace SDL2;

public delegate void EventHandler(object? sender, Event e);

public delegate void EventHandler< in T >(object? sender, T e);

public delegate void MouseMotionEventHandler(object? sender, MouseMotionEvent e);

public delegate void MouseButtonEventHandler(object? sender, MouseButtonEvent e);

internal class Game : Window {
    private const int LocationX = 100;
    private const int LocationY = 100;

    public static uint WindowId { get; private set; }
    public static bool IsPaused { get; private set; }
    
    public static Random Random { get; } = new();

    private bool _dead;

    private string? _motionMessage;
    private bool _debug;

    private List< Scene > _scenes;

    private Scene _currentScene;

    private Diagnostics _diag;


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
            WindowFlags.AllowHighdpi | WindowFlags.Resizable | WindowFlags.Shown) {
        WindowId = SDL.GetWindowID(WindowPtr);
        Width = width;
        Height = height;

        InitializeComponents();
    }

    /// <summary>
    /// Creates a Random <see cref="Color"/>
    /// </summary>
    /// <param name="transparent">Allow Transparency</param>
    /// <returns>A completely random <see cref="Color"/></returns>
    public static Color GetRandomColor(bool transparent = false) {
        byte[] bytes = new byte[4];
        Random.NextBytes(bytes);

        Color c = new() {
            R = bytes[ 0 ],
            G = bytes[ 1 ],
            B = bytes[ 2 ],
            A = transparent ? bytes[ 3 ] : (byte)255
        };

        return c;
    }

    /// <summary>
    /// Initializes all components of the Game Window
    /// </summary>
    /// <exception cref="Exception">Failure to create the Window and Renderer</exception>
    private void InitializeComponents() {
        AttachListeners();

        _dead = false;
        IsRunning = true;
        IsPaused = false;
        _diag = new Diagnostics(RendererPtr, 256, 256);


        _scenes = new List<Scene> {
            new SampleWorld(RendererPtr, Width, Height),
            new PauseScene(RendererPtr, Width, Height)
        };

        foreach (Scene s in _scenes) {
            s.Initialize();
        }

        _currentScene = _scenes[0];
    }

    private void AttachListeners() {
        FirstEvent += OnFirstEvent;
        Quit += OnQuit;
        KeyDown += OnKeyDown;
        MouseMove += OnMouseMove;
        WindowEvent += OnWindowEvent;
    }

    // TODO: This needs to be reworked so we can map buttons
    private void OnKeyDown(object? sender, KeyboardEvent e) {
        switch (e.Keysym.Sym) {
            case Keycode.F3:
                _debug = !_debug;
                break;
            case Keycode.Escape:
                IsPaused = !IsPaused;
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
            default:
                break;
        }
    }

    private void OnWindowEvent(object? sender, WindowEvent e) {
        try {
            switch (e.Event) {
                case WindowEventID.None: break;
                case WindowEventID.Shown: break;
                case WindowEventID.Hidden: break;
                case WindowEventID.Exposed: break;
                case WindowEventID.Moved:
                    UpdatePosition(WindowPtr);
                    break;
                case WindowEventID.Resized:
                    UpdateSize(WindowPtr);
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
        catch (ArgumentOutOfRangeException exception) {
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
    public void Start() {
        IsRunning = true;
    }

    /// <summary>
    /// Set the Draw BackgroundColor
    /// </summary>
    /// <param name="color"></param>
    private void SetColor(Color color) {
        _ = SDL.SetRenderDrawColor(RendererPtr, color.R, color.G, color.B, color.A);
    }
    
    #region Implementation of IGameObject

    /// <inheritdoc />
    public override string Name => "Game";

    /// <inheritdoc />
    public override void Draw() {

        _ = SDL.RenderClear(RendererPtr);

        _currentScene.Draw();

        if (_debug) {
            _diag.Draw();
        }

        SDL.RenderPresent(RendererPtr);
    }

    #region Event Methods

    /// <summary>
    /// Handles Quit Procedures
    /// </summary>
    public void OnQuit(object? sender, QuitEvent e) {
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
    public void OnMouseMove(object? sender, MouseMotionEvent e) {
        // Mouse Absolute Coordinates
        int mX = e.X;
        int mY = e.Y;
    }

    #endregion

    /// <inheritdoc />
    public override void Update(Event e) {
        if (_dead) {
            return;
        }

        base.Update(e);
        
        _diag.UpdateDiagnostics(_currentScene);
        _diag.Update(e);
        
        // Death Check has not countered the InvalidOpEx:
        //      "Collection was modified, enumeration operation may not execute."
        _currentScene = IsPaused ? _scenes[1] : _scenes[0];


        try {
            _currentScene.Update(e);
        }
        catch (InvalidOperationException exception) {
            Debug.WriteLine(exception);
        }
    }

    #endregion
}