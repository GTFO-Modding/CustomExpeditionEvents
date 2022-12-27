using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CustomExpeditionEvents.Utilities;
using MTFO.Managers;

namespace CustomExpeditionEvents.Data
{
    public static class DataManager
    {
        private static ReadOnlyCollection<ChainedPuzzleItemData>? s_chainedPuzzles;
        private static ReadOnlyCollection<EventListenerItemData>? s_eventListeners;
        private static ReadOnlyCollection<EventSequenceItemData>? s_eventSequences;
        private static ReadOnlyCollection<RundownSettingsItemData>? s_rundownSettings;

        public static ReadOnlyCollection<ChainedPuzzleItemData> ChainedPuzzles
        {
            get => DataManager.s_chainedPuzzles ?? new(new List<ChainedPuzzleItemData>());
        }

        public static ReadOnlyCollection<EventListenerItemData> EventListeners
        {
            get => DataManager.s_eventListeners ?? new(new List<EventListenerItemData>());
        }

        public static ReadOnlyCollection<EventSequenceItemData> EventSequences
        {
            get => DataManager.s_eventSequences ?? new(new List<EventSequenceItemData>());
        }

        public static ReadOnlyCollection<RundownSettingsItemData> RundownSettings
        {
            get => DataManager.s_rundownSettings ?? new(new List<RundownSettingsItemData>());
        }

        internal static void Initialize()
        {
            DataManager.s_chainedPuzzles = Load<ChainedPuzzleWrapper>("ChainedPuzzles.json")
                .Unwrap();
            DataManager.s_eventListeners = Load<EventListenersWrapper>("EventListeners.json")
                .Unwrap();
            DataManager.s_eventSequences = Load<EventSequencesWrapper>("EventSequences.json")
                .Unwrap();
            DataManager.s_rundownSettings = Load<RundownSettingsWrapper>("RundownSettings.json")
                .Unwrap();
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

            public ReadOnlyCollection<ChainedPuzzleItemData> Unwrap()
            {
                return new ReadOnlyCollection<ChainedPuzzleItemData>(this.Puzzles);
            }
        }

        private sealed class EventListenersWrapper
        {
            public List<EventListenerItemData> Listeners { get; set; } = new();


            public ReadOnlyCollection<EventListenerItemData> Unwrap()
            {
                return new ReadOnlyCollection<EventListenerItemData>(this.Listeners);
            }
        }

        private sealed class EventSequencesWrapper
        {
            public List<EventSequenceItemData> Sequences { get; set; } = new();

            public ReadOnlyCollection<EventSequenceItemData> Unwrap()
            {
                return new ReadOnlyCollection<EventSequenceItemData>(this.Sequences);
            }
        }

        private sealed class RundownSettingsWrapper
        {
            public List<RundownSettingsItemData> Rundowns { get; set; } = new();

            public ReadOnlyCollection<RundownSettingsItemData> Unwrap()
            {
                return new ReadOnlyCollection<RundownSettingsItemData>(this.Rundowns);
            }
        }
    }
}
