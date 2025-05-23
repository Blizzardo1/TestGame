
using SharpSDL3.Structs;
using TestGame.GameObjects;

namespace TestGame;

public class MenuItem : Button {
    public List< MenuItem > Children { get; set; }

    public int Id { get; set; }

    public FRect Position
    {
        get => new() { X = X, Y = Y, W = Width, H = Height };
        set
        {
            X = value.X;
            Y = value.Y;
            Width = (int)Math.Floor(value.W);
            Height = (int)Math.Floor(value.H);
        }
    }

    public Action Action { get; set; }

    public MenuItem(int id, string text, Action action, GameContext context) : base(context) {
        Id = id;
        Text = text;
        Action = action;
        Children = [];
        Flat = true;
    }
}