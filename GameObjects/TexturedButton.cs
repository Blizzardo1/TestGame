using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGame.GameObjects.Textures;

namespace TestGame.GameObjects {
    internal class TexturedButton(GameContext context, Texture texture) : Button(context) {

        public Texture Texture { get; set; } = texture;

        public override void Draw() {
            base.Draw();
        }
    }
}
