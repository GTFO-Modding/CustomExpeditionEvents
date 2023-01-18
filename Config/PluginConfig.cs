using BepInEx.Configuration;

namespace CustomExpeditionEvents.Config
{
    /// <summary>
    /// Represents all configurations for the plugin
    /// </summary>
    public sealed class PluginConfig
    {
        private readonly ConfigFile configFile;
        private readonly ConfigEntry<bool> dumpingEnabled;
        private readonly ConfigEntry<bool> dumpEvents;
        private readonly ConfigEntry<bool> dumpConditions;
        private readonly ConfigEntry<bool> dumpTriggers;
        private readonly ConfigEntry<int> maxEventTicks;

        /// <summary>
        /// Whether or not to dump data.
        /// </summary>
        public bool DumpingEnabled
        {
            get => this.dumpingEnabled.Value;
            set => this.dumpingEnabled.Value = value;
        }

        /// <summary>
        /// Whether or not to dump all the events registered
        /// </summary>
        public bool DumpEvents
        {
            get => this.dumpEvents.Value;
            set => this.dumpEvents.Value = value;
        }

        /// <summary>
        /// Whether or not to dump all conditions registered
        /// </summary>
        public bool DumpConditions
        {
            get => this.dumpConditions.Value;
            set => this.dumpConditions.Value = value;
        }

        /// <summary>
        /// Whether or not to dump all triggers registered
        /// </summary>
        public bool DumpTriggers
        {
            get => this.dumpTriggers.Value;
            set => this.dumpTriggers.Value = value;
        }

        /// <summary>
        /// The maximum number of ticks/event updates that can occur in one game frame.
        /// </summary>
        public int MaxEventTicks
        {
            get => this.maxEventTicks.Value;
            set => this.maxEventTicks.Value = value;
        }

        private PluginConfig(ConfigFile config)
        {
            this.configFile = config;
            this.dumpingEnabled = config.Bind("Dumping", "Enabled", false, "Whether or not dumping is enabled");
            this.dumpConditions = config.Bind("Dumping", "DumpConditions", true, "Whether or not to dump all registered conditions");
            this.dumpEvents = config.Bind("Dumping", "DumpEvents", true, "Whether or not to dump all registered events");
            this.dumpTriggers = config.Bind("Dumping", "DumpTriggers", true, "Whether or not to dump all registered triggers");
            this.maxEventTicks = config.Bind("Settings", "MaxEventTicks", -1, "The maximum number of ticks to occur each game frame. Set to -1 for infinite. This only matters for the master/host of the lobby.");
        }

        /// <summary>
        /// The current plugin config.
        /// </summary>
        public static PluginConfig Current { get; private set; } = null!;

        /// <summary>
        /// Saves the current config.
        /// </summary>
        public void Save()
        {
            this.configFile.Save();
        }

        internal static void Load(ConfigFile config)
        {
            PluginConfig.Current ??= new(config);

        }
    }
}
