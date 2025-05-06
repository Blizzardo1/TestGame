using System.Reflection;
using System.Runtime.InteropServices;
using SDL2;
using SDL2.TTF;
using TestGame.Config;
using Version = SDL2.Version;

namespace TestGame;

public static class Engine {
    public const int FramesPerSecond = 60;
    public static readonly string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public static readonly string LogPath = Path.Combine(StartupPath, "Logs");
    public static readonly string ConfigPath = Path.Combine(StartupPath, "Config");
    public static readonly string AudioConfigPath = Path.Combine(StartupPath, "Resources", "Audio");

    public static readonly string GameConfigFile = Path.Combine(ConfigPath, "game.json");

    private static readonly Logger? _log = Logger.GetCurrentClassLogger(LogCategory.Application);

    private static Thread? _thread;
    private static Core? _game;

    public static uint CurrentFPS { get; private set; } = 0;

    private static FpsTimer _fps;

    public static Core? Game => _game;

    static Engine() {
        _fps = new FpsTimer();
    }

    public static GameConfig? GameConfig { get; private set; }
    internal static string? OutputDevice { get; private set; }
    internal static string? InputDevice { get; private set; }

    private static List< string >? GetAudioDevices() {
        List< string > lst = [];

        int outputCount = SDL.GetNumAudioDevices(0);
        int inputCount = SDL.GetNumAudioDevices(1);

        for (int i = 0; i < outputCount; i++) {
            string devName = SDL.GetAudioDeviceName(i, 0);
            _log?.Debug($"Output Device {i}: {devName}");
            lst.Add($"output/{i}/{devName}");
        }

        for (int i = 0; i < inputCount; i++) {
            string devName = SDL.GetAudioDeviceName(i, 1);
            _log?.Debug($"Input Device {i}: {devName}");
            lst.Add($"input/{i}/{devName}");
        }

        if (GameConfig is null) {
            _log?.Error("Game Configuration not loaded or found. Please make sure this exists and is properly configured!");
            return null;
        }

        List< AudioDeviceConfig > devices = [];

        lst.ForEach(s => {
            AudioDeviceConfig adc = new(s);
            devices.Add(adc);
        });

        GameConfig.AudioDevices ??= devices;
        GameConfig.AudioDeviceOutput ??= lst.First(s => s.StartsWith("output"));
        GameConfig.AudioDeviceInput ??= lst.First(s => s.StartsWith("input"));

        GameConfig = GameConfig.SaveAndReload();
        return lst;
    }

    private static void TestSdlVersions() {
        SDL.GetVersion(out Version sdlVersion);
        Version mixerVersion = Mixer.MIX_Linked_Version();
        Version imageVersion = Image.LinkedVersion();
        Version ttfVersion = TTF.LinkedVersion();

        _log?.Debug($"SDL Version: {sdlVersion.String()}");
        _log?.Debug($"Mixer Version: {mixerVersion.String()}");
        _log?.Debug($"Image Version: {imageVersion.String()}");
        _log?.Debug($"TTF Version: {ttfVersion.String()}");
    }

    private static void CreateIfNotExist(string directory) {
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>
    /// Initializes the engine
    /// </summary>
    /// <remarks>Can break if you're not careful</remarks>
    public static void PreInitialize(string title, int width, int height) {

        CreateIfNotExist(LogPath);
        CreateIfNotExist(AudioConfigPath);
        CreateIfNotExist(ConfigPath);

        Logger.Setup(true
#if DEBUG
            , true
#endif
            );

        GameConfig = GameConfig.Load(GameConfigFile);

        _log?.Info("Initialized Core");

        _ = SDL.Init(InitFlags.Everything);
        _ = TTF.Init();
        TestSdlVersions();

        // This should be pulled from Config
        List< string >? lst = GetAudioDevices();
        AudioDeviceConfig? audioDevice =
            GameConfig.AudioDevices?.FirstOrDefault(adc =>
                GameConfig.AudioDeviceOutput != null && GameConfig.AudioDeviceOutput.Contains(adc.DeviceName!));
        OutputDevice = audioDevice?.DeviceName ?? lst?.FirstOrDefault(s => s.StartsWith(" output"));
        if (OutputDevice is null) {
            _log?.Error("No Audio Output Device!");
            return;
        }

        _log?.Info($"Selected Audio Output Device: {OutputDevice}");
        _game = new Core(width, height, title);
    }

    public static void PostInitialize() {
        if (_game is null) {
            _log?.Error("No Game instance created!");
            return;
        }

        _fps = new FpsTimer();
        _thread = new Thread(_game.Start);

        _game.InitializeComponents();
        _thread.Start();

        while (_game.IsRunning) {
            _fps.Start();
            _ = SDL.PollEvent(out Event e);

            _game.Update(e);
            _game.Draw();
            uint delta = _fps.GetTicks();
            if (delta < 1000 / FramesPerSecond) {
                SDL.Delay((1000 / FramesPerSecond) - delta);
            }
            CurrentFPS = delta;
        }

        Mixer.CloseAudio();

        Mixer.Quit();
        Image.Quit();
        TTF.Quit();
        SDL.Quit();

        _log?.Info("Goodbye!");

        // No need to free the console. The OS will do that for us when the program exits.
    }
}