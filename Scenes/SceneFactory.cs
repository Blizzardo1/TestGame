using TestGame.GameObjects;

namespace TestGame.Scenes; 
internal class SceneFactory {
    public static T? CreateScene< T >(string? type, GameContext context) where T : Scene =>
        Activator.CreateInstance(Type.GetType(type ?? "TestGame.Scenes.Scene") ?? typeof(T), context) as T ?? null;
}