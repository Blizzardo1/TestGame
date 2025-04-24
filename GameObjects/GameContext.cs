using SDL2;

namespace TestGame.GameObjects {
    public record GameContext(nint RendererPtr, Rect Rect, Color Color, Core Game, string FontName = "default");
}