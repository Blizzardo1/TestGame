using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Mixer;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
using System.Reflection;
using TestGame.Config;
using TestGame.GameObjects;

namespace TestGame;

public static class Engine {
    public const int FramesPerSecond = 60;
    public static readonly string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public static readonly string LogPath = Path.Combine(StartupPath, "Logs");
    public static readonly string ConfigPath = Path.Combine(StartupPath, "Config");
    public static readonly string AudioConfigPath = Path.Combine(StartupPath, "Resources", "Audio");

    public static readonly string GameConfigFile = Path.Combine(ConfigPath, "game.json");

    private static readonly Log _log = Log.GetCurrentClassLogger(LogCategory.Application);

    private static Thread? _thread;
    private static Core? _game;

    public static uint CurrentFPS { get; private set; } = 0;

    private static FpsTimer _fps = new();

    public static Core? Game => _game;

    static Engine() {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }

    public static GameConfig? GameConfig { get; private set; }
    internal static string? OutputDevice { get; private set; }
    internal static string? InputDevice { get; private set; }

    private  static List< string >? GetAudioDevices() {
        List< string > lst = [];

        // Arrays of uints
        uint[] playbackDevices = Sdl.GetAudioPlaybackDevices(out int outputCount);
        uint[] recordingDevices = Sdl.GetAudioRecordingDevices(out int inputCount);

        if (playbackDevices == null) {
            _log.Error("No Playback Devices found!");
            return null;
        }

        if (recordingDevices == null) {
            _log.Error("No Recording Devices found!");
            return null;
        }

        _log.Debug($"Playback Devices: {outputCount}");
        _log.Debug($"Recording Devices: {inputCount}");

        for (int i = 0; i < outputCount; i++) {
            string devName = Sdl.GetAudioDeviceName(playbackDevices[i]);
            _log.Debug($"Output Device {i}: {devName}");
            lst.Add($"output/{i}/{devName}");
        }

        for (int i = 0; i < inputCount; i++) {
            string devName = Sdl.GetAudioDeviceName(recordingDevices[i]);
            _log.Debug($"Input Device {i}: {devName}");
            lst.Add($"input/{i}/{devName}");
        }

        if (GameConfig is null) {
            _log.Error("Game Configuration not loaded or found. Please make sure this exists and is properly configured!");
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
        _log.Debug($"SDL Version: {SharpSDL3.Version.GetRevision()}");
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

        Log.Setup(true
#if DEBUG
            , true
#endif
            );

        GameConfig = GameConfig.Load(GameConfigFile);

        bool initialized = Sdl.Init(InitFlags.Everything);
        if(!initialized) {
            _log.Error("SDL Initialization failed!");
            return;
        }

        _log.Info("Initialized Core");

        Ttf.Init();
        AudioManager.Instance.Initialize();
        initialized = Sdl.InitSubSystem(InitFlags.Everything);
        if(!Sdl.WasInit(InitFlags.Joystick).HasFlag(InitFlags.Joystick)) {
            _log.Error("SDL Joystick Subsystem Initialization failed!");
        }
        if(!initialized) {
            _log.Error("SDL Subsystem Initialization failed!");
            return;
        }
        TestSdlVersions();

        // This should be pulled from Config
        List< string >? lst = GetAudioDevices();
        AudioDeviceConfig? audioDevice =
            GameConfig.AudioDevices?.FirstOrDefault(adc =>
                GameConfig.AudioDeviceOutput != null && GameConfig.AudioDeviceOutput.Contains(adc.DeviceName!));
        OutputDevice = audioDevice?.DeviceName ?? lst?.FirstOrDefault(s => s.StartsWith(" output"));
        if (OutputDevice is null) {
            _log.Error("No Audio Output Device!");
            return;
        }

        _log.Info($"Selected Audio Output Device: {OutputDevice}");
        _game = new Core(width, height, title);
    }

    public static void PostInitialize() {
        if (_game is null) {
            _log.Error("No Game instance created!");
            return;
        }

        _fps = new FpsTimer();
        _thread = new Thread(_game.Start);

        _game.InitializeComponents();
        _thread.Start();

        Event[] events = new Event[10];
        Sdl.PeepEvents(ref events, events.Length, EventAction.Peek, EventType.First, EventType.Last);
        for (int i = 0; i < events.Length; i++) {
            Event e = events[i];
            Logger.LogDebug(LogCategory.Application, $"Event[{i}] = {e.Type}");
        }

        while (_game.IsRunning) {
            _fps.Start();
            _ = Sdl.PollEvent(out Event e);

            _game.Update(e);
            _game.Draw();
            uint delta = (uint)_fps.GetTicks();
            if (delta < 1000 / FramesPerSecond) {
                Sdl.Delay((1000 / FramesPerSecond) - delta);
            }
            CurrentFPS = delta;
        }

        Mixer.CloseAudio();
        Mixer.Quit();
        // Image.Quit();
        Ttf.Quit();
        Sdl.Quit();

        _log.Info("Goodbye!");

        // No need to free the console. The OS will do that for us when the program exits.
    }
}