

namespace TestGame.GameObjects; 

public abstract class Audio : IGameObject {

    protected nint Pointer = nint.Zero;

    protected int PreviousVolume = 0;

    protected string? Filename { get; init; }
    public string? Name { get; protected init; }

    public bool IsPlaying { get; protected set; }
    ~Audio() {
        // Mixer.FreeChunk(Pointer);
    }

    public abstract void Play();

    public abstract void SetVolume(int volume);

    public abstract int GetVolume();

    public abstract void Resume();
    public abstract void Pause();
    public abstract void Stop();
}