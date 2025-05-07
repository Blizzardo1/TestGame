using TestGame.GameObjects;

namespace TestGame.Scenes; 
public class Hud(GameContext context) : Renderer {
    private readonly GameContext _context = context;

    public void Initialize() {
        RendererPtr = _context.RendererPtr;
    }

}
