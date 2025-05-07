namespace TestGame.GameObjects.Characters; 
public static class EntityFactory {
    public static Entity CreateEntity(string? name, string entityType, nint rendererPtr) => entityType switch {
        "NPC" => new Npc(name, rendererPtr),
        "Player" => new Player(name, rendererPtr),
        "Enemy" => new Enemy(name, rendererPtr),
        _ => throw new ArgumentException($"Unknown entity type: {entityType}")
    };
}
