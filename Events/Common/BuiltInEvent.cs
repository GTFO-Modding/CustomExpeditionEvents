using CustomExpeditionEvents.Utilities;
using CustomExpeditionEvents.Utilities.Json;
using GameData;
using LevelGeneration;
using System.ComponentModel;

namespace CustomExpeditionEvents.Events.Common
{
    public static class BuiltInEvent
    {
        public static object CreateData_ActivateChainedPuzzleEvent(string puzzleID)
        {
            return new ActivateChainedPuzzleEvent.Data()
            {
                PuzzleID = puzzleID
            };
        }
        public static object CreateData_ActivateSurvivalWaveEvent(uint settingsID, 
            uint populationID, 
            SurvivalWaveSpawnType spawnType,
            SurvivalWaveEventSource? source = null,
            float spawnDelay = 0,
            float areaDistance = 2,
            Vec3? customSpawnDirection = default,
            bool playScreamOnSpawn = true,
            string? overrideWorldEventFilter = null,
            string? customWaveID = null)
        {
            return new ActivateSurvivalWaveEvent.Data()
            {
                AreaDistance = areaDistance,
                CustomSpawnDirection = customSpawnDirection,
                CustomWaveID = customWaveID,
                OverrideWorldEventFilter = overrideWorldEventFilter,
                PlayScreamOnSpawn = playScreamOnSpawn,
                PopulationID = populationID,
                SettingsID = settingsID,
                Source = source,
                SpawnDelay = spawnDelay,
                SpawnType = spawnType
            };
        }

        public static object CreateData_FogTransitionEvent(
            uint fogID,
            float transitionDuration,
            eDimensionIndex dimension)
        {
            return new FogTransitionEvent.Data()
            {
                Dimension = dimension,
                FogID = fogID,
                TransitionDuration = transitionDuration
            };
        }

        public static object CreateData_OpenSecurityDoorEvent(eLocalZoneIndex zone,
            LG_LayerType layer,
            eDimensionIndex dimension)
        {
            return new OpenSecurityDoorEvent.Data()
            {
                Dimension = dimension,
                Layer = layer,
                Zone = zone
            };
        }

        public static object CreateData_PlaySoundEvent(SoundEvent soundID,
            PlaySoundEventSource from,
            eLocalZoneIndex zoneIndex,
            LG_LayerType layerType,
            eDimensionIndex dimensionIndex,
            int areaIndex)
        {
            return new PlaySoundEvent.Data()
            {
                SoundID = soundID,
                From = from,
                ZoneIndex = zoneIndex,
                LayerType = layerType,
                DimensionIndex = dimensionIndex,
                AreaIndex = areaIndex
            };
        }

        public static object CreateData_SetDataEvent(string key, bool value)
        {
            return new SetDataEvent.Data()
            {
                Key = key,
                Type = SetDataEvent.DataType.Boolean,
                Value = value
            };
        }

        public static object CreateData_SetDataEvent(string key, int value)
        {
            return new SetDataEvent.Data()
            {
                Key = key,
                Type = SetDataEvent.DataType.Int32,
                Value = value
            };
        }

        public static object CreateData_SetDataEvent(string key, long value)
        {
            return new SetDataEvent.Data()
            {
                Key = key,
                Type = SetDataEvent.DataType.Int64,
                Value = value
            };
        }

        public static object CreateData_SetDataEvent(string key, float value)
        {
            return new SetDataEvent.Data()
            {
                Key = key,
                Type = SetDataEvent.DataType.Single,
                Value = value
            };
        }

        public static object CreateData_SetDataEvent(string key, double value)
        {
            return new SetDataEvent.Data()
            {
                Key = key,
                Type = SetDataEvent.DataType.Double,
                Value = value
            };
        }

        public static object CreateData_StopCustomSurvivalWaveEvent(string waveID)
        {
            return new StopCustomSurvivalWaveEvent.Data()
            {
                WaveID = waveID
            };
        }

        public static object CreateData_UnlockSecurityDoorEvent(eLocalZoneIndex zone,
            LG_LayerType layer,
            eDimensionIndex dimension)
        {
            return new UnlockSecurityDoorEvent.Data()
            {
                Dimension = dimension,
                Layer = layer,
                Zone = zone
            };
        }

        public static object CreateData_WardenIntelEvent(string text,
            float displayDuration,
            bool isObjectiveText)
        {
            return new WardenIntelEvent.Data()
            {
                Text = text,
                DisplayDuration = displayDuration,
                IsObjectiveText = isObjectiveText
            };
        }


        public enum PlaySoundEventSource
        {
            Global,
            LocalPlayer,
            Area
        }

        public sealed class SurvivalWaveEventSource
        {
            [Description("The zone index")]
            public eLocalZoneIndex Zone { get; set; }
            [Description("The layer")]
            public LG_LayerType Layer { get; set; }
            [Description("The dimension")]
            public eDimensionIndex Dimension { get; set; }
            [Description("The area the enemies should spawn from. If not specified, will use a random area.")]
            public int? Area { get; set; }
        }
    }
}
