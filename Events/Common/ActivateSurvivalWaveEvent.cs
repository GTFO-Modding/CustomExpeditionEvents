using AIGraph;
using CustomExpeditionEvents.Events.Common.Managers;
using CustomExpeditionEvents.Utilities;
using CustomExpeditionEvents.Utilities.Json;
using CustomExpeditionEvents.Utilities.Randomization;
using GameData;
using LevelGeneration;
using Player;
using SNetwork;
using System;
using System.ComponentModel;

namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class ActivateSurvivalWaveEvent : IEvent<ActivateSurvivalWaveEvent.Data>
    {
        public string Name => "ActivateSurvivalWave";

        public void Activate(Data data)
        {
            Log.Debug(nameof(ActivateSurvivalWaveEvent), "Activate");
            
            if (!SNet.IsMaster)
            {
                return;
            }

            AIG_CourseNode? sourceNode;
            if (data.Source == null)
            {
                sourceNode = null;
            }
            else if (ExpeditionUtilities.TryGetZone(data.Source.Dimension, data.Source.Layer, data.Source.Zone, out LG_Zone? zone) && zone.m_areas.Count > 0)
            {
                int areaIndex;
                if (!data.Source.Area.HasValue)
                {
                    areaIndex = RandomUtility.GenericRandomizer.NextRangeInt32(zone.m_areas.Count);
                }
                else
                {
                    areaIndex = data.Source.Area.Value;
                }

                areaIndex = Math.Max(Math.Min(areaIndex, zone.m_areas.Count - 1), 0);

                LG_Area area = zone.m_areas[areaIndex];

                sourceNode = area.m_courseNode;
            }
            else
            {
                Log.Warn(nameof(ActivateSurvivalWaveEvent), $"Failed to get zone in dimension {data.Source.Dimension}, layer {data.Source.Layer}, zone index {data.Source.Zone}, or zone has no areas! Using default behaviour of local player as course node.");
                sourceNode = null;
            }

            sourceNode ??= PlayerManager.GetLocalPlayerAgent().CourseNode;

            if (!Mastermind.Current.TriggerSurvivalWave(sourceNode, data.SettingsID, data.PopulationID, out ushort eventID, 
                spawnType: data.SpawnType, 
                spawnDelay: data.SpawnDelay, 
                areaDistance: data.AreaDistance, 
                playScreamOnSpawn: data.PlayScreamOnSpawn, 
                customSpawnDirection: data.CustomSpawnDirection.HasValue, 
                spawnDirection: data.CustomSpawnDirection ?? default,
                overrideWorldEventFilter: data.OverrideWorldEventFilter ?? string.Empty))
            {
                Log.Error(nameof(ActivateSurvivalWaveEvent), "Failed to trigger survival wave!");
                return;
            }

            if (string.IsNullOrWhiteSpace(data.CustomWaveID))
            {
                return;
            }

            SurvivalWaveEventManager.Register(eventID, data.CustomWaveID);
        }

        public sealed class Data
        {
            [Description("The ID of the SurvivalWaveSettings datablock to use")]
            public uint SettingsID { get; set; }
            [Description("The ID of the SurvivalWavePopulation datablock to use")]
            public uint PopulationID { get; set; }

            [Description("The source of where the enemies will spawn from. If not specified, will use the location of the host.")]
            public BuiltInEvent.SurvivalWaveEventSource? Source { get; set; }

            [Description("The type of survival wave spawn")]
            public SurvivalWaveSpawnType SpawnType { get; set; }
            [Description("The delay (in seconds) before the enemies spawn")]
            public float SpawnDelay { get; set; }
            [Description("The distance of how far the enemies will be spawn from the source. Defaults to two rooms.")]
            public float AreaDistance { get; set; } = 2;

            [Description("Custom spawn direction. Optional.")]
            public Vec3? CustomSpawnDirection { get; set; }

            [Description("Whether or not to play a wave scream when enemies spawn")]
            public bool PlayScreamOnSpawn { get; set; } = true;
            [Description("Don't know what this is used for. Your guess is as good as mine.")]
            public string? OverrideWorldEventFilter { get; set; }

            [Description("A unique ID for the wave. This will allow the wave to be stopped via the StopCustomSurvivalWave event.")]
            public string? CustomWaveID { get; set; }
        }
    }
}
