using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.GameObjects {
    internal class MenuButton : Button {
        public MenuButton(GameContext context) : base(context) {
            BackgroundColor = BackgroundColor with { B = 0, G = 0, R = 0 };
            HighlightColor = HighlightColor with { B = 255, G = 255, R = 255 };
            ClickedColor = ClickedColor with { B = 190, G = 190, R = 190 };
        }
    }
}