using DotTiled;

namespace TestGame.GameObjects.Map;
public class Water : ICustomTypeDefinition {
    public uint ID { get; set; } = 25;
    public string Name { get; set; } = "Water";
}
