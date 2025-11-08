#region Header

// SDL2 >SDL2 >Game.cs\n Copyright (C) , 2023\nCreated 24 11, 2023

#endregion

using System.Diagnostics;
using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
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

/// <summary>
/// Core Engine
/// </summary>
public class Core : Window {
    private static readonly Log Log;

    public static uint WindowId { get; private set; }
    public static bool IsPaused { get; private set; }

    public static bool IsDebugging { get; private set; }
    public static bool IsPausedDisabled { get; private set; } = true;

    public static Random Random { get; } = new Random();

    private Dictionary< string, Scene >? _scenes;

    private Scene? _currentScene;

    private Diagnostics? _diagnostic;

    static Core() {
        Log = Log.GetCurrentClassLogger(LogCategory.Application);
        Log.Info("Core Engine Initialized");
    }

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
            WindowFlags.HighPixelDensity | WindowFlags.Resizable) {
        WindowId = Sdl.GetWindowId(WindowPtr);
        Log.Info($"Created Window with ID: {WindowId}");
        Width = width;
        Height = height;
    }

    public static void ToggleDebug() {
        IsDebugging = !IsDebugging;

        if(Engine.Game is null || Engine.Game._diagnostic is null) {
            return;
        }

        Engine.Game._diagnostic.Shown = IsDebugging;
    }

    /// <summary>
    /// Adds a <see cref="Scene"/> to the Scene Engine
    /// </summary>
    /// <param name="scene"></param>
    public void AddScene(Scene scene) {
        _scenes ??= [];
        _scenes.Add(scene.Name, scene);
    }

    public void NextScene(Scene? scene) {
        if(scene is null) {
            return;
        }

        if (_currentScene is null) {
            Log.Error("_currentScene is unset!");
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

        if (_scenes.TryGetValue(name, out Scene? s) && s is not null) {
            // InvalidOperationException:
            // Unable to cast object of type 'Bazinga.Scenes.MainMenu' to type 'Bazinga.Scenes.TiledScene'.'
            scene = (T)s;
            return true;
        }

        Log.Error($"Scene with name {name} does not exist");
        scene = null;
        return false;
    }

    /// <summary>
    /// Creates a Random <see cref="Color"/>
    /// </summary>
    /// <param name="transparent">Allow Transparency</param>
    /// <returns>A completely random <see cref="Color"/></returns>
    public static Color GetRandomColor(bool transparent = false, Color? backgroundColor = null) {
        byte[] bytes = new byte[4];
        
        if(backgroundColor is not null) {
            Random.NextBytes(bytes);
            Log.Debug($"Color: {bytes[0]:X2} {bytes[1]:X2} {bytes[2]:X2} {bytes[3]:X2}");
            int color = bytes[0] << 24
                | bytes[1] << 16
                | bytes[2] << 8
                | (transparent ? bytes[3] : 255);
            if(color < 0x7F7F7FFF) {
                bytes[0] = (byte)~bytes[0];
                bytes[1] = (byte)~bytes[1];
                bytes[2] = (byte)~bytes[2];
                Log.Debug($"Inverted Color: {bytes[0]:X2} {bytes[1]:X2} {bytes[2]:X2} {bytes[3]:X2}");
            }
        } else {
            Random.NextBytes(bytes);
        }

        Color c = new() {
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
    public void InitializeComponents() {
        if (_scenes is null) {
            throw new ArgumentNullException("Scene Engine needs to be initialized. Use AddScene or AddScenes", new Exception());
        }

        IsRunning = true;
        IsPaused = false;

        _ = Sdl.SetRenderDrawBlendMode(RendererPtr, BlendMode.Blend);

        _diagnostic = new Diagnostics(RendererPtr, 256, 256);

        // #TODO: This needs to be moved away from the engine and processed per separate Application.

        if(Engine.GameConfig?.AudioTracks is not null) {
            foreach(AudioConfig ac in Engine.GameConfig.AudioTracks) {
                ResourceManager.Add(new SoundEffect(ac.Reference, ac.Path));
            }
        }

        foreach (Scene s in _scenes!.Values) {
            s.Initialize();
        }

        ( _, _currentScene ) = _scenes.First();
    }

    /// <summary>
    /// Wrapper to set Render Draw Color more efficiently.
    /// </summary>
    /// <param name="rendererPtr">A Renderer* to the current renderer.</param>
    /// <param name="color">The new color to set to the Renderer*.</param>
    /// <returns>0 on success, negative error code on failure</returns>
    public static bool SetRenderColor(nint rendererPtr, Color color) {
        return Sdl.SetRenderDrawColor(rendererPtr, color);
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

    public void UpdateDiagnostics<T>(T e) where T: struct {
        if(_diagnostic is null) {
            return;
        }

        switch (e) {
            case MouseMotionEvent mme:
                _diagnostic.MouseData = _diagnostic.MouseData with {
                    Position = new Vector2 { X = mme.X, Y = mme.Y }
                };
                break;
            case MouseButtonEvent mbe:
                // SDL_PRESSED = 1, SDL_RELEASED = 0
                if (mbe.Clicks == 1) {
                    _diagnostic.MouseData = _diagnostic.MouseData with { Button = mbe.Button };
                } else if (mbe.Clicks == 0) {
                    _diagnostic.MouseData = _diagnostic.MouseData with { Button = 0 };
                }
                _diagnostic.MouseData = _diagnostic.MouseData with { Button = mbe.Button };
                break;
            case MouseWheelEvent mwe:
                _diagnostic.MouseData = _diagnostic.MouseData with { WheelDirection = mwe.Y };
                break;
        }
    }

    

    /// <summary>
    /// Toggles the Pause State
    /// </summary>
    /// <remarks>
    /// Whether to disable the Pause State or not.
    /// </remarks>
    public static void DisablePauseMenu(bool state = true) {
        IsPausedDisabled = state;
    }

    /// <summary>
    /// As it states, toggle the pause
    /// </summary>
    public static void TogglePause() {
        if(IsPausedDisabled) {
            return;
        }

        IsPaused = !IsPaused;
        AudioManager.Instance.PlaySoundEffect(IsPaused ? "pause" : "shutdown", 2);
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

        _ = Sdl.RenderClear(RendererPtr);

        _currentScene?.Draw();

        if (_diagnostic!.Shown) {
            _diagnostic.Draw();
        }

        Sdl.RenderPresent(RendererPtr);
    }

    #region Event Methods

    /// <summary>
    /// Handles Quit Procedures
    /// </summary>
    public void Close() {
        foreach(Scene scene in _scenes!.Values) {
            scene.Cleanup();
        }
        Ttf.Quit();

        Sdl.DestroyRenderer(RendererPtr);
        Sdl.DestroyWindow(WindowPtr);
        Sdl.Quit();
        IsRunning = false;
    }

    #endregion

    /// <inheritdoc />
    public override void Update(Event e) {
        if (_currentScene == null || !IsRunning) {
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