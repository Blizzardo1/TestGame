
using SharpSDL3;
using SharpSDL3.Structs;
using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters; 
public class Player : Entity {
   public override AnimatedSprite32 Sprite { get; set; }

    public override string EntityType => "Player";

    public override float Width => 24;

    public override float Height => 48;

    public Hud Hud { get; set; }

    public Player(string? name, nint rendererPtr) {
        Name = name;
        RendererPtr = rendererPtr;
        Weapon = Weapons.None;
        Defense = Defenses.None;
        MaxHp = 100;
        Heal(MaxHp);
        Sprite = AnimatedSprite32.BlankSprite;
        Hud = new Hud(this);
    }

    public override void Initialize() {
        Hud.Initialize();
    }

    public override void Attack() {
        if(Weapon is null) {
            return;
        }
        if (Hp > 0) {
            Weapon.Use();
        }
    }

    public override void Defend() {
        if (Defense is null) {
            return;
        }

        if (Hp > 0) {
            Defense.Use();
        }
    }

    public override void Draw() {
        Core.SetRenderColor(RendererPtr, Colors.Colors.CornflowerBlue);
        _ = Sdl.RenderFillRect(RendererPtr, ref PBody);
        FRect r = HitBox;
        Core.SetRenderColor(RendererPtr, Colors.Colors.Red);
        _ = Sdl.RenderRect(RendererPtr, ref r);
        Hud.Draw();
    }

    public override void Update(Event e) {
        base.Update(e);
        Z = Y;
        HitBox = PBody with { H = Height / 2 };
        Hud.Update(e);
    }
}
