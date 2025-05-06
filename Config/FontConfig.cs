using Newtonsoft.Json;

namespace TestGame.Config; 
public class FontConfig([JsonProperty("name")] string? name, [JsonProperty("font")] string? font) {
    public string? Name { get; } = name;
    public string? Font { get; } = font;
}