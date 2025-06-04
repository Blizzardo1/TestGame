

using SharpSDL3.Structs;

namespace TestGame.GameObjects.Items;

public class EmptyWeapon : IWeapon {

    public float X => 0;

    public float Y => 0;

    public float Z => 0;

    public float Width => 0;

    public float Height => 0;

    public string? Name => "Empty";

    public void Draw() {
        
    }

    public void Update(Event e) {

    }
}
