using SharpSDL3;
using SharpSDL3.Structs;
using TestGame.GameObjects.Textures;

namespace TestGame.GameObjects;

public class AnimatedSprite : Sprite {
    private int _frame;

    private FRect[] _sourceRects;
    private FRect _currentFrame;

    /// <inheritdoc />
    public AnimatedSprite(GameContext context, string imagePath, FRect[] frames, int delay = 0)
        : base(context, imagePath) {
        _frame = 0;
        _sourceRects = frames;
        _currentFrame = _sourceRects[ _frame ];
        AddDelay(delay);
    }

    private void AddDelay(int delay) {
        if (delay == 0) return;

        var clone = new FRect[_sourceRects.Length];
        
        Array.Copy(_sourceRects, clone, _sourceRects.Length);
        Array.Resize(ref _sourceRects, delay * _sourceRects.Length);

        for (int y = 0; y < clone.Length; y++) {
            for (int x = 0; x < delay; x++) {
                _sourceRects[ x + y * delay ] = clone[ y ];
            }
        }
    }

    #region Overrides of Texture

    /// <inheritdoc />
    public override void Draw() {
        FRect rect = Rect;
        
        _ = Sdl.RenderTexture(RendererPtr, TexturePtr, ref _currentFrame, ref rect);
    }

    /// <inheritdoc />
    public override void Update(Event e) {
        base.Update(e);
        
        _frame = ++_frame % _sourceRects.Length;
        _currentFrame = _sourceRects[ _frame ];
    }

    #endregion
}