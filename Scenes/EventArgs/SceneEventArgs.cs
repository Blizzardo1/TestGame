namespace TestGame.Scenes.EventArgs; 
public class SceneEventArgs(Scene scene, Scene lastScene) {
    public Scene Scene { get; } = scene;
    public Scene LastScene { get; } = lastScene;
}