using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.GameObjects.Characters {
    public class Enemy : Entity {
        private int _hp;
        public override IWeapon Weapon { get; set; }

        public override IDefensive Defense { get; set; }

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

        public override string EntityType => "Enemy";

        public override float X { get; set; }

        public override float Y { get; set; }

        public override float Z { get; set; }

        public override int Width => 24;

        public override int Height => 48;

        public override string? Name { get; protected set; } = "Judge";

        private Rect _rect;

        public Enemy(nint rendererPtr) {
            RendererPtr = rendererPtr;
        }

        public override void Initialize() {

        }

        public override void Attack() {

        }

        public override void Defend() {

        }

        public override void Draw() {
            Core.SetRenderColor(RendererPtr, Colors.Colors.Red);
            _ = SDL.RenderFillRect(RendererPtr, ref _rect);
            Core.SetRenderColor(RendererPtr, Colors.Colors.White);
            Rect r = HitBox;
            _ = SDL.RenderDrawRect(RendererPtr, ref r);
        }

        public override void Update(Event e) {
            _rect = new() {
                X = (int)X,
                Y = (int)Y,
                W = Width,
                H = Height
            };
            Z = Y;
            HitBox = _rect with { H = Height / 2 };
        }
    }
}
