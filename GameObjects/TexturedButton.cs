using TestGame.GameObjects.Textures;

namespace TestGame.GameObjects; 
internal class TexturedButton(GameContext context, Texture texture) : Button(context) {
    public Texture Texture { get; set; } = texture;

    public override void Draw() {
        base.Draw();
    }
}