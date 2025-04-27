using TestGame.GameObjects;
using TestGame.Scenes;

namespace TestGame; 
public static class SceneFactory {
    // TODO: Finish Implementation of CreateScene
    public static T CreateScene< T >(string name, GameContext context) where T : Scene {
        T newScene = (T)Activator.CreateInstance(typeof(T), context, name)!
                     ?? throw new Exception("Failed to create scene");
        return newScene;
    }
}