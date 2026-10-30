using System;
using System.IO;

namespace OnlineSchoolDataAccess
{
    /// <summary>
    /// Severity levels for log entries.
    /// </summary>
    public enum AppLogLevel
    {
        Info    = 0,
        Warning = 1,
        Error   = 2
    }

    /// <summary>
    /// Thread-safe, structured file + console logger used by all layers.
    /// Writes rotating daily log files to  Logs/schoolnet-YYYY-MM-DD.log
    /// alongside the running assembly.
    /// </summary>
    public sealed class clsLogger
    {
        // ── configuration ──────────────────────────────────────────────────────
        private readonly string _logDirectory;
        private readonly object _fileLock = new object();

        // Legacy delegate kept for backward-compatibility with existing callers
        public delegate void LogAction(string message, DateTime dateOfMessage);
        private readonly LogAction? _legacyAction;

        // ── constructors ────────────────────────────────────────────────────────

        /// <summary>Legacy constructor — keeps existing callers working.</summary>
        public clsLogger(LogAction logaction)
        {
            _legacyAction = logaction;
            _logDirectory = ResolveLogDirectory();
            EnsureLogDirectory();
        }

        /// <summary>File-only constructor.</summary>
        public clsLogger(string logDirectory)
        {
            _logDirectory = logDirectory;
            EnsureLogDirectory();
        }

        // ── public API ──────────────────────────────────────────────────────────

        /// <summary>Legacy single-argument log (Info level, for backward compat).</summary>
        public void Log(string message, DateTime date)
        {
            Write(AppLogLevel.Info, message, null, date);
            _legacyAction?.Invoke(message, date);
        }

        public void Info(string message, string? context = null)
            => Write(AppLogLevel.Info, message, context);

        public void Warning(string message, string? context = null)
            => Write(AppLogLevel.Warning, message, context);

        public void Error(string message, string? context = null)
            => Write(AppLogLevel.Error, message, context);

        public void Error(Exception ex, string? context = null)
            => Write(AppLogLevel.Error, ex.Message, context, exception: ex);

        // ── private helpers ─────────────────────────────────────────────────────

        private void Write(AppLogLevel level, string message, string? context,
                           DateTime? timestamp = null, Exception? exception = null)
        {
            var ts    = timestamp ?? DateTime.Now;
            var label = level switch
            {
                AppLogLevel.Warning => "WARN ",
                AppLogLevel.Error   => "ERROR",
                _                   => "INFO "
            };

            var contextPart = string.IsNullOrEmpty(context) ? "" : $" [{context}]";
            var line        = $"[{ts:yyyy-MM-dd HH:mm:ss.fff}] [{label}]{contextPart} {message}";

            if (exception != null)
                line += $"\n  → {exception.GetType().Name}: {exception.Message}\n  {exception.StackTrace}";

            // Console (colour-coded)
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = level switch
            {
                AppLogLevel.Error   => ConsoleColor.Red,
                AppLogLevel.Warning => ConsoleColor.Yellow,
                _                   => ConsoleColor.Cyan
            };
            Console.WriteLine(line);
            Console.ForegroundColor = prev;

            // File (thread-safe)
            try
            {
                var filePath = Path.Combine(_logDirectory, $"schoolnet-{ts:yyyy-MM-dd}.log");
                lock (_fileLock)
                {
                    File.AppendAllText(filePath, line + Environment.NewLine);
                }
            }
            catch
            {
                // Never let logging crash the application
            }
        }

        private static string ResolveLogDirectory()
        {
            var assemblyDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(assemblyDir, "Logs");
        }

        private void EnsureLogDirectory()
        {
            try { Directory.CreateDirectory(_logDirectory); }
            catch { /* best effort */ }
        }
    }
}
