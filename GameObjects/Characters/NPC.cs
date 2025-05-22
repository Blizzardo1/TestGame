
using SharpSDL3;
using TestGame.GameObjects.Items;

namespace TestGame.GameObjects.Characters; 
public class Npc : Entity {

    public override AnimatedSprite32 Sprite { get; set; }

    public override string EntityType => "NPC";

    public override float Width => 24;

<<<<<<< HEAD
    public override float Height => 48;
=======
    public override float Y { get; set; }

    public override float Z { get; set; }

    public override float Width => 24;

    public override float Height => 48;

    public override string? Name { get; protected set; }
>>>>>>> b06930d (Included Global UnmanagedTypes for Strings and Bools)

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
<<<<<<< HEAD
<<<<<<< HEAD
        _ = Sdl.RenderFillRect(RendererPtr, ref PBody);
=======
        _ = Render.RenderFillRect(RendererPtr, ref _body);
>>>>>>> b06930d (Included Global UnmanagedTypes for Strings and Bools)
=======
        _ = Sdl.RenderFillRect(RendererPtr, ref _body);
>>>>>>> 833ffb1 (SDL3 Migration almost complete)
    }
}
