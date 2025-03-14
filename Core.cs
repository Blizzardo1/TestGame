#region Header

// SDL2 >SDL2 >Game.cs\n Copyright (C) , 2023\nCreated 24 11, 2023

#endregion

using System.Diagnostics;
using System.Runtime.InteropServices;
using Forms = Eto.Forms;
using NLog;
using TestGame;
using TestGame.Colors;
using TestGame.GameObjects;
using TestGame.Scenes;
using TestGame.Scenes.EventArgs;

namespace SDL2;

public delegate void EventHandler(object? sender, Event e);

public delegate void EventHandler< in T >(object? sender, T e);

public delegate void MouseMotionEventHandler(object? sender, MouseMotionEvent e);

public delegate void MouseButtonEventHandler(object? sender, MouseButtonEvent e);

public class Core : Window {
    private const int LocationX = 100;
    private const int LocationY = 100;

    public static uint WindowId { get; private set; }
    public static bool IsPaused { get; private set; }

    private static Core? _instance;
    public static Core Instance { get => _instance!; }
    
    public static Random Random { get; } = new();

    private bool _dead;    
    private bool _debug;

    private Dictionary<string, Scene>? _scenes;

    private Scene? _currentScene;
    private string? _initialSceneName;
    private const string PauseMenu = "pause";

    private Diagnostics? _diag;
    

    /// <summary>
    /// Creates a new Game
    /// </summary>
    /// <param name="width">The Width</param>
    /// <param name="height">The Height</param>
    /// <param name="title">The Title</param>
    public Core(int width, int height, string title)
        : base(
            title,
            new Point { X = 0x7FFFFFFF, Y = 0x7FFFFFFF },
            new Size(width, height),
            WindowFlags.AllowHighdpi | WindowFlags.Resizable | WindowFlags.Shown) {
        WindowId = SDL.GetWindowID(WindowPtr);
        Width = width;
        Height = height;
        _instance = this;
    }

