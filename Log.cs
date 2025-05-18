using SharpSDL3;
using SharpSDL3.Enums;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace TestGame; 

public  class Log : IDisposable {
    private static bool _setup = false;
    private static bool _outputDebug = false;
    private static string _currentClass = "";
    private string _class = "";
    private LogCategory _category;
    private static string currentCusomCategoryStr = "";
    private string _customCategoryStr = "";
    private static FileStream? _logOut = null;

    private bool _disposed;

    private Log() {
        _disposed = false;
    }

    public static void Setup(bool fileOutput = false, bool outputDebug = false) {
        if (_setup) {
            return;
        }
        NativeMethods.AllocConsole();

        Logger.SetLogOutputFunction(GameLogOutputFunction, nint.Zero);
#if DEBUG
        Logger.SetLogPriorities(LogPriority.Debug);
#else
        Logger.SetLogPriorities(LogPriority.Info);
#endif
        if (fileOutput) {
            string logPath = Path.Combine("Logs", $"log{System.DateTime.Now:yyyyMMddHHmmss}.log");
            _logOut = new FileStream(logPath, FileMode.Create, FileAccess.Write);
            Logger.SetLogOutputFunction(GameLogOutputFunction, nint.Zero);
        }
        _outputDebug = outputDebug;
        _setup = true;
    }

    private static void GameLogOutputFunction(nint userdata,
        LogCategory category,
        LogPriority priority,
        string message) {

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
            _ => "Unknown"
        };

        string output = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}|{priorityStr}|{categoryStr}|{_currentClass}] {message}";
#if WINDOWS
        Console.ForegroundColor = priority switch {
            LogPriority.Verbose => ConsoleColor.White,
            LogPriority.Debug => ConsoleColor.Gray,
            LogPriority.Info => ConsoleColor.Cyan,
            LogPriority.Warn => ConsoleColor.Yellow,
            LogPriority.Error => ConsoleColor.Red,
            LogPriority.Critical => ConsoleColor.DarkRed,
            _ => ConsoleColor.White
        };
#else
    // Setup Non-Windows Console Colors

#endif
        Console.WriteLine(output);

        if (priority is LogPriority.Debug && !_outputDebug) {
            return;
        }

        if (_logOut is not null) {
            byte[] data = Encoding.UTF8.GetBytes(output + Environment.NewLine);
            _logOut.Write(data, 0, data.Length);
            _logOut.Flush();
        }
    }

    public static Log GetCurrentClassLogger(LogCategory category, string customCategory = "") {
        Log logger = new();
        (_, MethodBase methodBase) = GetStackFrame();
        logger._class = methodBase.ReflectedType!.FullName!;
        logger._category = category;
        if (category == LogCategory.Custom) {
            logger._customCategoryStr = customCategory;
        }
        return logger;
    }

    private static (StackFrame, MethodBase) GetStackFrame() {
        StackTrace stackTrace = new();
        StackFrame stackFrame = stackTrace.GetFrames().Skip(2).FirstOrDefault()!;
        MethodBase methodBase = stackFrame.GetMethod()!;
        return (stackFrame, methodBase);
    }

    private static void SetCurrentClassInfo(string className, string customCategory) {
        _currentClass = className;
        currentCusomCategoryStr = customCategory;
    }

    private static string AggregateConstructorInfo(ConstructorInfo constructorInfo) {
        StringBuilder sb = new();
        sb.Append(constructorInfo.DeclaringType!.Name);
        sb.Append('(');
        ParameterInfo[] parameters = constructorInfo.GetParameters();
        for (int i = 0; i < parameters.Length; i++) {
            sb.Append(parameters[i].ParameterType.Name);
            sb.Append(' ');
            sb.Append(parameters[i].Name);
            if (i < parameters.Length - 1) {
                sb.Append(", ");
            }
        }
        sb.Append(')');
        return sb.ToString();
    }

    private static string AggregateTypes(Type[] types) {
        StringBuilder sb = new();
        for (int i = 0; i < types.Length; i++) {
            sb.Append(types[i].Name);
            if (i < types.Length - 1) {
                sb.Append(", ");
            }
        }
        return sb.ToString();
    }

    private static string AggregateMethodInfo(MethodBase methodBase) {
        StringBuilder sb = new();
        MethodInfo mi = (MethodInfo)methodBase;
        switch (mi.ReturnType.Name) {
            case "List`1":
                sb.Append($"List<{AggregateTypes(mi.ReturnTypeCustomAttributes.GetType().GenericTypeArguments)}>");
                break;

            default:
                sb.Append(mi.ReturnType.Name);
                break;
        }
        sb.Append(' ');
        sb.Append(methodBase.Name);
        sb.Append('(');
        ParameterInfo[] parameters = methodBase.GetParameters();
        for (int i = 0; i < parameters.Length; i++) {
            sb.Append(parameters[i].ParameterType.Name);
            sb.Append(' ');
            sb.Append(parameters[i].Name);
            if (i < parameters.Length - 1) {
                sb.Append(", ");
            }
        }
        sb.Append(')');
        return sb.ToString();
    }

    public bool Assert(bool condition, string message) {
        SetCurrentClassInfo(_class, _customCategoryStr);
        if (condition) {
            return true;
        }
        Logger.LogError(LogCategory.Error, message);
        return false;
    }

    public void Debug(string message, bool includeStack = true) {
        SetCurrentClassInfo(_class, _customCategoryStr);
        (StackFrame stackFrame, MethodBase methodBase) = GetStackFrame();
        if (includeStack) {
            message = methodBase switch {
                ConstructorInfo constructorInfo => $"({AggregateConstructorInfo(constructorInfo)}) {message} : {stackFrame.GetFileName()} -> {stackFrame.GetFileLineNumber()}",
                _ => $"({AggregateMethodInfo(methodBase)}) {message}",
            };
        }
        Logger.LogDebug(_category, message);
    }

    public void Info(string message) {
        SetCurrentClassInfo(_class, _customCategoryStr);
        Logger.LogInfo(_category, message);
    }

    public void Warn(string message) {
        SetCurrentClassInfo(_class, _customCategoryStr);
        Logger.LogWarn(_category, message);
    }

    public void Error(string message) {
        SetCurrentClassInfo(_class, _customCategoryStr);
        Logger.LogError(LogCategory.Error, message);
    }

    public void Error(Exception ex, string message) {
        SetCurrentClassInfo(_class, _customCategoryStr);
        Logger.LogError(LogCategory.Error, $"{message} | {ex.Source}");
    }

    public void Critical(string message) {
        SetCurrentClassInfo(_class, _customCategoryStr);
        Logger.LogCritical(LogCategory.Error, message);
    }

    public void ConditionalDebug(string message) {
#if DEBUG
        SetCurrentClassInfo(_class, _customCategoryStr);
        Logger.LogDebug(_category, message);
#endif
    }

    public void Verbose(string message) {
        SetCurrentClassInfo(_class, _customCategoryStr);
        Logger.LogVerbose(_category, message);
    }

    public void SetPriority(LogPriority priority) {
        SetCurrentClassInfo(_class, _customCategoryStr);
        Logger.SetLogPriority(_category, priority);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) {
            return;
        }

        if (disposing) {
            _logOut?.Dispose();
        }
        _disposed = true;
    }
}