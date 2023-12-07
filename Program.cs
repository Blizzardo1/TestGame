using System.Reflection;
using SDL2;
using SDL2.TTF;

namespace TestGame;

internal static class Program {
    public const int FramesPerSecond = 30;
    public static readonly string StartupPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main() {
#if DEBUG
        NativeMethods.AllocConsole();
#endif

        _ = SDL.Init(InitFlags.Everything);
        _ = TTF.Init();

        var game = new Game(1920, 1080, "This is a Test");
        game.Start();
        while (game.IsRunning) {
            uint start = SDL.GetTicks();
            _ = SDL.PollEvent(out Event e);
            game.Update(e);
            game.Draw();
            uint stop = SDL.GetTicks();
            uint delta = stop - start;

            if (delta < 1000 / FramesPerSecond) {
                //Thread.Sleep(TimeSpan.FromTicks(1000 / FramesPerSecond - delta));
            }
        }

#if DEBUG
        NativeMethods.FreeConsole();
#endif
    }
}