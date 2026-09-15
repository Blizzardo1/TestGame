
using SharpSDL3.Structs;
using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters;

public abstract class Entity : Renderer, ICharacter {
    private const float Speed = 0.00125f;

    public FRect HitBox { get; protected set; }

    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public float VelocityX { get; set; } = Speed;
    public float VelocityY { get; set; } = Speed;

    public IWeapon? Weapon { get; set; }

    public IDefensive? Defense { get; set; }

    public int AttackPower { get; set; }

    public bool CanSwim { get; set; }

    public bool CanAttack { get; set; }

    public bool CanDefend { get; set; }

    public bool CanClimb { get; set; }

    public bool CanMove { get; set; }

    public bool CanJump { get; set; }
    public bool IsOverWater { get; set; }
    public bool IsOverGround { get; set; }

    public bool IsInvincible { get; set; }

    public abstract AnimatedSprite32 Sprite { get; set; }

    public abstract string EntityType { get; }

    public float X { get; set; }

    public float Y { get; set; }

    public float Z { get; set; }

    public abstract float Width { get; }

    public abstract float Height { get; }

    public string? Name { get; protected set; }

    protected FRect PBody;

    public FRect Body => PBody;

    public Direction Direction { get; set; } = Direction.None;

    public abstract void Initialize();
    public abstract void Attack();
    public abstract void Defend();
    public void Die() {
        if (IsInvincible) {
            return;
        }
        // Implement die logic here
    }
    public abstract void Draw();

    public void Duck() {
        if (!CanDefend) {
            return;
        }
        // Implement duck logic here
    }
    public void Heal(int hp) {
        if (IsInvincible) {
            return;
        }
        Hp += hp;
        if (Hp > MaxHp) {
            Hp = MaxHp;
        }
    }

    public bool IsClimbing() {
        if(!CanClimb) {
            return false;
        }
        // Implement climbing logic here
        // We need to check if the entity is over a climbable surface
        return IsOverGround;
    }

    public bool IsSwimming() {
        if(!CanSwim) {
            return false;
        }
        return IsOverWater;
    }
    public void Jump() {
        if (!CanJump) {
            return;
        }
        // Implement jump logic here
    }
    public void Move(float x, float y) {
        if(!CanMove) {
            return;
        }

        Direction = Direction.None;

        switch (x) {
            case > 0:
                Direction |= Direction.Right;
                break;
            case < 0:
                Direction |= Direction.Left;
                break;
            default: {
                switch (y) {
                    case > 0:
                        Direction |= Direction.Down;
                        break;
                    case < 0:
                        Direction |= Direction.Up;
                        break;
                }

                break;
            }
        }

        X += x * Speed;
        Y += y * Speed;
    }

    public void SetName(string name) {
        Name = name;
    }

    public void TakeDamage(int damage) {
        if (IsInvincible) {
            return;
        }
        if(Hp <= 0) {
            Hp = 0;
            return;
        }

        Hp -= damage;
    }
    public virtual void Update(Event e) {
        PBody = PBody with {
            X = (int)X,
            Y = (int)Y,
            W = Width,
            H = Height
        };
    }
}
