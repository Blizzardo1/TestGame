namespace TestGame.GameObjects.Characters; 
public class Intersection {
    public enum IntersectionType {
        None,
        Point,
        Line,
        Rectangle,
        Circle
    }

    public enum Directional {
        None,
        Up,
        Down,
        Left,
        Right
    }

    public IntersectionType Type { get; set; } = IntersectionType.None;
    public Directional Direction { get; set; } = Directional.None;

}
