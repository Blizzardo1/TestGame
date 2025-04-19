using Newtonsoft.Json;
using NLog;

namespace TestGame.Config;

public record GameConfig([property: JsonIgnore] string Path) {
    [property: JsonProperty("maps")]
    public List< MapConfig >? Maps { get; set; }

    [property: JsonProperty("audio")]
    public List< AudioConfig >? AudioTracks { get; set; }

    [property: JsonProperty("audio-devices")]
    public List< AudioDeviceConfig >? AudioDevices { get; set; }

    [property: JsonProperty("fonts")]
    public List< FontConfig >? Fonts { get; set; }

    [property: JsonProperty("selected-audio-output")]
    public string? AudioDeviceOutput { get; set; }

    [property: JsonProperty("selected-audio-input")]
    public string? AudioDeviceInput { get; set; }

    private static readonly Logger? Log =
#if DEBUG
        LogManager.GetCurrentClassLogger();
#else
            null;
#endif

    // Can't do Scenes or Maps in this function as there's
    // no real way to add them as a resource. They are not GameObjects.
    private void AllocateResources() {
        //AudioTracks.ForEach(audio => ResourceManager.Add(new SoundEffect(audio.Reference, Program.AudioConfigPath)));
        Fonts?.ForEach(font => {
            if (font.Name is null || font.Font is null || font.Name.IsEmpty() || font.Font.IsEmpty()) {
                Log?.Error("Font name or path is empty.");
                return;
            }

            Renderer.GetFont(font.Name, font.Font, 12);
        });

        if (AudioDevices is null) return;
        if (AudioDevices.Count is 0) {
            Log?.Warn(
                "No audio devices are found in the config. The Engine should initialize and save all audio configs.");
        }

        AudioDevices.ForEach(adc => {
            if (adc.DeviceName is null || adc.DeviceIndex < 0 || adc.DeviceType is null) {
                Log?.Error(
                    "Audio Devices are not configured properly! Please check config and remove null or broken entries.");
            }
        });
    }

    public static GameConfig Load(string path) {
        if (!File.Exists(path)) {
            Log?.Error($"Could not find GameConfig at {path}");
            var g = new GameConfig(path);
            g.Save();
            return g;
        }

        using var reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        var gc = JsonConvert.DeserializeObject< GameConfig >(json);
        if (gc == null) {
            Log?.Error("Failed to load GameConfig");
            return new GameConfig(path);
        }

        gc = gc with { Path = path };
        gc.AllocateResources();

        return gc;
    }

    public void Save() {
        using var writer = new StreamWriter(Path);
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        writer.Write(json);
    }

    public GameConfig SaveAndReload() {
        Save();
        return Load(Path);
    }
}