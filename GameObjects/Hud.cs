using SharpSDL3;
using SharpSDL3.Structs;
using TestGame.GameObjects.Characters;

namespace TestGame.GameObjects;

public class Hud : GameObject {
    private FRect _playerHealthRect;
    private readonly Player p;
    private readonly TextString _healthText;

    public Hud(Player player) {
        p = player;
        // HORRIBLE HACK BECAUSE I NEED THE FUCKING RENDERER ONLY...
        RendererPtr = p.GetRenderer();
        _healthText = new TextString(new GameContext(nint.Zero, RendererPtr, new(), new(), Engine.Game!), GetFontStatic(), 18, "Health");
    }

    public override void Draw() {

        _playerHealthRect = new() { X = Width - 80, Y = 10, W = 100, H = 16 };
        Core.SetRenderColor(RendererPtr, Colors.Colors.Red);
        _ = Sdl.RenderFillRect(RendererPtr, ref _playerHealthRect);
        Core.SetRenderColor(RendererPtr, Colors.Colors.White);
        _healthText.Draw();
    }

    public override void Initialize() {
        _healthText.ForegroundColor = Colors.Colors.White;
        _healthText.ShadowColor = Colors.Colors.Black;
        _healthText.ShowShadow = true;
        _healthText.SetPosition(Width - 80, 10);
    }

    public override void Update(Event e) {
        _healthText.Text = $"Health {p.HP}";
        _healthText.SetPosition(Width - 80, 10);
    }
}