    /// <summary>
    /// Adds a <see cref="Scene"/> to the Scene Engine
    /// </summary>
    /// <param name="scene"></param>
    public void AddScene(Scene scene) {
        _scenes ??= [];
        _scenes.Add(scene.Name, scene);
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
    public void InitializeComponents() {
        if(_scenes is null) {
            throw new NullReferenceException("Scene Engine needs to be initialized. Use AddScene or AddScenes");
        }

        AttachListeners();

        _dead = false;
        IsRunning = true;
        IsPaused = false;

        _ = SDL.SetRenderDrawBlendMode(RendererPtr, BlendMode.Blend);
        

        _diag = new Diagnostics(RendererPtr, 256, 256);

        // SoundEffect startup = new ("audio/startup", @"E:\Users\Adonis\Music\Youtube Stuff\98 Media\WO_START.WAV");
        ResourceManager.Add(new SoundEffect("audio/startup", @"E:\Users\Adonis\Music\OST\Yoshi's Island\01-NintendoMark.mp3"));
        ResourceManager.Add(new SoundEffect("audio/shutdown", @"E:\Users\Adonis\Music\Youtube Stuff\XP Media\tada.wav"));
        ResourceManager.Add(new SoundEffect("audio/pause", @"E:\Users\Adonis\Music\OST\Earthbound\170- Earthbound - OK _Ssuka_.mp3"));
        // audio/bgm/overworld
        // audio/bgm/dungeon
        // audio/bgm/boss
        // audio/sfx/kick
        // audio/sfx/punch
        // audio/sfx/sword
        // npc/antagonist
        // npc/village/priest
        // npc/city/priest

        foreach (Scene s in _scenes!.Values) {
            s.Initialize();
        }

        (_initialSceneName, _currentScene) = _scenes.First();
    }

    /// <summary>
    /// A Pointer to the Renderer
    /// </summary>
    /// <returns>A <see cref="nint"/> Pointer to the Renderer</returns>
    public nint GetRenderer() => RendererPtr;

    /// <summary>
    /// A Pointer to the Window
    /// </summary>
    /// <returns>A <see cref="nint"/> Pointer to the Window</returns>
    public nint GetWindow() => WindowPtr;

    private void AttachListeners() {
        FirstEvent += OnFirstEvent;
        Quit += OnQuit;
        KeyDown += OnKeyDown;
        MouseMove += OnMouseMove;
        MouseDown += OnMouseDown;
        MouseWheel += OnMouseWheel;
        MouseUp += OnMouseUp;
        WindowEvent += OnWindowEvent;
    }
    
    /// <summary>
    /// Wrapper to set Render Draw Color more efficiently.
    /// </summary>
    /// <param name="rendererPtr">A Renderer* to the current renderer.</param>
    /// <param name="color">The new color to set to the Renderer*.</param>
    /// <returns>0 on success, negative error code on failure</returns>
    public static int SetRenderColor(nint rendererPtr, Color color) {
        return SDL.SetRenderDrawColor(rendererPtr, color.R, color.G, color.B, color.A);
    }

    // TODO: This needs to be reworked so we can map buttons
    private void OnKeyDown(object? sender, KeyboardEvent e) {
        if (e.Keysym.Mod == Keymod.LCtrl) {
            switch (e.Keysym.Sym) {
                case Keycode.q:
                    _dead = true;
                    IsRunning = false;
                    break;
            }
        }

        switch (e.Keysym.Sym) {
            case Keycode.F3:
                _debug = !_debug;
                break;
            case Keycode.Escape:
                TogglePause();
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
    public bool IsRunning { get; private set; }
    

    private void Cleanup() {
        if (_scenes is null) {
            return;
        }
        
        foreach (Scene s in _scenes.Values) {
            s.Cleanup();
        }
    }

    public void TogglePause() {
        IsPaused = !IsPaused;
    }

    /// <summary>
    /// Start the Game
    /// </summary>
    public void Start() {
        IsRunning = true;
        var start = ResourceManager.Get<SoundEffect>("audio/startup");
        // start.SetVolume(48);
        // start.Play();
    }

    public void Stop() {
        IsRunning = false;
        var end = ResourceManager.Get<SoundEffect>("audio/shutdown");
        // end.SetVolume(48);
        // end.Play();
        Cleanup();
    }
    
    #region Implementation of IRenderable

    /// <inheritdoc />
    public override string Name => "Game";

    /// <inheritdoc />
    public override void Draw() {

        SetRenderColor(RendererPtr,
            // We should have this set already, buuuut, if not... DAVE!? Wait... Who's Dave!?
            _currentScene?.BackgroundColor
            // Default to this if the Current Scene's Background Color isn't set.
            ?? Colors.FromKnownColor(KnownColor.Black));
        
        _ = SDL.RenderClear(RendererPtr);

        _currentScene?.Draw();

        if (_debug) {
            _diag?.Draw();
        }

        
        SDL.RenderPresent(RendererPtr);
    }

    #region Event Methods

    /// <summary>
    /// Handles Quit Procedures
    /// </summary>
    public void OnQuit(object? sender, QuitEvent e) {
        CloseFonts();
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
        _diag!.MouseData = _diag.MouseData with { Position = new System.Numerics.Vector2 { X = e.X, Y = e.Y } };
    }

    private void OnMouseDown(object? sender, MouseButtonEvent e) {
        _diag!.MouseData = _diag.MouseData with { Button = e.Button };
    }

    private void OnMouseUp(object? sender, MouseButtonEvent e) {
        _diag!.MouseData = _diag.MouseData with { Button = 0 };
    }
    private void OnMouseWheel(object? sender, MouseWheelEvent e) {
        _diag!.MouseData = _diag.MouseData with { WheelDirection = e.Y };
    }


    #endregion

    /// <inheritdoc />
    public override void Update(Event e) {
        if(_currentScene == null) {
            return;
        }
        
        if (_dead) {
            return;
        }

        base.Update(e);
        
        _diag?.UpdateDiagnostics(_currentScene);
        _diag?.Update(e);

        // Death Check has not countered the InvalidOpEx:
        //      "Collection was modified, enumeration operation may not execute."
        Scene lastScene = _currentScene;
        /* This needs to be handled better. Maybe ResourceManager.Get<PauseMenu>()?
            Either way, something needs to be done to include multiple scenes
            while still referencing the Pause Menu.
         */
        _currentScene = IsPaused ? _scenes[PauseMenu] : _scenes![_initialSceneName!];
        _currentScene!.LastScene = lastScene;

        SceneEventArgs sea = new(_currentScene, lastScene);
        
        if(_currentScene != lastScene) {
            lastScene.OnSceneLeave(this, sea);
            _currentScene.OnSceneEnter(this, sea);
        }
        
        try {
            _currentScene.Update(e);
        }
        catch (InvalidOperationException exception) {
            Debug.WriteLine(exception);
        }
    }

    #endregion
}