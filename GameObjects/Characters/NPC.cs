
using SharpSDL3;
using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters; 
public class Npc : Entity {

    public override AnimatedSprite32 Sprite { get; set; }

    public override string EntityType => "NPC";

    public override float Width => 24;

    public override float Height => 48;

    public Npc(string? name, nint rendererPtr) {
        Name = name;
        RendererPtr = rendererPtr;
        Weapon = Weapons.None;
        Defense = Defenses.None;
        Sprite = AnimatedSprite32.BlankSprite; // Initialize the non-nullable property
    }

    public override void Initialize() {

    }

    public override void Attack() {

    }

    public override void Defend() {

    }

    public override void Draw() {
        Core.SetRenderColor(RendererPtr, Colors.Colors.Pink);
        _ = Sdl.RenderFillRect(RendererPtr, ref PBody);
    }
}
