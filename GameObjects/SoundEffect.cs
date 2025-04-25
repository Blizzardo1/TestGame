using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SDL2;

namespace TestGame.GameObjects {
    public class SoundEffect : Audio {
        private static readonly Logger? Log = LogManager.GetCurrentClassLogger();

        private int currentChannel = -1;

        private bool _loaded;

        ~SoundEffect() {
            Mixer.FreeChunk(Pointer);
        }

        /// <inheritdoc />
        public SoundEffect(string name, string filename) {
            Name = name;
            Filename = filename;

            // OpenAudioDevice();

            // Pointer = Mixer.LoadWav(filename);
            // Pointer = SDL.NewAudioStream((ushort)AudioFormat.Signed16LeastSignificantBit, 2,
            //     48000, (ushort)AudioFormat.Signed16LeastSignificantBit, 4, 48000);

            // New Mix_Chunk*
            if(!AudioManager.Instance.DeviceOpened) {
                Log?.Error($"Device not opened or initialized!");
                return;
            }

            Pointer = Mixer.LoadWav(filename);
            
            if (Pointer == nint.Zero) {
                Log?.Error(new FileLoadException(nameof(filename)),
                    $"Could not load Sound Effect \"{filename}\"; ${SDL.GetError()}");
                return;
            }
            Log?.Debug($"Loaded Audio \"{Filename}\" on Device {AudioManager.Instance.DeviceId}");
            _loaded = true;
        }

        #region Overrides of Audio

        /// <inheritdoc />
        public override void Play() {
            Play(-1, 0);
        }

        public void Play(int channel, int loops) {
            SetVolume(PreviousVolume);

            if (Pointer == nint.Zero) {
                Log?.Error(new FileLoadException(nameof(Pointer)),
                    $"Could not play Sound Effect \"{Filename}\"; ${SDL.GetError()}");
                return;
            }
            // TODO: Need to try and play sound effects globally;

            int channels = Mixer.AllocateChannels(1);
            if (channels == 0) {
                Log?.Error(new FileLoadException(nameof(channels)),
                    $"Unable to allocate channels for \"{Filename}\"; ${SDL.GetError()}");
                return;
            }


            currentChannel = Mixer.PlayChannel(channel, Pointer, loops);

            if(currentChannel < 0) {
                Log?.Error(new FileLoadException(nameof(currentChannel)),
                    $"Unable to play sound effect \"{Filename}\"; ${SDL.GetError()}");
                return;
            }

            Log?.Debug($"Playing Sound Effect \"{Filename}\"");
            IsPlaying = true;
        }

        public override void Stop() {
            if (Pointer == nint.Zero || currentChannel < 0) return;
            int res = Mixer.HaltChannel(currentChannel);
            if (res < 0) {
                Log?.Error($"Unable to stop sound effect \"{Filename}\"; ${SDL.GetError()}");
                return;
            }
            currentChannel = -1;
        }

        /// <inheritdoc />
        public override void SetVolume(int volume) {
            volume = Math.Clamp(volume, 0, 128);
            PreviousVolume = Mixer.Volume(currentChannel, volume);
        }

        /// <inheritdoc />
        public override int GetVolume() {
            return Mixer.Volume(currentChannel, -1);
        }

        public override void Resume() {
            Mixer.Resume(currentChannel);
        }

        public override void Pause() {
            Mixer.Pause(currentChannel);
        }

        #endregion
    }
}