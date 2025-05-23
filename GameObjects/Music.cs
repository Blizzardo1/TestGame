

using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Mixer;

namespace TestGame.GameObjects; 
internal class Music : Audio {
    private static readonly Log _log = Log.GetCurrentClassLogger(LogCategory.Audio);
    private SharpSDL3.Mixer.Music MusicPtr;

    ~Music() {
        Mixer.FreeMusic(MusicPtr);
    }

    /// <inheritdoc />
    public Music(string name, string filename) {
        Name = name;
        // #TODO: Does not load MP3 nor WAV? What other files don't load?

        MusicPtr = Mixer.LoadMusic(filename);
        if (Pointer.AudioBuffer == nint.Zero) {
            _log.Error(new FileNotFoundException(), $"Could not load audio file {filename}; ${Sdl.GetError()}");
        }            
    }

    #region Overrides of Audio

    /// <inheritdoc />
    public override void Play() {
        if (Mixer.PlayMusic(MusicPtr, -1)) {
            _log.Error(new Exception(Sdl.GetError()).Message);
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