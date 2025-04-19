#region Header

// SDL2 >SDL2 >Game.cs\n Copyright (C) , 2023\nCreated 24 11, 2023

#endregion

using System.Diagnostics;
using NLog;
using SDL2;
using SDL2.TTF;
using TestGame.Colors;
using TestGame.Config;
using TestGame.GameObjects;
using TestGame.Scenes;
using TestGame.Scenes.EventArgs;

namespace TestGame;

/// <summary>
/// Standard Event Handler
/// </summary>
/// <param name="sender">object triggering the event</param>
/// <param name="e">An SDL <see cref="Event"/> containing information about the triggered event</param>
public delegate void EventHandler(object? sender, Event e);

/// <summary>
/// Standard Event Handler that takes in an <typeparamref name="T"/> parameter
/// for a more customized and refined event system
/// </summary>
/// <typeparam name="T">struct or class type object</typeparam>
/// <param name="sender">object triggering the event</param>
/// <param name="e"><typeparamref name="T"/> object containing information (if any) about the triggered event</param>
public delegate void EventHandler< in T >(object? sender, T e);

/// <summary>
/// Handles any mouse motion
/// </summary>
/// <param name="sender">object triggering the event</param>
/// <param name="e">An SDL <see cref="MouseMotionEvent"/> containing information about the triggered event</param>
public delegate void MouseMotionEventHandler(object? sender, MouseMotionEvent e);

/// <summary>
/// Handles any mouse button
/// </summary>
/// <param name="sender">object triggering the event</param>
/// <param name="e">An SDL <see cref="MouseButtonEvent"/> containing information about the triggered event</param>
public delegate void MouseButtonEventHandler(object? sender, MouseButtonEvent e);

public class Core : Window {
    public static uint WindowId { get; private set; }
    public static bool IsPaused { get; private set; }

    private static Core? _instance;
    public static Core Instance => _instance!;

    public static Random Random { get; } = new();

    private bool _dead;
    private bool _debug;

    private Dictionary< string, Scene >? _scenes;

    private Scene? _currentScene;

    private Diagnostics? _diagnostic;

    private static readonly Logger? Log = LogManager.GetCurrentClassLogger();

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

    public void Die() {
        _dead = true;
        IsRunning = false;
    }

    public void ToggleDebug() {
        _debug = !_debug;
    }

    /// <summary>
    /// Adds a <see cref="Scene"/> to the Scene Engine
    /// </summary>
    /// <param name="scene"></param>
    public void AddScene(Scene scene) {
        _scenes ??= [];
        _scenes.Add(scene.Name, scene);
    }

    public void NextScene(Scene scene) {
        if (_currentScene is null) {
            Log?.Error("_currentScene is unset!");
            return;
        }

        scene.LastScene = _currentScene;
        _currentScene.NextScene = scene;
    }

