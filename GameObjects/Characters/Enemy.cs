using SharpSDL3;
using SharpSDL3.Structs;
using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters;

public class Enemy : Entity {

    public override AnimatedSprite32 Sprite { get; set; }

    public override string EntityType => "Enemy";

    public override float Width => 24;

    public override float Height => 48;

    public Enemy(string? name, nint rendererPtr) {
        Name = name;
        RendererPtr = rendererPtr;
        Weapon = Weapons.None;
        Defense = Defenses.None;
        Sprite = AnimatedSprite32.BlankSprite;
    }

    public override void Initialize() {

    }

    public override void Attack() {

    }

    public override void Defend() {

    }

    public override void Draw() {
        Core.SetRenderColor(RendererPtr, Colors.Colors.Red);
        _ = Sdl.RenderFillRect(RendererPtr, ref PBody);
        Core.SetRenderColor(RendererPtr, Colors.Colors.White);
        FRect r = HitBox;
        _ = Sdl.RenderRect(RendererPtr, ref r);
    }

    public override void Update(Event e) {
        base.Update(e);
        Z = Y;
        HitBox = PBody with { H = Height / 2 };
    }
}
