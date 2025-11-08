

using SharpSDL3.Structs;

namespace TestGame.GameObjects; 
public record GameContext(nint WindowPtr, nint RendererPtr, FRect Rect, Color Color, Core Game, string FontName = "default") {
    public float Width => Rect.W;
    public float Height => Rect.H;
}