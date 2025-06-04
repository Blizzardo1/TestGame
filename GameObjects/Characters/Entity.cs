
using SharpSDL3.Structs;
using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters; 
public abstract class Entity : Renderer, ICharacter {
    private static readonly float _speed = 0.00125f;

    public FRect HitBox { get; protected set; }

    public int HP { get; set; }
    public int MaxHP { get; set; }
    public float VelocityX { get; set; } = _speed;
    public float VelocityY { get; set; } = _speed;

    public abstract IWeapon Weapon { get; set; }

    public abstract IDefensive Defense { get; set; }

    public abstract int AttackPower { get; set; }

    public abstract bool CanSwim { get; set; }

    public abstract bool CanAttack { get; set; }

    public abstract bool CanDefend { get; set; }

    public abstract bool CanClimb { get; set; }

    public abstract bool CanMove { get; set; }

    public abstract bool CanJump { get; set; }
    public abstract bool IsOverWater { get; set; }
    public abstract bool IsOverGround { get; set; }

    public abstract bool IsInvincible { get; set; }

    public abstract AnimatedSprite32 Sprite { get;set; }

    public abstract string EntityType { get; }

    public abstract float X { get; set; }

    public abstract float Y { get; set; }

    public abstract float Z { get; set; }

    public abstract float Width { get; }

    public abstract float Height { get; }

    public abstract string? Name { get; protected set; }

    protected FRect _body;

    public FRect Body => _body;

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
        HP += hp;
        if (HP > MaxHP) {
            HP = MaxHP;
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
        
        if (x > 0) {
            Direction |= Direction.Right;
        } else if (x < 0) {
            Direction |= Direction.Left;
        } else if (y > 0) {
            Direction |= Direction.Down;
        } else if (y < 0) {
            Direction |= Direction.Up;
        }

        X += x * _speed;
        Y += y * _speed;
    }

    public void SetName(string name) {
        Name = name;
    }

    public void TakeDamage(int damage) {
        if (IsInvincible) {
            return;
        }
        if(HP <= 0) {
            HP = 0;
            return;
        }

        HP -= damage;
    }
    public virtual void Update(Event e) {
        _body = _body with {
            X = (int)X,
            Y = (int)Y,
            W = Width,
            H = Height
        };
    }
}
