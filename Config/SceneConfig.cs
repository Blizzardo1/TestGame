using Newtonsoft.Json;
using TestGame.GameObjects;

namespace TestGame.Config; 
internal class SceneConfig(string name, string sceneType, List< GameObject > gameObjects) {
    /// <summary>
    /// A Reference to a given Scene
    /// </summary>
    /// <example>scene/main</example>
    [JsonProperty("name")]
    public string SceneName { get; } = name;

    /// <summary>
    /// The Type of Scene
    /// </summary>
    /// <example>TestGame.Scenes.PauseScene</example>
    [JsonProperty("type")]
    public string Type { get; } = sceneType;

    /// <summary>
    /// The Game objects to load into the scene
    /// </summary>
    [JsonProperty("objects")]
    public List< GameObject > GameObjects { get; } = gameObjects;
}