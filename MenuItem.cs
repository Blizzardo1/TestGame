using SDL2;
using TestGame.Colors;
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

    public MenuItem(int id, string text, Action action, nint rendererPtr) : base(rendererPtr) {
        Id = id;
        Text = text;
        Action = action;
        Children = new List< MenuItem >();
        Flat = true;
        TextPosition = new Point { X = 1, Y = 1 };
    }
}
/*
internal class MenuItem : Button
{
   public Action? Action { get; init; }
   public List<MenuItem> Children { get; set; } = new();
   private Color _currColor;

   private bool _visible;
   public bool Visible
   {
       get => _visible;
       set => _visible = value;
   }

   public bool Parent { get; set; }

   /// <inheritdoc />
   public MenuItem(nint rendererPtr) : base(rendererPtr)
   {
       TextPosition = new Point { X = 4, Y = 0 };
       BackgroundColor = new Color { R = 240, G = 240, B = 240, A = 255 };
       HighlightColor = new Color { R = 32, G = 100, B = 132, A = 255 };
       ForegroundColor = new Color { R = 0, G = 0, B = 0, A = 255 };
       Flat = true;
       _visible = false;
       Click += MenuItemClicked;
       MouseEnter += MenuItemMouseEnter;
       MouseLeave += MenuItemMouseLeave;
   }

   private void MenuItemMouseLeave(object? sender, MouseMotionEvent e)
   {
       _currColor = BackgroundColor;
   }

   private void MenuItemMouseEnter(object? sender, MouseMotionEvent e)
   {
       _currColor = HighlightColor;
   }

   private void MenuItemClicked(object? sender, MouseButtonEvent e)
   {
       // Check to see if the mouse is in the vicinity
       int x1 = (int)X;
       int x2 = (int)(X + Width);
       int y1 = (int)Y;
       int y2 = (int)(Y + Height * 2);
       if (e.X >= x1 && e.X <= x2 && e.Y >= y1 && e.Y <= y2)
       {
           Console.WriteLine($"{Text} clicked; mX: {e.X}, mY: {e.Y} :: X: {X}, Y: {Y} :: W: {Width}, H: {Height}");
           if (Children.Count > 0)
           {
               foreach (MenuItem child in Children)
               {
                   child.Visible = !child.Visible;
               }
           }

           Action?.Invoke();
       }
       else
       {
           Children.ForEach(c => c.Visible = false);
       }

   }

   #region Overrides of Button

   /// <inheritdoc />
   public override void Draw()
   {
       if (!_visible && !Parent) return;
       base.Draw();
       if (Children.Count <= 0 && !_visible) return;
       var rect = new FRect { X = X + 8, Y = Y + Height, W = MeasureString(Children.LongestString()).Width, H = Height };

       _ = SDL.SetRenderDrawColor(RendererPtr, _currColor.R, _currColor.G, _currColor.B, _currColor.A);
       _ = SDL.RenderDrawRectF(RendererPtr, ref rect);
       RenderText(Text, (int)rect.X + 4, (int)rect.Y, ForegroundColor);
       foreach (MenuItem menuItem in Children)
       {
           menuItem.Draw();
       }
   }

   /// <inheritdoc />
   public override void Update(Event e)
   {
       if (!_visible && !Parent) return;

       MouseMotionEvent mm = e.Type switch
       {
           EventType.MouseMotion => e.Motion,
           _ => new MouseMotionEvent()
       };

       MouseButtonEvent mb = e.Type switch
       {
           EventType.MouseButtonDown => e.Button,
           EventType.MouseButtonUp => e.Button,
           _ => new MouseButtonEvent()
       };

       if (mm.X > X || mm.X < X + Width && mm.Y > Y || mm.Y < Y + Height)
       {
           _currColor = HighlightColor;
           ForegroundColor = KnownColor.White.ToColor();
       }
       else
       {
           _currColor = BackgroundColor;
           ForegroundColor = KnownColor.Black.ToColor();
       }


       base.Update(e);
   }

   #endregion
}*/