using BepInEx.Logging;

namespace CustomExpeditionEvents.Utilities
{
    internal sealed class Log
    {
        private static readonly ManualLogSource logger = new ManualLogSource(PLUGIN_CONSTANTS.PLUGIN_NAME);

        static Log() => Logger.Sources.Add(Log.logger);

        public static void Verbose(object msg) => Log.logger.LogInfo(msg);

        public static void Debug(object msg) => Log.logger.LogDebug(msg);

        public static void Message(object msg) => Log.logger.LogMessage(msg);

        public static void Error(object msg) => Log.logger.LogError(msg);

        public static void Warn(object msg) => Log.logger.LogWarning(msg);

        public static void Verbose(string category, object msg) => Log.Verbose("[" + category + "] " + msg);

        public static void Debug(string category, object msg) => Log.Verbose("[" + category + "] " + msg);

        public static void Message(string category, object msg) => Log.Verbose("[" + category + "] " + msg);

        public static void Error(string category, object msg) => Log.Verbose("[" + category + "] " + msg);

        public static void Warn(string category, object msg) => Log.Verbose("[" + category + "] " + msg);
    }
}
