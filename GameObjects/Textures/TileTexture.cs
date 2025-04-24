using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.GameObjects.Textures; 
public class TileTexture : Texture {
    public uint Id { get; private set; }
    public TileTexture(uint id, nint rendererPtr, nint texture, int width, int height)
        : base(rendererPtr, width, height) {
        TexturePtr = texture;
        Id = id;
    }
}
