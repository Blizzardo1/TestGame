using Newtonsoft.Json;
using NLog;
using SDL2;
using SDL2.TTF;
using TestGame.GameObjects;
using TestGame.Scenes;

namespace TestGame.Config {
    public record GameConfig(
        [property: JsonIgnore] string Path,
        [property: JsonProperty("maps")] List<MapConfig> MapConfigs,
        [property: JsonProperty("audio")] List<AudioConfig> AudioConfigs,
        [property: JsonProperty("fonts")] List<FontConfig> FontConfigs) {
        
        private static readonly Logger? _log =
#if DEBUG
            LogManager.GetCurrentClassLogger();
#else
            null;
#endif

        // Can't do Scenes or Maps in this function as there's
        // no real way to add them as a resource. They are not GameObjects.
        private void AllocateResources() {
            //AudioConfigs.ForEach(audio => ResourceManager.Add(new SoundEffect(audio.Reference, Program.AudioConfigPath)));
            FontConfigs?.ForEach(font => {
                    if (font.Name is null || font.Font is null || font.Name.IsEmpty() || font.Font.IsEmpty()) {
                        _log?.Error("Font name or path is empty.");
                        return;
                    }
                    Renderer.GetFont(font.Name, font.Font, 12);
                });
        }

        public static GameConfig Load(string path) {
            if (!File.Exists(path)) {
                _log?.Error($"Could not find GameConfig at {path}");
                GameConfig g = new(path, [], [], []);
                g.Save();
                return g;
            }

            using var reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            GameConfig? gc = JsonConvert.DeserializeObject<GameConfig>(json);
            if (gc == null) {
                _log?.Error("Failed to load GameConfig");
                return new(path, [], [], []);
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
    }
}
