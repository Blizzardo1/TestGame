using System.Reflection;
using SharpSDL3;
using SharpSDL3.Enums;
using SharpSDL3.Mixer;
using SharpSDL3.Structs;
using SharpSDL3.TTF;
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

    private static readonly Log Log = Log.GetCurrentClassLogger(LogCategory.Application);

    private static Thread? _thread;
    private static Core? _game;

    public static uint CurrentFps { get; private set; } = 0;

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

        if (playbackDevices.Length == 0) {
            Log.Error("No Playback Devices found!");
            return null;
        }

        if (recordingDevices.Length == 0) {
            Log.Error("No Recording Devices found!");
            return null;
        }

        Log.Debug($"Playback Devices: {outputCount}");
        Log.Debug($"Recording Devices: {inputCount}");

        for (int i = 0; i < outputCount; i++) {
            string devName = Sdl.GetAudioDeviceName(playbackDevices[i]);
            Log.Debug($"Output Device {i}: {devName}");
            lst.Add($"output/{i}/{devName}");
        }

        for (int i = 0; i < inputCount; i++) {
            string devName = Sdl.GetAudioDeviceName(recordingDevices[i]);
            Log.Debug($"Input Device {i}: {devName}");
            lst.Add($"input/{i}/{devName}");
        }

        if (GameConfig is null) {
            Log.Error("Game Configuration not loaded or found. Please make sure this exists and is properly configured!");
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
        Log.Info($"SDL Version: {Sdl.GetRevision()}");
        Log.Info($"IMG Version {Sdl.ImageVersion().SdlVersionToString()}");
        Log.Info($"MIX Version {Mixer.MixerVersion()}");
        Log.Info($"TTF Version {Ttf.Version().SdlVersionToString()}");
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
            Log.Error("SDL Initialization failed!");
            return;
        }

        Log.Info("Initialized Core");

        Ttf.Init();
        AudioManager.Instance.Initialize();
        if(!Sdl.WasInit(InitFlags.Joystick).HasFlag(InitFlags.Joystick)) {
            Log.Error("SDL Joystick Subsystem Initialization failed!");
        }
        if(!initialized) {
            Log.Error("SDL Subsystem Initialization failed!");
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
            Log.Error("No Audio Output Device!");
            return;
        }

        Log.Info($"Selected Audio Output Device: {OutputDevice}");
        _game = new Core(width, height, title);
    }

    public static void PostInitialize() {
        if (_game is null) {
            Log.Error("No Game instance created!");
            return;
        }

        _fps = new FpsTimer();
        _thread = new Thread(_game.Start);

        _game.InitializeComponents();
        _thread.Start();

        while (_game.IsRunning) {
            _fps.Start();
            _ = Sdl.PollEvent(out Event e);

            _game.Update(e);
            _game.Draw();
            uint delta = (uint)_fps.GetTicks();
            if (delta < 1000 / FramesPerSecond) {
                Sdl.Delay(1000 / FramesPerSecond - delta);
            }
            CurrentFps = delta;
        }

        Mixer.CloseAudio();
        Mixer.Quit();
        // Image.Quit();
        Ttf.Quit();
        Sdl.Quit();

        Log.Info("Goodbye!");

        // No need to free the console. The OS will do that for us when the program exits.
    }
}