
using SharpSDL3;
using SharpSDL3.Structs;
using TestGame.GameObjects;

namespace TestGame;

public class Menu(GameContext context) : GameObject {
    private List< MenuItem > menuItems = [];
    private readonly GameContext _context = context;

    public override void Initialize() {
        RendererPtr = _context.RendererPtr;
        menuItems = [];
        _ = Sdl.GetRenderOutputSize(_context.RendererPtr, out int w, out _);
        Width = w;
        Height = 24;
        frect = new FRect { X = 0, Y = 0, W = Width, H = Height };
        Initialize(RendererPtr);
    }

    /// <inheritdoc />
    public override void Draw() {
        _ = Sdl.SetRenderDrawColor(RendererPtr, 240, 240, 240, 255); // Set background color
        _ = Sdl.RenderFillRect(RendererPtr, ref frect);

        // Render each menu item
        menuItems.ForEach(mi => mi.Draw());

        // Border Lines
        _ = Sdl.SetRenderDrawColor(RendererPtr, 0, 0, 0, 255); // Black
        _ = Sdl.RenderLine(RendererPtr, X, Y + Height, Width, Y + Height);

        _ = Sdl.SetRenderDrawColor(RendererPtr, 190, 190, 190, 255); // Gray
        _ = Sdl.RenderLine(RendererPtr, X, Y + Height - 1, Width, Y + Height - 1);
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        menuItems.ForEach(mi => mi.Update(e));
    }

    public void AddMenuItem(string text, int id, Action action) {
        float w = MeasureString(GetFont("default"), text).Width + 8;
        float x = menuItems.Count > 0 ? menuItems[ ^1 ].X + menuItems[ ^1 ].Width + 2 : 0;
        Console.WriteLine($"MenuItem::{text} X: {x}, Width: {w}");
        menuItems.Add(new MenuItem(id, text, action, _context)
            { Position = new FRect { X = x, Y = 0, W = w, H = Height } });
    }
}