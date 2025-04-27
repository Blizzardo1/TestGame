using SDL2;
using TestGame.Colors;
using TestGame.GameObjects;

namespace TestGame.Scenes; 
public class SampleWorld(GameContext context, string name) : Scene(context, name) {

    private readonly GameContext _context = context;

    private void SetColorBasedOnTime() {
        // Daytime/Nighttime Cycles
        SetColor(DateTime.Now.Hour switch {
            < 6 or >= 18 => KnownColor.Black.ToColor(),
            >= 6 and < 8 or >= 16 and < 18 => KnownColor.DeepSkyBlue.ToColor(),
            _ => KnownColor.SkyBlue.ToColor()
        });
    }

    #region Overrides of Scene

    /// <inheritdoc />
    public override void Initialize() {
        for (int y = 0; y < Height; y += 64) {
            for (int x = 0; x < Width; x += 64) {
                // if (y < 128 && x < 256) continue;
                AddGameObject(new WaterSprite(_context, new(16, 16)) {
                    X = x,
                    Y = y,
                    Z = 0,
                    Width = 64,
                    Height = 64,
                });
            }
        }

        AddGameObject(new Clock(_context));
    }

    public override void Cleanup() {
        foreach (var gameobject in GameObjects) {
            RemoveGameObject(gameobject);
        }
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        base.Update(e);
        SetColorBasedOnTime();
    }

    #endregion
}