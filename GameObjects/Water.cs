using SDL2;

namespace TestGame.GameObjects {
    /// <inheritdoc />
    public class Water(GameContext context, Size spriteSize) : AnimatedSprite(context.RendererPtr,
        context.Rect.W,
        context.Rect.H,
        Path.Combine(Engine.StartupPath, "Water.bmp"),
        [
            new() {
                W = spriteSize.Width,
                H = spriteSize.Height,
                X = 1,
                Y = 1
            },
            new() {
                W = spriteSize.Width,
                H = spriteSize.Height,
                X = 18,
                Y = 1
            },
            new() {
                W = spriteSize.Width,
                H = spriteSize.Height,
                X = 35,
                Y = 1
            },
            new() {
                W = spriteSize.Width,
                H = spriteSize.Height,
                X = 52,
                Y = 1
            }
        ],
        8 // Delay in frames
    ) { }
}