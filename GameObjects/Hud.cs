using SharpSDL3;
using SharpSDL3.Structs;
using TestGame.GameObjects.Characters;

namespace TestGame.GameObjects;

public class Hud(Player player) : GameObject {
    private FRect _playerHealthRect;
    private readonly Player _p = player;
    private readonly TextString _healthText = new (OpenFont(), 18, "Health");

    public override void Draw() {
        _playerHealthRect = new FRect { X = Width - 80, Y = 10, W = 100, H = 16 };
        Core.SetRenderColor(RendererPtr, Colors.Colors.Red);
        _ = Sdl.RenderFillRect(RendererPtr, ref _playerHealthRect);
        Core.SetRenderColor(RendererPtr, Colors.Colors.White);
        _healthText.Draw();
    }

    public override void Initialize() {
        _healthText.ForegroundColor = Colors.Colors.White;
        _healthText.ShadowColor = Colors.Colors.Black;
        _healthText.ShowShadow = true;
        Width = Engine.Game!.Width;
        Height = Engine.Game.Height;
        RendererPtr = Engine.Game.GetRenderer();
    }

    public override void Update(Event e) {
        Width = Engine.Game!.Width;
        Height = Engine.Game.Height;
        _healthText.Text = $"Health {_p.Hp}";
        _healthText.X = _playerHealthRect.X + 5;
        _healthText.Y = _playerHealthRect.Y;
    }
}
