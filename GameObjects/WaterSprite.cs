using SharpSDL3.Structs;

namespace TestGame.GameObjects; 
/// <inheritdoc />
public class WaterSprite(GameContext context, Size spriteSize)
    : AnimatedSprite(context, Path.Combine(Engine.StartupPath, "Water.bmp"),
    [
        new FRect {
            W = spriteSize.Width,
            H = spriteSize.Height,
            X = 1,
            Y = 1
        },
        new FRect {
            W = spriteSize.Width,
            H = spriteSize.Height,
            X = 18,
            Y = 1
        },
        new FRect {
            W = spriteSize.Width,
            H = spriteSize.Height,
            X = 35,
            Y = 1
        },
        new FRect {
            W = spriteSize.Width,
            H = spriteSize.Height,
            X = 52,
            Y = 1
        }
    ],
    8 // Delay in frames
);