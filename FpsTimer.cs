using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame {
    internal class FpsTimer {
        private bool _running;
        private bool _paused;
        private uint _startTicks;
        private uint _pausedTicks;


        public bool Running => _running;
        public bool Paused => _paused;

        public void Start() {
            _running = true;
            _paused = false;
            _startTicks = SDL.GetTicks();
        }

        public void Stop() {
            _running = false;
            _paused = false;
            _startTicks = 0;
            _pausedTicks = 0;
        }

        public void Pause() {
            if (_running && !_paused) {
                _paused = true;
                _pausedTicks = SDL.GetTicks() - _startTicks;
            }
        }

        public void Unpause() {
            if (_running && _paused) {
                _paused = false;
                _startTicks = SDL.GetTicks() - _pausedTicks;
                _pausedTicks = 0;
            }
        }

        public uint GetTicks() {
            if(!_running) {
                return 0;
            }

            if (_paused) {
                return _pausedTicks;
            }

            return SDL.GetTicks() - _startTicks;
        }
    }
}
