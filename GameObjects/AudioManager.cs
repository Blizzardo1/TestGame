using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Structs;
using TestGame.Config;

using SAudio = SharpSDL3.Audio;

namespace TestGame.GameObjects; 
public class AudioManager : IDisposable {
    private bool _initialized;
    private AudioSpec FileSupported;

    private static readonly Log? _log = Log.GetCurrentClassLogger(LogCategory.Audio);

    private bool _disposed;

    public bool DeviceOpened { get; private set; }
    public uint DeviceId { get; private set; }

    private static AudioManager? _instance;
    public static AudioManager Instance {
        get {
            _instance ??= new AudioManager();
            return _instance;
        }
    }

    private AudioManager() {
        _disposed = false;
        OpenAudioDevice();
    }

    //#TODO: Implement a new way to handle Audio
    private void OpenAudioDevice() {
        
        _initialized = true;
    }

    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    /// <param name="effectName">The name of the effect without the scope. e.g effect, not audio/effect</param>
    /// <param name="channel">Channel to play the sound effect on. -1 for any available channel.</param>
    /// <param name="loops">How many times we want to loop.</param>
    public void PlaySoundEffect(string effectName, int channel = -1, int loops = 0) {
        Task.Run(() => {
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
