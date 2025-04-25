using System.Reflection;
using NLog;
using NLog.Conditions;
using NLog.Targets;
using SDL2;
using SDL2.TTF;
using TestGame.Config;
using Version = SDL2.Version;

namespace TestGame;

public static class Engine {
    public const int FramesPerSecond = 30;
    public static readonly string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public static readonly string ConfigPath = Path.Combine(StartupPath, "Config");
    public static readonly string AudioConfigPath = Path.Combine(StartupPath, "Resources", "Audio");

    public static readonly string GameConfigFile = Path.Combine(ConfigPath, "game.json");

    private static Logger? _log;
    private static Thread? _thread;
    private static Core? _game;

    public static Core? Game => _game;

    public static GameConfig? GameConfig { get; private set; }
    internal static string? OutputDevice { get; private set; }
    internal static string? InputDevice { get; private set; }

    private static ColoredConsoleTarget SetupColoredConsole() {
        ColoredConsoleTarget cct = new();
        var infoHighlight = new ConsoleRowHighlightingRule(ConditionParser.ParseExpression("level == LogLevel.Info"),
            ConsoleOutputColor.Green, ConsoleOutputColor.NoChange);
        var warnHighlight = new ConsoleRowHighlightingRule(ConditionParser.ParseExpression("level == LogLevel.Warn"),
            ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange);
        var errorHighlight = new ConsoleRowHighlightingRule(ConditionParser.ParseExpression("level == LogLevel.Error"),
            ConsoleOutputColor.Red, ConsoleOutputColor.NoChange);
        var fatalHighlight = new ConsoleRowHighlightingRule(ConditionParser.ParseExpression("level == LogLevel.Fatal"),
            ConsoleOutputColor.Magenta, ConsoleOutputColor.NoChange);
        cct.RowHighlightingRules.Add(infoHighlight);
        cct.RowHighlightingRules.Add(warnHighlight);
        cct.RowHighlightingRules.Add(errorHighlight);
        cct.RowHighlightingRules.Add(fatalHighlight);
        cct.Layout = @"[${date:format=HH\:mm\:ss}] (${level:uppercase=true}) >> ${logger} -> ${message}";
        cct.UseDefaultRowHighlightingRules = false;
        cct.WordHighlightingRules.Add(new ConsoleWordHighlightingRule("TestGame", ConsoleOutputColor.Cyan,
            ConsoleOutputColor.NoChange));
        return cct;
    }

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
            _log?.Error(
                "Game Configuration not loaded or found. Please make sure this exists and is properly configured!");
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

        _log?.Info($"SDL Version: {sdlVersion.String()}");
        _log?.Info($"Mixer Version: {mixerVersion.String()}");
        _log?.Info($"Image Version: {imageVersion.String()}");
        _log?.Info($"TTF Version: {ttfVersion.String()}");
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
#if DEBUG
        NativeMethods.AllocConsole();
        _log = LogManager.GetCurrentClassLogger();
#endif

        LogManager.Setup().LoadConfiguration(builder => {
            builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteTo(SetupColoredConsole());
            builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToFile(fileName: "output.log");
        });

        CreateIfNotExist(AudioConfigPath);
        CreateIfNotExist(ConfigPath);
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

        _thread = new Thread(_game.Start);

        _game.InitializeComponents();
        _thread.Start();

        while (_game.IsRunning) {
            uint start = SDL.GetTicks();
            _ = SDL.PollEvent(out Event e);

            _game.Update(e);
            _game.Draw();
            uint stop = SDL.GetTicks();
            uint delta = stop - start;

            if (delta < 1000 / FramesPerSecond) {
                //Thread.Sleep(TimeSpan.FromTicks(1000 / FramesPerSecond - delta));
            }
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