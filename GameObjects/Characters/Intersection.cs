namespace TestGame.GameObjects.Characters; 
public class Intersection {
    public enum IntersectionTypes { // Renamed to match the required naming convention  
        None,
        Point,
        Line,
        Rectangle,
        Circle
    }

    [Flags]
    public enum Directionals { // Renamed to match the required naming convention  
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8
    }

    public IntersectionTypes Type { get; set; } = IntersectionTypes.None;
    public Directionals Direction { get; set; } = Directionals.None;
}
