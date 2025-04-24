using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.GameObjects.Characters {
    public class Enemy : ICharacter {
        private int _hp;
        public IWeapon Weapon => throw new NotImplementedException();

        public IDefensive Defense => throw new NotImplementedException();

        public bool CanSwim => true;

        public bool CanAttack => true;

        public bool CanDefend => true;

        public bool CanDie => true;

        public bool CanClimb => true;

        public string EntityType => "Enemy";

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public int Width => 32;

        public int Height => 64;

        public string? Name => "Judge";

        public void Attack() {

        }

        public void Defend() {

        }

        public void Die() {
            if (_hp < 0) _hp = 0;
            // Death animation
        }

        public void Draw() {

        }

        public void Duck() {

        }

        public void Heal(int hp) {
            _hp += hp;
        }

        public bool IsClimbing() {
            return false;
        }

        public bool IsSwimming() {
            return false;
        }

        public void Jump() {

        }

        public void TakeDamage(int damage) {
            _hp -= damage;
        }

        public void Update(Event e) {

        }
    }
}
