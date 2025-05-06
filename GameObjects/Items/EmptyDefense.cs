using SDL2;

namespace TestGame.GameObjects.Items;
public class EmptyDefense : IDefensive {
    public float X => 0;

    public float Y => 0;

    public float Z => 0;

    public int Width => 0;

    public int Height => 0;

    public string? Name => "Empty";

    public void Draw() {

    }

    public void Update(Event e) {

    }
}
