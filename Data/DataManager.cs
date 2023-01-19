using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CustomExpeditionEvents.Data.Registries;
using CustomExpeditionEvents.Utilities;
using MTFO.Managers;

namespace CustomExpeditionEvents.Data
{
    public static class DataManager
    {
        public static ChainedPuzzleRegistry ChainedPuzzles { get; } = new();
        public static EventListenerRegistry EventListeners { get; } = new();
        public static EventSequenceRegistry EventSequences { get; } = new();
        public static RundownSettingsRegistry RundownSettings { get; } = new();

        internal static void Initialize()
        {
            DataManager.EventListeners.RegisterAll(DataManager.Load<EventListenersWrapper>("EventListeners.json").Listeners);
            DataManager.ChainedPuzzles.RegisterAll(DataManager.Load<ChainedPuzzleWrapper>("ChainedPuzzles.json").Puzzles);
            DataManager.EventSequences.RegisterAll(DataManager.Load<EventSequencesWrapper>("EventSequences.json").Sequences);
            DataManager.RundownSettings.RegisterAll(DataManager.Load<RundownSettingsWrapper>("RundownSettings.json").Rundowns);
        }

        #region Utilities
        private static string GetFilePath(string file)
        {
            string rootPath = Path.Combine(ConfigManager.CustomPath, "CustomExpeditionEvents");
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            return Path.Combine(rootPath, file);
        }

        private static TData Load<TData>(string file)
            where TData : new()
            => Load(file, new TData());

        private static TData Load<TData>(string file, TData defaultData)
        {
            if (!TryLoad(file, out TData? data))
            {
                data = defaultData;
                Save(file, data);
            }
            

            return data;
        }

        private static bool TryLoad<TData>(string file, [NotNullWhen(true)] out TData? data)
        {
            string filePath = GetFilePath(file);

            if (!File.Exists(filePath))
            {
                data = default;
                return false;
            }

            string contents = File.ReadAllText(filePath);
            data = JsonSerializer.Deserialize<TData>(contents, options: new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                },
                ReadCommentHandling = JsonCommentHandling.Skip
            });
            return data is not null;
        }

        private static void Save<TData>(string file, TData data)
        {
            if (!TrySave(file, data))
            {
                Log.Warn(nameof(DataManager), "Failed to save file '" + file + "'");
            }
        }

        private static bool TrySave<TData>(string file, TData data)
        {
            try
            {
                string filePath = GetFilePath(file);
                string contents = JsonSerializer.Serialize(data, options: new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    Converters =
                    {
                        new JsonStringEnumConverter()
                    },
                    WriteIndented = true
                });

                File.WriteAllText(file, contents);

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        
        private sealed class ChainedPuzzleWrapper
        {
            public List<ChainedPuzzleItemData> Puzzles { get; set; } = new();
        }

        private sealed class EventListenersWrapper
        {
            public List<EventListenerItemData> Listeners { get; set; } = new();
        }

        private sealed class EventSequencesWrapper
        {
            public List<EventSequenceItemData> Sequences { get; set; } = new();
        }

        private sealed class RundownSettingsWrapper
        {
            public List<RundownSettingsItemData> Rundowns { get; set; } = new();
        }
    }
}
