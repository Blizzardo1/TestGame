

using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Mixer;

namespace TestGame.GameObjects; 
public class SoundEffect : Audio {

    private static readonly Log Log = Log.GetCurrentClassLogger(LogCategory.Audio);

    private int _currentChannel = 1;

    ~SoundEffect() {
        Mixer.FreeChunk(Pointer);
    }

    /// <inheritdoc />
    public SoundEffect(string name, string filename) {
        Name = name;
        Filename = filename;

        // OpenAudioDevice();

        // Pointer = Mixer.LoadWav(filename);
        // Pointer = Sdl.NewAudioStream((ushort)AudioFormat.Signed16LeastSignificantBit, 2,
        //     48000, (ushort)AudioFormat.Signed16LeastSignificantBit, 4, 48000);


        Pointer = Mixer.LoadWav(filename);
        
        if (Pointer.AudioBuffer == nint.Zero) {
            Log.Error(new FileNotFoundException(), $"Could not load Sound Effect \"{filename}\"; ${Sdl.GetError()}");
            return;
        }
        Log.Debug($"Loaded Audio \"{Filename}\"");
    }

    #region Overrides of Audio

    /// <inheritdoc />
    public override void Play() {
        Play(-1, 0);
    }

    public void Play(int channel, int loops) {
        SetVolume(PreviousVolume);

        if (Pointer.AudioBuffer == nint.Zero) {
            Log.Error(new FileNotFoundException(),
                $"Could not play Sound Effect \"{Filename}\"; ${Sdl.GetError()}");
            return;
        }

        // #TODO: Need to try and play sound effects globally;

        _currentChannel = Mixer.PlayChannel(channel, Pointer, loops);

        if(_currentChannel < 0) {
            Log.Error($"Unable to play sound effect \"{Filename}\"; ${Sdl.GetError()}");
            return;
        }

        Log.Debug($"Playing Sound Effect \"{Filename}\"");
        IsPlaying = true;
    }

    public override void Stop() {
        // #TODO: Fix this function
        if (Pointer.AudioBuffer == nint.Zero || _currentChannel < 0) return;
        int res = 0;
        Mixer.HaltChannel(_currentChannel);
        if (res < 0) {
            Log.Error($"Unable to stop sound effect \"{Filename}\"; ${Sdl.GetError()}");
            return;
        }
        _currentChannel = -1;
    }

    /// <inheritdoc />
    public override void SetVolume(int volume) {
        volume = Math.Clamp(volume, 0, 128);
        PreviousVolume = Mixer.Volume(_currentChannel, volume);
    }

    /// <inheritdoc />
    public override int GetVolume() {
        return Mixer.Volume(_currentChannel, -1);
    }

    public override void Resume() {
        Mixer.Resume(_currentChannel);
    }

    public override void Pause() {
        Mixer.Pause(_currentChannel);
    }

    #endregion
}