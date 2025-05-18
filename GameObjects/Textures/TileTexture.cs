namespace TestGame.GameObjects.Textures; 
public class TileTexture : Texture {
    public uint Id { get; private set; }
    public TileTexture(uint id, nint rendererPtr, nint texture, int width, int height)
        : base(rendererPtr, width, height) {
        TexturePtr = texture;
        Id = id;
    }

    public override void Initialize() {
        // No initialization needed for TileTexture
    }
}
