using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGame.GameObjects;

namespace TestGame.Scenes {
    internal class SceneFactory {
        public static T? CreateScene< T >(string? type, GameContext context) where T : Scene =>
            Activator.CreateInstance(Type.GetType(type ?? "TestGame.Scenes.Scene") ?? typeof(T), context) is not T o
                ? default
                : o;
    }
}