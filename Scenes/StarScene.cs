using NLog;
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGame.GameObjects;

namespace TestGame.Scenes {
    public class StarScene : Scene {
        private string[]? _data;
        private Logger _log;
        private float _centerX;
        private float _centerY;

        public StarScene(GameContext context, string name)
            : base(context, name) {
            _log = LogManager.GetCurrentClassLogger();
            _log.Info("StarScene()");
        }

        public override void Initialize() {
            _centerX = Width / 2;
            _centerY = Height / 2;
            // _log.Info($"Width: {Width}, Height: {Height}, Center Point ({_centerX}, {_centerY})");
            _data = File.ReadAllLines("star_data.csv");
            foreach (string line in _data.Skip(1)) {
                string[] l = line.Split(',');
                int id = Convert.ToInt32(l[ 0 ]);
                float distance = Convert.ToSingle(l[ 1 ]);
                int numOfStars = Convert.ToInt32(l[ 2 ]);
                float xPos = Convert.ToSingle(l[ 3 ]) + _centerX;
                float yPos = Convert.ToSingle(l[ 4 ]) + _centerY;
                float zPos = Convert.ToSingle(l[ 5 ]);
                int hue = Convert.ToInt32(l[ 6 ]);
                int sat = Convert.ToInt32(l[ 7 ]);
                int lum = Convert.ToInt32(l[ 8 ]);
                float alpha = Convert.ToSingle(l[ 9 ]);
                Star star = new Star(id, distance, numOfStars,
                    xPos, yPos, zPos, hue, sat, lum, alpha);
                star.Initialize(RendererPtr);
                AddGameObject(star);
            }
        }

        public override void Cleanup() {
            foreach (var gameObject in GameObjects) {
                RemoveGameObject(gameObject);
            }
        }
    }
}