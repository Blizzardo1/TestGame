using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Mixer;
using SharpSDL3.Structs;

namespace TestGame.GameObjects;

public class AudioManager : IDisposable {
    private bool _initialized;

    private static readonly Log Log = Log.GetCurrentClassLogger(LogCategory.Audio);

    private bool _disposed;

    private static AudioManager? _instance;
    public static AudioManager Instance {
        get {
            _instance ??= new AudioManager();
            return _instance;
        }
    }

    private AudioManager() {
        _disposed = false;
    }

    public void Initialize() {
        Mixer.MixInit result = Mixer.Initialize(Mixer.MixInit.Midi | Mixer.MixInit.Ogg | Mixer.MixInit.Flac | Mixer.MixInit.Mp3);
        if (result == Mixer.MixInit.None) {
            Log.Error($"Failed to initialize audio mixer: {Sdl.GetError()}");
            return;
        }
        Log.Info($"Initialized Audio Manager with {result}");
        OpenAudioDevice();
    }

    //#TODO: Implement a new way to handle Audio
    private void OpenAudioDevice() {
        Mixer.OpenAudio(AudioDeviceId.DefaultPlayback,
            new AudioSpec {
                Channels = 8,
                Format = SharpSDL3.Enums.AudioFormat.S16,
                Freq=48000
            });

        if(Mixer.MasterVolume(50) == -1) {
            Log.Error($"Failed to set master volume: {Sdl.GetError()}");
            return;
        }

        _initialized = true;
        Log.Info("Audio Manager opened an audio device - OK");
    }

    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    /// <param name="effectName">The name of the effect without the scope. e.g effect, not audio/effect</param>
    /// <param name="channel">Channel to play the sound effect on. Channel 1 is default base channel</param>
    /// <param name="loops">How many times we want to loop.</param>
    public void PlaySoundEffect(string effectName, int channel = 1, int loops = 0) {
        if (!_initialized) {
            Log.Error("AudioManager is not initialized.");
            return;
        }

        if (!ResourceManager.TryGet($"audio/{effectName}", out SoundEffect? effect)) {
            return;
        }

        effect!.Play(channel, loops);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;

        if (disposing) {
            // Dispose managed resources here
            _initialized = false;
            _disposed = true;
        }
    }

    public override bool Equals(object? obj) => obj is AudioManager && obj == this;

    public override int GetHashCode() {
        return HashCode.Combine(typeof(AudioManager).FullName);
    }
}
