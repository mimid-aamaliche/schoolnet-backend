using System;
using System.IO;

namespace OnlineSchoolDataAccess
{
    /// <summary>
    /// Shared static state for the DataAccess layer.
    /// The Logger instance is configured once at startup and used across
    /// all clsSql* classes for error and diagnostic logging.
    /// </summary>
    internal static class clsGlobal
    {
        // ── Logger singleton ────────────────────────────────────────────────────
        // Writes to console (colour-coded) AND to a daily rolling log file
        // at  <assembly-dir>/Logs/schoolnet-YYYY-MM-DD.log

        public static readonly clsLogger Logger = new clsLogger(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs")
        );

        // ── Convenience shortcuts ───────────────────────────────────────────────

        /// <summary>Log an informational message from the data-access layer.</summary>
        public static void LogInfo(string context, string message)
            => Logger.Info(message, context);

        /// <summary>Log a warning from the data-access layer.</summary>
        public static void LogWarning(string context, string message)
            => Logger.Warning(message, context);

        /// <summary>Log an exception from the data-access layer.</summary>
        public static void LogError(string context, Exception ex)
            => Logger.Error(ex, context);
    }
}
