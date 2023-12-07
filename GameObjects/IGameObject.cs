using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame.GameObjects;

public interface IGameObject {
    float X { get; }
    float Y { get; }

    int Width { get; }
    int Height { get; }

    string Name { get; }
    void Draw();
    void Update(Event e);
}