namespace TestGame.GameObjects.Items; 
public static class Defenses {
    public static IDefensive None => new EmptyDefense();
}
