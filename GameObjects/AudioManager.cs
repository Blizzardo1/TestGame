using NLog;
using SDL2;
using TestGame.Config;

namespace TestGame.GameObjects {
    public class AudioManager : IDisposable {
        private bool _initialized;
        private AudioSpec FileSupported;

        public bool DeviceOpened { get; private set; }
        public uint DeviceId { get; private set; }

        private static readonly Logger? _log = LogManager.GetCurrentClassLogger();
        private static AudioManager? _instance;
        public static AudioManager Instance {
            get {
                _instance ??= new AudioManager();
                return _instance;
            }
        }

        private AudioManager() {
            OpenAudioDevice();
        }

        //TODO: Audio not working properly......
        private void OpenAudioDevice() {
            FileSupported.Frequency = 48000;
            FileSupported.Format = (int)AudioFormat.Signed16MostSignedBit;
            //FileSupported.Callback = new AudioCallback(ProcessAudio);
            if (DeviceOpened) {
                int devopened = Mixer.QuerySpec(out FileSupported.Frequency, out FileSupported.Format, out int channels);
                FileSupported.Channels = (byte)channels;
                _log?.Debug($"Device Opened; Desired Audio: {FileSupported.Frequency}Hz; Channels: {FileSupported.Channels}; Samples: {FileSupported.Samples}; Size: {FileSupported.Size}; {FileSupported.Samples * 1000 / FileSupported.Frequency} bytes");
                return;
            }

            AudioDeviceConfig audioDeviceConfig = new(Engine.GameConfig?.AudioDeviceOutput ?? null!);
            string device = audioDeviceConfig.DeviceName ?? SDL.GetAudioDeviceName(0, 0);
            _log?.Debug($"Audio Device: {device}");
            DeviceId = SDL.OpenAudioDevice(device, 0,
                ref FileSupported,
                out AudioSpec obtained, (int)AudioAllow.All);
            DeviceOpened = DeviceId > 0;
            _log?.Debug($"Audio Device {(DeviceOpened ? "opened" : "not initialized")}; ID: {DeviceId}");
            _log?.Debug($"Desired Audio: {FileSupported.Frequency}Hz; Channels: {FileSupported.Channels}; Samples: {FileSupported.Samples}; Size: {FileSupported.Size}; {FileSupported.Samples * 1000 / FileSupported.Frequency} bytes");
            _log?.Debug($"Obtained Audio: {obtained.Frequency}Hz; Channels: {obtained.Channels}; Samples: {obtained.Samples}; Size: {obtained.Size}; {obtained.Samples * 1000 / obtained.Frequency} bytes");
            if (obtained.Channels > FileSupported.Channels) {
                FileSupported = obtained;
            }

            int res = Mixer.OpenAudio(FileSupported.Frequency, FileSupported.Format, 8, 2048);
            FileSupported.Channels += (byte)Mixer.AllocateChannels(8 - FileSupported.Channels);
            _log?.Debug($"Allocated {FileSupported.Channels} channels");
            if (res < 0) {
                _log?.Error($"Audio Device not opened; {SDL.GetError()}");
                return;
            }

            if (!DeviceOpened) {
                _initialized = false;
                SDL.CloseAudio();
                string msg = SDL.GetError();
                _log?.Error(!msg.IsEmpty() ? msg : "Audio Device not opened");
                return;
            }
            _initialized = true;
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="effectName">The name of the effect without the scope. e.g effect, not audio/effect</param>
        /// <param name="channel">Channel to play the sound effect on. -1 for any available channel.</param>
        /// <param name="loops">How many times we want to loop.</param>
        public void PlaySoundEffect(string effectName, int channel = -1, int loops = 0) {
            Task.Run(async () => {
                if (!_initialized) {
                    _log?.Error("AudioManager is not initialized.");
                    return;
                }

                if (!ResourceManager.TryGet($"audio/{effectName}", out SoundEffect? effect)) {
                    return;
                }

                effect!.Play(channel, loops);
            }).Wait();
        }

        public void Dispose() {
            Mixer.CloseAudio();
            _initialized = false;
        }
    }
}
