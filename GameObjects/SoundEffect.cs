

using SharpSDL3;
using SharpSDL3.Enums;

namespace TestGame.GameObjects; 
public class SoundEffect : Audio {

    private static readonly Log? _log = Log.GetCurrentClassLogger(LogCategory.Audio);

    private int currentChannel = -1;

    ~SoundEffect() {
        //Mixer.FreeChunk(Pointer);
    }

    /// <inheritdoc />
    public SoundEffect(string name, string filename) {
        Name = name;
        Filename = filename;

        // OpenAudioDevice();

        // Pointer = Mixer.LoadWav(filename);
        // Pointer = Sdl.NewAudioStream((ushort)AudioFormat.Signed16LeastSignificantBit, 2,
        //     48000, (ushort)AudioFormat.Signed16LeastSignificantBit, 4, 48000);

        // New Mix_Chunk*
        if(!AudioManager.Instance.DeviceOpened) {
            _log?.Error($"Device not opened or initialized!");
            return;
        }

        //Pointer = Mixer.LoadWav(filename);
        
        if (Pointer == nint.Zero) {
            _log?.Error(new FileNotFoundException(), $"Could not load Sound Effect \"{filename}\"; ${Sdl.GetError()}");
            return;
        }
        _log?.Debug($"Loaded Audio \"{Filename}\" on Device {AudioManager.Instance.DeviceId}");
    }

    #region Overrides of Audio

    /// <inheritdoc />
    public override void Play() {
        Play(-1, 0);
    }

    public void Play(int channel, int loops) {
        SetVolume(PreviousVolume);

        if (Pointer == nint.Zero) {
            _log?.Error(new FileNotFoundException(),
                $"Could not play Sound Effect \"{Filename}\"; ${Sdl.GetError()}");
            return;
        }
        // #TODO: Need to try and play sound effects globally;

        int channels = 0; // Mixer.AllocateChannels(1);
        if (channels == 0) {
            _log?.Error($"Unable to allocate channels for \"{Filename}\"; ${Sdl.GetError()}");
            return;
        }


        //currentChannel = Mixer.PlayChannel(channel, Pointer, loops);

        if(currentChannel < 0) {
            _log?.Error($"Unable to play sound effect \"{Filename}\"; ${Sdl.GetError()}");
            return;
        }

        _log?.Debug($"Playing Sound Effect \"{Filename}\"");
        IsPlaying = true;
    }

    public override void Stop() {
        if (Pointer == nint.Zero || currentChannel < 0) return;
        int res = 0; // Mixer.HaltChannel(currentChannel);
        if (res < 0) {
            _log?.Error($"Unable to stop sound effect \"{Filename}\"; ${Sdl.GetError()}");
            return;
        }
        currentChannel = -1;
    }

    /// <inheritdoc />
    public override void SetVolume(int volume) {
        volume = Math.Clamp(volume, 0, 128);
        //PreviousVolume = Mixer.Volume(currentChannel, volume);
    }

    /// <inheritdoc />
    public override int GetVolume() {
        return 0;// Mixer.Volume(currentChannel, -1);
    }

    public override void Resume() {
        //Mixer.Resume(currentChannel);
    }

    public override void Pause() {
        //Mixer.Pause(currentChannel);
    }

    #endregion
}