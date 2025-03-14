using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SDL2;

namespace TestGame.GameObjects
{
    internal class SoundEffect : Audio {
        private static readonly Logger? Log = LogManager.GetCurrentClassLogger();

        ~SoundEffect()
        {
            Mixer.FreeChunk(Pointer);
        }


        /// <inheritdoc />
        public SoundEffect(string name, string filename) {
            Name = name;
            Filename = filename;

            OpenAudioDevice();
            
            Pointer = Mixer.LoadWav(filename);
            
            if (Pointer == nint.Zero)
            {
                Log?.Error(new FileLoadException(nameof(filename)), $"Could not load Sound Effect \"{filename}\"; ${SDL.GetError()}");
                return;
            }

            Log?.Info(
                $"Loaded Audio: \"{filename}\"; Freq: {FileSupported.Frequency}; Format: {(AudioFormat)FileSupported.Format}; Channels: {FileSupported.Channels}");
        }

        private int GetChunkSize() {
            if (Filename is null) return 0;
            
            using var br = new BinaryReader(File.OpenRead(Filename));
            br.BaseStream.Seek(40, SeekOrigin.Begin);
            uint dataSize = br.ReadUInt32();
            return (int)dataSize;
        }

        #region Overrides of Audio

        /// <inheritdoc />
        public override void Play() {
            Play(0);
        }

        public void Play(int loops)
        {
            SetVolume(PreviousVolume);
            Mixer.PlayChannel(-1, Pointer, loops);
            IsPlaying = true;
        }

        /// <inheritdoc />
        public override void SetVolume(int volume) {
            volume = Math.Clamp(volume, 0, 128);
            PreviousVolume = Mixer.Volume(-1, volume);
        }

        /// <inheritdoc />
        public override int GetVolume() {
            return Mixer.Volume(-1, -1);
        }

        #endregion
    }
}
