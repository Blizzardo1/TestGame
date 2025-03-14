using SDL2;

namespace TestGame.GameObjects;

public interface IRenderer : IGameObject {
    float X { get; }
    float Y { get; }
    float Z { get; }

    int Width { get; }
    int Height { get; }

    void Draw();
    void Update(Event e);
}
