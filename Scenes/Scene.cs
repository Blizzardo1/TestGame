using System.Collections.ObjectModel;
using NLog;
using SDL2;
using TestGame.GameObjects;

namespace TestGame.Scenes {
    public abstract class Scene : Renderer {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public abstract string Name { get; }

        private List< IGameObject > _gameObjects;
        private Logger _log;

        public ReadOnlyCollection< IGameObject > GameObjects => _gameObjects.AsReadOnly();

        private List< IGameObject > GameObjectsToAdd { get; } = new();
        private List< IGameObject > GameObjectsToRemove { get; } = new();

        public Scene(nint rendererPtr, int width, int height) {
            _log = LogManager.GetCurrentClassLogger();
            _gameObjects = new List< IGameObject >();
            Initialize(rendererPtr);
            Width = width;
            Height = height;
        }

        protected void SetColor(Color color) {
            _ = SDL.SetRenderDrawColor(RendererPtr, color.R, color.G, color.B, color.A);
        }

        public abstract void Initialize();

        public void AddGameObject(IGameObject gameObject) {
            GameObjectsToAdd.Add(gameObject);
        }

        public void RemoveGameObject(IGameObject gameObject) {
            GameObjectsToRemove.Add(gameObject);
        }

        public virtual void Draw() {
            foreach (IGameObject gameObject in _gameObjects) {
                gameObject.Draw();
            }
        }

        public virtual void Update(Event e) {
            foreach (IGameObject gameObject in _gameObjects) {
                gameObject.Update(e);
            }

            foreach (IGameObject gameObject in GameObjectsToAdd) {
                _gameObjects.Add(gameObject);
                _log.Debug($"Added game object {gameObject.Name}");
                // TODO: Add Spawn Animation Function for _gameObjects
            }

            foreach (IGameObject gameObject in GameObjectsToRemove.Where(_gameObjects.Remove)) {
                _log.Debug($"Removed game object {gameObject.Name}");
            }

            GameObjectsToAdd.Clear();
            GameObjectsToRemove.Clear();
        }
    }
}