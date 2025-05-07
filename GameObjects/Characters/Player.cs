using SDL2;
using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters; 
public class Player : Entity {
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

    public override string EntityType => "Player";

    public override float X { get; set; }

    public override float Y { get; set; }

    public override float Z { get; set; }

    public override int Width => 24;

    public override int Height => 48;

    public Rect HealthRect => new() {
        X = (int)X,
        Y = (int)Y,
        W = 100,
        H = 16
    };

    public override string? Name { get; protected set; }

    public Player(string? name, nint rendererPtr) {
        Name = name;
        RendererPtr = rendererPtr;
        Weapon = Weapons.None;
        Defense = Defenses.None;
        MaxHP = 100;
        Heal(100);
        Sprite = AnimatedSprite32.BlankSprite;
    }

    public override void Initialize() {

    }

    public override void Attack() {

    }

    public override void Defend() {

    }

    public override void Draw() {
        Core.SetRenderColor(RendererPtr, Colors.Colors.CornflowerBlue);
        _ = SDL.RenderFillRect(RendererPtr, ref _body);
        Rect r = HitBox;
        Core.SetRenderColor(RendererPtr, Colors.Colors.Red);
        _ = SDL.RenderDrawRect(RendererPtr, ref r);
    }

    public override void Update(Event e) {
        base.Update(e);
        Z = Y;
        HitBox = _body with { H = Height / 2 };
    }
}
