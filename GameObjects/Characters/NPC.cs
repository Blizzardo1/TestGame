using SDL2;
using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters; 
public class NPC : Entity {
    private int _hp;
    public override IWeapon Weapon { get; set; }

    public override IDefensive Defense { get; set; }

    public override int AttackPower { get; set; } = 0;

    public override bool CanSwim { get; set; } = true;

    public override bool CanAttack { get; set; } = true;

    public override bool CanDefend { get; set; } = true;


    public override bool CanClimb { get; set; } = true;
    
    public override bool CanMove { get; set; } = true;
    
    public override bool CanJump { get; set; } = true;

    public override bool IsOverWater { get; set; }

    public override bool IsOverGround { get; set; }

    public override bool IsInvincible { get; set; }


    public override AnimatedSprite32? Sprite { get; set; }

    public override string EntityType => "NPC";

    public override float X { get; set; }

    public override float Y { get; set; }

    public override float Z { get; set; }

    public override int Width => 24;

    public override int Height => 48;

    public override string? Name { get; protected set; } = "NPC";

    public NPC(string? name, nint rendererPtr) {
        Name = name;
        RendererPtr = rendererPtr;
        Weapon = Weapons.None;
        Defense = Defenses.None;
    }

    public override void Initialize() {

    }

    public override void Attack() {

    }

    public override void Defend() {

    }

    public override void Draw() {
        Core.SetRenderColor(RendererPtr, Colors.Colors.Pink);
        _ = SDL.RenderFillRect(RendererPtr, ref _body);
    }

    public override void Update(Event e) {
        base.Update(e);
    }
}
