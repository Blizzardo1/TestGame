using SDL2;
using TestGame.GameObjects;

namespace TestGame;

public class Menu(GameContext context) : GameObject {
    private List< MenuItem > menuItems;
    private GameContext _context;

    public override void Initialize() {
        RendererPtr = context.RendererPtr;
        _context = context;
        menuItems = [];
        _ = SDL.GetRendererOutputSize(context.RendererPtr, out int w, out _);
        Width = w;
        Height = 24;
        frect = new FRect { X = 0, Y = 0, W = Width, H = Height };
        Initialize(RendererPtr);
    }

    /// <inheritdoc />
    public override void Draw() {
        _ = SDL.SetRenderDrawColor(RendererPtr, 240, 240, 240, 255); // Set background color
        _ = SDL.RenderFillRectF(RendererPtr, ref frect);

        // Render each menu item
        menuItems.ForEach(mi => mi.Draw());

        // Border Lines
        _ = SDL.SetRenderDrawColor(RendererPtr, 0, 0, 0, 255); // Black
        _ = SDL.RenderDrawLineF(RendererPtr, X, Y + Height, Width, Y + Height);

        _ = SDL.SetRenderDrawColor(RendererPtr, 190, 190, 190, 255); // Gray
        _ = SDL.RenderDrawLineF(RendererPtr, X, Y + Height - 1, Width, Y + Height - 1);
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        menuItems.ForEach(mi => mi.Update(e));
    }

    public void AddMenuItem(string text, int id, Action action) {
        int w = MeasureString(GetFont("default", 12), text).Width + 8;
        float x = menuItems.Count > 0 ? menuItems[ ^1 ].X + menuItems[ ^1 ].Width + 2 : 0;
        Console.WriteLine($"MenuItem::{text} X: {x}, Width: {w}");
        menuItems.Add(new MenuItem(id, text, action, _context)
            { Position = new FRect { X = x, Y = 0, W = w, H = Height } });
    }
}