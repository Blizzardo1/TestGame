using SharpSDL3.Structs;

namespace TestGame.GameObjects;

public interface IRenderer : IGameObject {
    float X { get; }
    float Y { get; }
    float Z { get; }

    float Width { get; }
    float Height { get; }

    void Draw();
    void Update(Event e);
}