using SDL2;

namespace TestGame.GameObjects; 
internal class Music : Audio {
    private static readonly Logger? _log = Logger.GetCurrentClassLogger(LogCategory.Audio);
    ~Music() {
        Mixer.FreeMusic(Pointer);
    }

    /// <inheritdoc />
    public Music(string name, string filename) {
        Name = name;
        // TODO: Does not load MP3 nor WAV? What other files don't load?

        Pointer = Mixer.LoadMusic(filename);
        if (Pointer == nint.Zero) {
            _log?.Error(new FileNotFoundException(), $"Could not load audio file {filename}; ${SDL.GetError()}");
            return;
        }            
    }

    #region Overrides of Audio

    /// <inheritdoc />
    public override void Play() {
        if (Mixer.PlayMusic(Pointer, -1) == -1) {
            _log?.Error(new Exception(SDL.GetError()).Message);
        }
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