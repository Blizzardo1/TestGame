using SDL2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestGame {
    public class Logger {
        private static bool _setup = false;
        private static bool _outputDebug = false;
        private static string _currentClass = "";
        private string _class = "";
        private LogCategory _category;
        private static string currentCusomCategoryStr = "";
        private string _customCategoryStr = "";
        private static FileStream? _logOut = null;

        public static void Setup(bool fileOutput = false, bool outputDebug = false) {
            if(_setup) {
                return;
            }
            NativeMethods.AllocConsole();

            SDL.LogSetOutputFunction(GameLogOutputFunction, nint.Zero);
#if DEBUG
            SDL.LogSetAllPriority(LogPriority.Debug);
#else
        _log?.SetAllPriority(LogPriority.Info);
#endif
            if (fileOutput) {
                string logPath = Path.Combine("Logs", $"log{DateTime.Now:yyyyMMddHHmmss}.log");
                _logOut = new FileStream(logPath, FileMode.Create, FileAccess.Write);
                SDL.LogSetOutputFunction(GameLogOutputFunction, nint.Zero);
            }
            _outputDebug = outputDebug;
            _setup = true;
        }

        private static void GameLogOutputFunction(nint userdata,
            LogCategory category,
            LogPriority priority,
            nint message) {
            string msg = Marshal.PtrToStringUTF8(message)!;
            string categoryStr = category switch {
                LogCategory.Application => "Application",
                LogCategory.Audio => "Audio",
                LogCategory.Video => "Video",
                LogCategory.Error => "Error",
                LogCategory.Assert => "ASSERT",
                LogCategory.System => "System",
                LogCategory.Render => "Render",
                LogCategory.Input => "Input",
                LogCategory.Test => "TEST",
                LogCategory.Reserved1 => "RESERVED1",
                LogCategory.Reserved2 => "RESERVED2",
                LogCategory.Reserved3 => "RESERVED3",
                LogCategory.Reserved4 => "RESERVED4",
                LogCategory.Reserved5 => "RESERVED5",
                LogCategory.Reserved6 => "RESERVED6",
                LogCategory.Reserved7 => "RESERVED7",
                LogCategory.Reserved8 => "RESERVED8",
                LogCategory.Reserved9 => "RESERVED9",
                LogCategory.Reserved10 => "RESERVEDA",
                LogCategory.Custom => currentCusomCategoryStr,
                _ => "Unknown"
            };
            string priorityStr = priority switch {
                LogPriority.Verbose => "VERBOSE",
                LogPriority.Debug => "DEBUG",
                LogPriority.Info => "INFO",
                LogPriority.Warn => "WARN",
                LogPriority.Error => "ERROR",
                LogPriority.Critical => "CRITICAL",
                LogPriority.NumLogPriorities => "PRIORITY",
                _ => "Unknown"
            };

            string output = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}|{priorityStr}|{categoryStr}|{_currentClass}] {msg}";
#if WINDOWS
            Console.ForegroundColor = priority switch {
                LogPriority.Verbose => ConsoleColor.White,
                LogPriority.Debug => ConsoleColor.Gray,
                LogPriority.Info => ConsoleColor.Cyan,
                LogPriority.Warn => ConsoleColor.Yellow,
                LogPriority.Error => ConsoleColor.Red,
                LogPriority.Critical => ConsoleColor.DarkRed,
                LogPriority.NumLogPriorities => ConsoleColor.Magenta,
                _ => ConsoleColor.White
            };
#else
        // Setup Non-Windows Console Colors

#endif
            Console.WriteLine(output);

            if(priority is LogPriority.Debug && !_outputDebug) {
                return;
            }

            if (_logOut is not null) {
                byte[] data = Encoding.UTF8.GetBytes(output + Environment.NewLine);
                _logOut.Write(data, 0, data.Length);
                _logOut.Flush();
            }
        }

        public static Logger GetCurrentClassLogger(LogCategory category, string customCategory = "") {
            Logger logger = new();
            StackTrace stackTrace = new();
            StackFrame stackFrame = stackTrace.GetFrames().Skip(1).FirstOrDefault()!;
            MethodBase methodBase = stackFrame.GetMethod()!;
            logger._class = methodBase.ReflectedType!.FullName!;
            logger._category = category;
            if(category == LogCategory.Custom) {
                logger._customCategoryStr = customCategory;
            }
            return logger;
        }

        private static void SetCurrentClassInfo(string className, string customCategory) {
            _currentClass = className;
            currentCusomCategoryStr = customCategory;
        }

        public void Debug(string message) {
            SetCurrentClassInfo(_class, _customCategoryStr);
            SDL.LogDebug(_category, message);
        }
        public void Info(string message) {
            SetCurrentClassInfo(_class, _customCategoryStr);
            SDL.LogInfo(_category, message);
        }
        public void Warn(string message) {
            SetCurrentClassInfo(_class, _customCategoryStr);
            SDL.LogWarn(_category, message);
        }
        public void Error(string message) {
            SetCurrentClassInfo(_class, _customCategoryStr);
            SDL.LogError(LogCategory.Error, message);
        }

        public void Error(Exception ex, string message) {
            SetCurrentClassInfo(_class, _customCategoryStr);
            SDL.LogError(LogCategory.Error, $"{message} | {ex.Source}");
        }

        public void Critical(string message) {
            SetCurrentClassInfo(_class, _customCategoryStr);
            SDL.LogCritical(LogCategory.Error, message);
        }
        public void ConditionalDebug(string message) {
#if DEBUG
            SetCurrentClassInfo(_class, _customCategoryStr);
            SDL.LogDebug(_category, message);
#endif
        }
        public void Verbose(string message) {
            SetCurrentClassInfo(_class, _customCategoryStr);
            SDL.LogVerbose(_category, message);
        }
        public void SetPriority(LogPriority priority) {
            SetCurrentClassInfo(_class, _customCategoryStr);
            SDL.LogSetPriority(_category, priority);
        }
    }
}
