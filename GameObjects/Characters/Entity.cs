using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.GameObjects.Characters {
    public abstract class Entity : Renderer, ICharacter {
        private float _speed = 0.000625f;

        public Rect HitBox { get; protected set; }

        protected int HP { get; set; }
        protected int MaxHP { get; set; }
        public abstract IWeapon Weapon { get; set; }

        public abstract IDefensive Defense { get; set; }

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

        public abstract int Width { get; }

        public abstract int Height { get; }

        public abstract string? Name { get; protected set; }

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
            X += x * _speed;
            Y += y * _speed;
        }
        public void TakeDamage(int damage) {
            if (IsInvincible) {
                return;
            }
            HP -= damage;
        }
        public abstract void Update(Event e);
    }
}
