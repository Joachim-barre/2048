using Godot;
using System.Runtime.CompilerServices;
using System;

public static class Log
{
    public enum Severity{
        DEBUG = -1,
        INFO = 0,
        WARN = 1,
        ERROR = 2,
    }

#if DEBUG
    public static Severity LOG_SEVERITY = Severity.DEBUG; 
#else
    public static Severity LOG_SEVERITY = Severity.WARN;
#endif

    public static readonly Severity DEBUG = Severity.DEBUG;
    public static readonly Severity INFO = Severity.INFO;
    public static readonly Severity WARN = Severity.WARN;
    public static readonly Severity ERROR = Severity.ERROR;

    public static void log (Variant _m, Severity _s = Severity.DEBUG,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callingFilePath = ""){
        string fileName = callingFilePath.GetFile();
        DateTime localDate = DateTime.Now;
        string message = $"[{localDate}][{fileName} : {caller} : {lineNumber}] [{_s}] {_m}";
        switch(_s){
            case Severity.WARN:
                if(LOG_SEVERITY > Severity.WARN)
                    break;
                GD.PushWarning(message);
                GD.PrintRich($"[color=orange]{message}[/color]");
                break;
            case Severity.ERROR:
                GD.PushError(message);
                GD.PrintErr(message);
                break;
            case Severity.INFO:
                if(LOG_SEVERITY > Severity.INFO)
                    break;
                GD.Print(message);
                break;
            default:
                if(LOG_SEVERITY != Severity.DEBUG)
                    break;
                GD.PrintRich($"[code]{message}[/code]");
                break;
        }
    }

    public static void dbg(Variant _m,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callingFilePath = ""){
        log(_m, Severity.DEBUG, lineNumber, caller, callingFilePath);
    }

    public static void info(Variant _m,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callingFilePath = ""){
        log(_m, Severity.INFO, lineNumber, caller, callingFilePath);
    }

    public static void warn(Variant _m,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callingFilePath = ""){
        log(_m, Severity.WARN, lineNumber, caller, callingFilePath);
    }

    public static void error(Variant _m,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callingFilePath = ""){
        log(_m, Severity.ERROR, lineNumber, caller, callingFilePath);
    }

}
