using Godot;
using System.Runtime.CompilerServices;
using System;

public partial class logger : Node
{
    public enum Severity{
        INFO = 0,
        WARN = 1,
        ERROR = 2,
    }

    public static readonly Severity LOG_SEVERITY = Severity.INFO; 

    public readonly Severity INFO = Severity.INFO;
    public readonly Severity WARN = Severity.WARN;
    public readonly Severity ERROR = Severity.ERROR;

    public void log (Variant _m, Severity _s = Severity.INFO,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string callingFilePath = ""){
        string fileName = callingFilePath.GetFile();
        DateTime localDate = DateTime.Now;
        string message = $"[{localDate}][{fileName} : {caller} : {lineNumber}] [{_s}] {_m}";
        switch(_s){
            case Severity.WARN:
                if(LOG_SEVERITY >= Severity.WARN)
                    break;
                GD.PushWarning(message);
                GD.Print(message);
                break;
            case Severity.ERROR:
                GD.PushError(message);
                GD.PrintErr(message);
                break;
            default:
                if(LOG_SEVERITY != Severity.INFO)
                    break;
                GD.Print(message);
                break;
        }
    }
}
