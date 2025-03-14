using NLog;
using SDL2;

namespace TestGame.GameObjects {
    public abstract class Audio : IGameObject {
        private static readonly Logger? _log = LogManager.GetCurrentClassLogger();
        protected static bool DeviceOpened;
        
        protected AudioSpec FileSupported;
        
        protected nint Pointer;
        
        protected int PreviousVolume = 0;

        protected string? Filename { get; init; }
        public string? Name { get; protected init; }
        
        public bool IsPlaying { get; protected set; }

        public abstract void Play();

        public abstract void SetVolume(int volume);

        public abstract int GetVolume();

        // Let's see if this ultimately works
        protected virtual void OpenAudioDevice() {
            if (DeviceOpened) return;
            // int chunkSize = GetChunkSize();
            _ = Mixer.QuerySpec(out FileSupported.Frequency, out FileSupported.Format, out int channels);
            bool opened = Mixer.OpenAudio(FileSupported.Frequency, FileSupported.Format, channels, 2048) == 0;
            _log?.Info($"{(opened ? "Audio Channels opened" : "Unable to open Audio Channels")}");

            int devOpened = Mixer.QuerySpec(out FileSupported.Frequency, out FileSupported.Format, out channels);

            _log?.Info($"{(devOpened == 1 ? "Audio Opened" : "Audio Closed")}");
            if (devOpened == 0) {
                _log?.Error("Audio is not opened. We cannot proceed loading the sound effect.");
                return;
            }

            FileSupported.Channels = (byte)channels;
            DeviceOpened = opened;
        }

        public virtual void Pause() {
            IsPlaying = false;
            Mixer.Pause(-1);
        }

        public virtual void Stop() {
            Mixer.CloseAudio();
        }
    }
}