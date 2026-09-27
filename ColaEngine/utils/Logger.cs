using System;
using System.Diagnostics;
using Raylib_cs;

namespace ColaEngine.utils;

/// <summary>
/// Wrapper Around Raylibs Logging
/// </summary>
public static class Logger
{
    private static TraceLogLevel logLevel = TraceLogLevel.All;

    /// <summary>
    /// Get and set the current Logging Level.
    /// </summary>
    public static TraceLogLevel LogLevel
    {
        get => logLevel;
        set
        {
            logLevel = value;
            Raylib.SetTraceLogLevel(value);
        }
    }

    /// <summary>
    /// Log a message to the console will display as yellow.
    /// </summary>
    /// <param name="message">Message to Log.</param>
    public static void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"LOG: {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Trace logging, intended for internal use only
    /// </summary>
    /// <param name="message"></param>
    public static void Trace(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Raylib.TraceLog(TraceLogLevel.Trace, message);
        Console.ResetColor();
    }

    /// <summary>
    ///  Debug logging, used for internal debugging, it should be disabled on release builds
    /// </summary>
    /// <param name="message"></param>
    [Conditional("DEBUG")]
    public static void Debug(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Raylib.TraceLog(TraceLogLevel.Debug, message);
        Console.ResetColor();
    }

    /// <summary>
    /// Info logging, used for program execution info
    /// </summary>
    /// <param name="message"></param>
    public static void Info(string message)
    {
        Raylib.TraceLog(TraceLogLevel.Info, message);
    }

    /// <summary>
    /// Warning logging, used on recoverable failures
    /// </summary>
    /// <param name="message"></param>
    public static void Warning(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Raylib.TraceLog(TraceLogLevel.Warning, message);
        Console.ResetColor();
    }

    /// <summary>
    /// Error logging, used on unrecoverable failures
    /// </summary>
    /// <param name="message"></param>
    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Raylib.TraceLog(TraceLogLevel.Error, message);
        Console.ResetColor();
    }

    /// <summary>
    /// Fatal logging, used to abort program: exit(EXIT_FAILURE)
    /// </summary>
    /// <param name="message"></param>
    public static void Fatal(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Raylib.TraceLog(TraceLogLevel.Fatal, message);
        Console.ResetColor();
    }
}