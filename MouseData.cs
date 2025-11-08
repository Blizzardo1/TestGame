using SharpSDL3.Structs;

namespace TestGame;

public record MouseData(Vector2 Position, byte Button, float WheelDirection);