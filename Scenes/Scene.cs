using System.Collections.ObjectModel;
using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using TestGame.GameObjects;
using TestGame.Scenes.EventArgs;

namespace TestGame.Scenes; 
public delegate void SceneEventHandler(object? sender, SceneEventArgs sea);

public abstract class Scene : Renderer {
    public event SceneEventHandler? SceneEnter;
    public event SceneEventHandler? SceneLeave;

    private static readonly Log Log = Log.GetCurrentClassLogger(LogCategory.Video);
    protected bool Initialized { get; set; } = false;

    public Scene? LastScene { get; set; }
    public Scene? NextScene { get; set; }

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public Color BackgroundColor { get; set; }

    public string Name { get; }

    public string? FontName { get; set; }

    private readonly List< GameObject > _gameObjects;

    public ReadOnlyCollection< GameObject > GameObjects => _gameObjects.AsReadOnly();

    private List< GameObject > GameObjectsToAdd { get; } = [];
    private List< GameObject > GameObjectsToRemove { get; } = [];

    protected Scene(GameContext context, string name) {
        _gameObjects = [];
        Width = (int)context.Width;
        Height = (int)context.Height;
        LastScene = null;
        BackgroundColor = context.Color;
        Name = name;
        FontName = context.FontName;
        Initialize(context.RendererPtr);
    }

    protected void SetColor(Color color) {
        _ = Sdl.SetRenderDrawColor(RendererPtr, color);
    }

    public abstract void Initialize();

    public abstract void Cleanup();

    public void OnSceneEnter(object? sender, SceneEventArgs sea) {
        if (sea.Scene == this) {
            SceneEnter?.Invoke(sender, sea);
        }
    }

    public void OnSceneLeave(object? sender, SceneEventArgs sea) {
        if (sea.LastScene == this) {
            SceneLeave?.Invoke(sender, sea);
        }
    }

    public void AddGameObject(GameObject gameObject) {
        gameObject.Initialize();
        GameObjectsToAdd.Add(gameObject);
    }

    public void RemoveGameObject(GameObject gameObject) {
        GameObjectsToRemove.Add(gameObject);
    }

    public virtual void Draw() {
        foreach (IRenderer gameObject in _gameObjects) {
            gameObject.Draw();
        }
    }

    public virtual void Update(Event e) {
        foreach (GameObject gameObject in _gameObjects) {
            gameObject.Update(e);
        }

        foreach (GameObject gameObject in GameObjectsToAdd) {
            _gameObjects.Add(gameObject);
            Log.Debug($"Added game object {gameObject.Name}");
            // #TODO: Add Spawn Animation Function for _gameObjects
        }

        foreach (GameObject gameObject in GameObjectsToRemove.Where(_gameObjects.Remove)) {
            Log.Debug($"Removed game object {gameObject.Name}");
        }

        if (GameObjectsToRemove.Count > 0) {
            Log.Error($"Cannot remove {GameObjectsToRemove.Count} {( GameObjectsToRemove.Count != 1 ? "objects" : "object" )} at this time.");
        }

        GameObjectsToAdd.Clear();
        GameObjectsToRemove.Clear();
    }
}