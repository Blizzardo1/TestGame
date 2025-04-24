using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SDL2;

namespace TestGame.GameObjects {
    internal class Music : Audio {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        ~Music() {
            Mixer.FreeMusic(Pointer);
        }

        /// <inheritdoc />
        public Music(string name, string filename) {
            Name = name;
            // TODO: Does not load MP3 nor WAV? What other files don't load?

            Pointer = Mixer.LoadMusic(filename);
            if (Pointer == nint.Zero) {
                Log.Error(new FileLoadException(nameof(filename)),
                    $"Could not load audio file {filename}; ${SDL.GetError()}");
                return;
            }            
        }

        #region Overrides of Audio

        /// <inheritdoc />
        public override void Play() {
            if (Mixer.PlayMusic(Pointer, -1) == -1)
                Log.Error(new Exception(SDL.GetError()));
        }

        /// <inheritdoc />
        public override void SetVolume(int volume) {
            volume = Math.Clamp(volume, 0, 128);
            PreviousVolume = Mixer.VolumeMusic(volume);
        }

        /// <inheritdoc />
        public override int GetVolume() {
            return Mixer.VolumeMusic(-1);
        }

        public override void Resume() => throw new NotImplementedException();
        public override void Pause() => throw new NotImplementedException();
        public override void Stop() => throw new NotImplementedException();

        #endregion
    }
}