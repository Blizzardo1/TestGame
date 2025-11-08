using SharpSDL3.Structs;

namespace TestGame.GameObjects;

public interface IRenderer : IGameObject {
    float X { get; }
    float Y { get; }
    float Z { get; }

    float Width { get; }
    float Height { get; }

    /// <summary>
    /// Draw objects to the screen
    /// </summary>
    void Draw();

    /// <summary>
    /// Handles all updates
    /// </summary>
    /// <param name="e">A <see cref="Event"/> of the current frame</param>
    void Update(Event e);
}