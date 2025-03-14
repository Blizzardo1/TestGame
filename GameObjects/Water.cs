using SDL2;

namespace TestGame.GameObjects {
    /// <inheritdoc />
    public class Water(GameContext context) : AnimatedSprite(context.RendererPtr, context.Rect.W, context.Rect.H, Path.Combine(Engine.StartupPath, "Water.bmp"),
            [
                    new() {
                        W = context.Rect.W,
                        H = context.Rect.H,
                        X = 1,
                        Y = 1
                    },
                    new() {
                        W = context.Rect.W,
                        H = context.Rect.H,
                        X = 18,
                        Y = 1
                    },
                    new() {
                        W = context.Rect.W,
                        H = context.Rect.H,
                        X = 35,
                        Y = 1
                    },
                    new() {
                        W = context.Rect.W,
                        H = context.Rect.H,
                        X = 52,
                        Y = 1
                    }
                ],
            6 // Delay in frames
            ) {
        // private const int PSize = 16;
    }
}