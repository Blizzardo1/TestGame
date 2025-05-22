
using SharpSDL3;
using SharpSDL3.Structs;
using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters; 
public class Enemy : Entity {
    public override IWeapon Weapon { get; set; }

    public override IDefensive Defense { get; set; }

    public override int AttackPower { get; set; } = 1;

    public override bool CanSwim { get; set; } = true;

    public override bool CanAttack { get; set; } = true;

    public override bool CanDefend { get; set; } = true;

    public override bool CanClimb { get; set; } = true;

    public override bool CanMove { get; set; } = true;

    public override bool CanJump { get; set; } = true;

    public override bool IsOverWater { get; set; }

    public override bool IsOverGround { get; set; }

    public override bool IsInvincible { get; set; }

    public override AnimatedSprite32 Sprite { get; set; }

    public override string EntityType => "Enemy";

    public override float X { get; set; }

    public override float Y { get; set; }

    public override float Z { get; set; }

    public override float Width => 24;

    public override float Height => 48;

    public override string? Name { get; protected set; }

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
        _ = Sdl.RenderFillRect(RendererPtr, ref _body);
        Core.SetRenderColor(RendererPtr, Colors.Colors.White);
        FRect r = HitBox;
        _ = Sdl.RenderRect(RendererPtr, ref r);
    }

    public override void Update(Event e) {
        base.Update(e);
        Z = Y;
        HitBox = _body with { H = Height / 2 };
    }
}
