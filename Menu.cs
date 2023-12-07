using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGame.Colors;
using TestGame.GameObjects;

namespace TestGame;

public class Menu : Renderer, IGameObject {
    private List< MenuItem > menuItems;

    private FRect _rect;

    /// <inheritdoc />
    public float X => 0;

    /// <inheritdoc />
    public float Y => 0;

    /// <inheritdoc />
    public int Width
    {
        get => (int)_rect.W;
        set => _rect.W = value;
    }

    /// <inheritdoc />
    public int Height
    {
        get => (int)_rect.H;
        set => _rect.H = value;
    }

    /// <inheritdoc />
    public string Name => "Menu";

    /// <inheritdoc />
    public void Draw() {
        _ = SDL.SetRenderDrawColor(RendererPtr, 240, 240, 240, 255); // Set background color
        _ = SDL.RenderFillRectF(RendererPtr, ref _rect);

        // Render each menu item
        menuItems.ForEach(mi => mi.Draw());

        // Border Lines
        _ = SDL.SetRenderDrawColor(RendererPtr, 0, 0, 0, 255); // Black
        _ = SDL.RenderDrawLineF(RendererPtr, X, Y + Height, Width, Y + Height);

        _ = SDL.SetRenderDrawColor(RendererPtr, 190, 190, 190, 255); // Gray
        _ = SDL.RenderDrawLineF(RendererPtr, X, Y + Height - 1, Width, Y + Height - 1);
    }

    /// <inheritdoc />
    public void Update(Event e) {
        menuItems.ForEach(mi => mi.Update(e));
    }

    public Menu(IntPtr renderer) {
        RendererPtr = renderer;
        menuItems = new List< MenuItem >();
        _ = SDL.GetRendererOutputSize(renderer, out int w, out _);
        Width = w;
        Height = 24;
        _rect = new FRect { X = 0, Y = 0, W = Width, H = Height };
        Initialize(RendererPtr);
    }

    public void AddMenuItem(string text, int id, Action action) {
        int w = MeasureString(text).Width + 8;
        float x = menuItems.Count > 0 ? menuItems[ ^1 ].X + menuItems[ ^1 ].Width + 2 : 0;
        Console.WriteLine($"MenuItem::{text} X: {x}, Width: {w}");
        menuItems.Add(new MenuItem(id, text, action, RendererPtr)
            { Position = new FRect { X = x, Y = 0, W = w, H = Height } });
    }
}

/**
internal class Menu : Renderer, IGameObject
{
    private readonly List<MenuItem> _items;

    #region Implementation of IGameObject

    /// <inheritdoc />
    public float X => 0;

    /// <inheritdoc />
    public float Y => 0;

    /// <inheritdoc />
    public int Width
    {
        get => (int)_rect.W;
        set => _rect.W = value;
    }

    /// <inheritdoc />
    public int Height
    {
        get => (int)_rect.H;
        set => _rect.H = value;
    }

    private FRect _rect;

    /// <inheritdoc />
    public string Name { get; set; } = "Menu";

    public Menu(nint rendererPtr)
    {
        Initialize(rendererPtr);
        InitializeComponents();
        Height = 24;
        _items = new List<MenuItem>();
    }

    private void InitializeComponents()
    {
        nint windowPtr = SDL.GetWindowFromID(Game.WindowId);
        SDL.GetWindowSize(windowPtr, out int width, out int _);
        Width = width;
    }

    public void AddMenuItem(string text, Action action, params MenuItem[]? items)
    {
        var mi = new MenuItem(RendererPtr) { Parent = true, Visible = true, Text = text, Action = action, Height = Height - 1 };
        mi.Click += MenuItem_Click;
        mi.Children = items?.ToList() ?? new List<MenuItem>();

        for (int index = 0; index < mi.Children.Count; index++)
        {
            MenuItem m = mi.Children[index];
            m.X = X;
            m.Y = m.Y * index + Height;
        }

        _items.Add(mi);
        for (int index = 0; index < _items.Count; index++)
        {
            MenuItem i = _items[index];
            i.Width = MeasureString(mi.Text).Width + 8;
            i.X = index * i.Width;
        }

        _rect = _rect with { X = X, Y = Y };
    }

    private void MenuItem_Click(object? sender, MouseButtonEvent e)
    {
        if (sender is not MenuItem mi) return;
        if (e.X < mi.X || e.X > mi.X + mi.Width && e.Y < mi.Y || e.Y > mi.Y + mi.Height) return;
        mi.Action?.Invoke();
    }

    /// <inheritdoc />
    public void Draw()
    {
        // Background
        _ = SDL.SetRenderDrawColor(RendererPtr, 240, 240, 240, 255); // White
        _ = SDL.RenderFillRectF(RendererPtr, ref _rect);

        _items.ForEach(mi => mi.Draw());

        // Border Lines
        _ = SDL.SetRenderDrawColor(RendererPtr, 0, 0, 0, 255); // Black
        _ = SDL.RenderDrawLineF(RendererPtr, X, Y + Height, Width, Y + Height);

        _ = SDL.SetRenderDrawColor(RendererPtr, 190, 190, 190, 255); // Gray
        _ = SDL.RenderDrawLineF(RendererPtr, X, Y + Height - 1, Width, Y + Height - 1);
    }

    /// <inheritdoc />
    public void Update(Event e)
    {
        _items.ForEach(mi => mi.Update(e));
    }

    #endregion
}*/