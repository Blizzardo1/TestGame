using SDL2;

namespace TestGame.GameObjects; 
internal class Text(GameContext context, string fontFile, int size, string text) : GameObject {
    private string? _text;
    private Color _color;

    public override void Initialize() {
        RendererPtr = context.RendererPtr;

        GetFont(fontFile, size);
        _text = text;
        _color = context.Color;
    }

    public override void Draw() {
        RenderText(_text, (int)X, (int)Y, _color);
    }

    public override void Update(Event e) { }
}