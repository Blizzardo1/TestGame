using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.GameObjects.Characters {
    public static class EntityFactory {
        public static Entity CreateEntity(string entityType, nint rendererPtr) =>
            entityType switch {
            "Player" => new Player(rendererPtr),
            "Enemy" => new Enemy(rendererPtr),
            _ => throw new ArgumentException($"Unknown entity type: {entityType}")
        };
    }
}
