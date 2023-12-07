using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using TestGame.GameObjects;

namespace TestGame {
    public class Scene {
        public List< IGameObject > GameObjects { get; } = new();
        public List< IGameObject > GameObjectsToAdd { get; } = new();
        public List< IGameObject > GameObjectsToRemove { get; } = new();

        public void AddGameObject(IGameObject gameObject) {
            GameObjectsToAdd.Add(gameObject);
        }

        public void RemoveGameObject(IGameObject gameObject) {
            GameObjectsToRemove.Add(gameObject);
        }

        public void Update(Event e) {
            foreach (IGameObject gameObject in GameObjects) {
                gameObject.Update(e);
            }

            foreach (IGameObject gameObject in GameObjectsToAdd) {
                GameObjects.Add(gameObject);
                // TODO: Add Spawn Animation Function for GameObjects
            }

            foreach (IGameObject gameObject in GameObjectsToRemove) {
                // TODO: Add Death Animation Function for GameObjects
                GameObjects.Remove(gameObject);
            }

            GameObjectsToAdd.Clear();
            GameObjectsToRemove.Clear();
        }

        public void Draw() {
            foreach (IGameObject gameObject in GameObjects) {
                gameObject.Draw();
            }
        }
    }
}