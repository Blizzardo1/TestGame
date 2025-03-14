using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGame.GameObjects;

namespace TestGame.Scenes {
    public class TiledScene(GameContext context, string name) : Scene(context, name) {

        public override void Initialize() {
            GameContext c = context with { Rect = new() { X = 100, Y = 100, W = 90, H = 25 } };
            Button b = new(c) {
                Text = "Test"
            };

            
            AddGameObject(new Maps.Map(context, "Maps/Test.tmx"));
            AddGameObject(b);
        }

        public override void Cleanup() {
            foreach (var gameObject in GameObjects) {
                RemoveGameObject(gameObject);
            }
        }
    }
}
