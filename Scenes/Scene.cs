using System.Collections.ObjectModel;
using NLog;
using SDL2;
using TestGame.GameObjects;
using TestGame.Scenes.EventArgs;

namespace TestGame.Scenes {
    
    public delegate void SceneEventHandler(object? sender, SceneEventArgs sea);
    
    public abstract class Scene : Renderer {

        public event SceneEventHandler? SceneEnter;
        public event SceneEventHandler? SceneLeave;

        public Scene? LastScene { get; set; }
        
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Color BackgroundColor { get; set; }

        public string Name { get; }

        private readonly List< IRenderer > _gameObjects;
        private readonly Logger _log;

        public ReadOnlyCollection< IRenderer > GameObjects => _gameObjects.AsReadOnly();

        private List<IRenderer> GameObjectsToAdd { get; } = [];
        private List<IRenderer> GameObjectsToRemove { get; } = [];

        public Scene(GameContext context, string name) {
            _log = LogManager.GetCurrentClassLogger();
            _gameObjects = [];
            Width = context.Rect.W;
            Height = context.Rect.H;
            LastScene = null;
            BackgroundColor = context.Color;
            Name = name;
            Initialize(context.RendererPtr);
        }

        protected void SetColor(Color color) {
            _ = SDL.SetRenderDrawColor(RendererPtr, color.R, color.G, color.B, color.A);
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

        public void AddGameObject(IRenderer renderer) {
            GameObjectsToAdd.Add(renderer);
        }

        public void RemoveGameObject(IRenderer renderer) {
            GameObjectsToRemove.Add(renderer);
        }

        public virtual void Draw() {
            foreach (IRenderer gameObject in _gameObjects) {
                gameObject.Draw();
            }
        }
         
        public virtual void Update(Event e) {
            foreach (IRenderer gameObject in _gameObjects) {
                gameObject.Update(e);
            }

            foreach (IRenderer gameObject in GameObjectsToAdd) {
                _gameObjects.Add(gameObject);
                _log.Trace($"Added game object {gameObject.Name}");
                // TODO: Add Spawn Animation Function for _gameObjects
                
            }

            foreach (IRenderer gameObject in GameObjectsToRemove.Where(_gameObjects.Remove)) {
                _log.Trace($"Removed game object {gameObject.Name}");
            }

            if (GameObjectsToRemove.Count > 0) {
                _log.Error($"Cannot remove {GameObjectsToRemove.Count} {(GameObjectsToRemove.Count != 1 ? "objects" : "object")} at this time.");
            }

            GameObjectsToAdd.Clear();
            GameObjectsToRemove.Clear();
        }
    }
}