    /// <summary>
    /// Gets a <see cref="Scene"/> by name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="scene"></param>
    /// <returns></returns>
    public bool TryGetScene< T >(string name, out T? scene) where T : Scene {
        if (_scenes is null) {
            scene = null;
            return false;
        }

        if (_scenes.TryGetValue(name, out Scene? s)) {
            scene = (T)s;
            return true;
        }

        scene = null;
        return false;
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
        if (_scenes is null) {
            throw new NullReferenceException("Scene Engine needs to be initialized. Use AddScene or AddScenes");
        }

        AttachListeners();

        _dead = false;
        IsRunning = true;
        IsPaused = false;

        _ = SDL.SetRenderDrawBlendMode(RendererPtr, BlendMode.Blend);

        _diagnostic = new Diagnostics(RendererPtr, 256, 256);

        // TODO: This needs to be moved away from the engine and processed per separate Application.

        if(Engine.GameConfig.AudioTracks is not null) {
            foreach(AudioConfig ac in Engine.GameConfig.AudioTracks) {
                ResourceManager.Add(new SoundEffect(ac.Reference, ac.Path));
            }
        }

        #if WINDOWS
        // SoundEffect startup = new ("audio/startup", @"E:\Users\Adonis\Music\Youtube Stuff\98 Media\WO_START.WAV");
        ResourceManager.Add(new SoundEffect("audio/startup",
            @"E:\Users\Adonis\Music\OST\Yoshi's Island\01-NintendoMark.mp3"));
        ResourceManager.Add(new SoundEffect("audio/shutdown",
            @"E:\Users\Adonis\Music\Youtube Stuff\XP Media\tada.wav"));
        ResourceManager.Add(new SoundEffect("audio/pause",
            @"E:\Users\Adonis\Music\OST\Earthbound\170- Earthbound - OK _Ssuka_.mp3"));
        // audio/bgm/overworld
        // audio/bgm/dungeon
        // audio/bgm/boss
        // audio/sfx/kick
        // audio/sfx/punch
        // audio/sfx/sword
        // npc/antagonist
        // npc/village/priest
        // npc/city/priest
        #endif
        foreach (Scene s in _scenes!.Values) {
            s.Initialize();
        }

        ( _, _currentScene ) = _scenes.First();
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
        Log?.Info("Installing listeners");
        FirstEvent += OnFirstEvent;
        Quit += OnQuit;
        MouseMove += OnMouseMove;
        MouseDown += OnMouseDown;
        MouseWheel += OnMouseWheel;
        MouseUp += OnMouseUp;
        WindowEvent += OnWindowEvent;
        Log?.Info("Listeners installed.");
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

    private void OnWindowEvent(object? sender, WindowEvent e) {
        try {
            switch (e.Event) {
                case WindowEventID.None:
                case WindowEventID.Shown:
                case WindowEventID.Hidden:
                case WindowEventID.Exposed:
                    break;
                case WindowEventID.Moved:
                    UpdatePosition(WindowPtr);
                    break;
                case WindowEventID.Minimized:
                case WindowEventID.Restored:
                    break;
                case WindowEventID.Maximized:
                case WindowEventID.Resized:
                case WindowEventID.SizeChanged:
                    UpdateSize(WindowPtr);
                    break;
                case WindowEventID.Enter:
                case WindowEventID.Leave:
                case WindowEventID.FocusGained:
                case WindowEventID.FocusLost:
                case WindowEventID.Close:
                case WindowEventID.TakeFocus:
                case WindowEventID.HitTest:
                case WindowEventID.ICCProfileChanged:
                case WindowEventID.DisplayChanged:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(e.Event.ToString());
            }
        }
        catch (ArgumentOutOfRangeException exception) {
            Debug.WriteLine(exception);
        }
    }

    private void OnFirstEvent(object? sender, Event e) {
        Log?.Info("Game Engine Fired away!");
    }

    /// <summary>
    /// Whether the game is running
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Clean up any resources used
    /// </summary>
    private void Cleanup() {
        if (_scenes is null) {
            return;
        }

        foreach (Scene s in _scenes.Values) {
            s.Cleanup();
        }
    }

    /// <summary>
    /// As it states, toggle the pause
    /// </summary>
    public void TogglePause() {
        IsPaused = !IsPaused;
    }

    /// <summary>
    /// Start the Game
    /// </summary>
    public virtual void Start() {
        IsRunning = true;
    }

    /// <summary>
    /// Stops execution
    /// </summary>
    public virtual void Stop() {
        IsRunning = false;
        Cleanup();
    }

    #region Implementation of IRenderable

    /// <inheritdoc />
    public override string Name => "Game Engine";

    /// <inheritdoc />
    public override void Draw() {
        SetRenderColor(RendererPtr,
            // We should have this set already, buuuut, if not... DAVE!? Wait... Who's Dave!?
            _currentScene?.BackgroundColor
            // Default to this if the Current Scene's Background Color isn't set.
            ?? KnownColor.Black.ToColor());

        _ = SDL.RenderClear(RendererPtr);

        _currentScene?.Draw();

        if (_debug) {
            _diagnostic?.Draw();
        }

        SDL.RenderPresent(RendererPtr);
    }

    #region Event Methods

    /// <summary>
    /// Handles Quit Procedures
    /// </summary>
    private void OnQuit(object? sender, QuitEvent e) {
        CloseFonts();
        TTF.Quit();

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
    private void OnMouseMove(object? sender, MouseMotionEvent e) {
        if (_diagnostic is null) return;
        _diagnostic.MouseData = _diagnostic.MouseData with {
            Position = new System.Numerics.Vector2 { X = e.X, Y = e.Y }
        };
    }

    private void OnMouseDown(object? sender, MouseButtonEvent e) {
        if (_diagnostic is null) return;
        _diagnostic.MouseData = _diagnostic.MouseData with { Button = e.Button };
    }

    private void OnMouseUp(object? sender, MouseButtonEvent e) {
        if (_diagnostic is null) return;
        _diagnostic.MouseData = _diagnostic.MouseData with { Button = 0 };
    }

    private void OnMouseWheel(object? sender, MouseWheelEvent e) {
        if (_diagnostic is null) return;
        _diagnostic.MouseData = _diagnostic.MouseData with { WheelDirection = e.Y };
    }

    #endregion

    /// <inheritdoc />
    public override void Update(Event e) {
        if (_currentScene == null) {
            return;
        }

        if (_dead) {
            return;
        }

        base.Update(e);

        _diagnostic?.UpdateDiagnostics(_currentScene);
        _diagnostic?.Update(e);

        // Death Check has not countered the InvalidOpEx:
        //      "Collection was modified, enumeration operation may not execute."
        Scene lastScene = _currentScene;
        /* This needs to be handled better. Maybe ResourceManager.Get<PauseMenu>()?
            Either way, something needs to be done to include multiple scenes
            while still referencing the Pause Menu.
         */

        if (_currentScene.NextScene is not null) {
            _currentScene = _currentScene.NextScene;
            _currentScene!.LastScene = lastScene;
            lastScene.NextScene = null;
        }

        SceneEventArgs sea = new(_currentScene, lastScene);

        if (_currentScene != lastScene) {
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