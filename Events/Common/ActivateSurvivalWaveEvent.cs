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

namespace CustomExpeditionEvents.Events.Common
{
    public sealed class ActivateSurvivalWaveEvent : IEvent<ActivateSurvivalWaveEvent.Data>
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
            public uint SettingsID { get; set; }
            public uint PopulationID { get; set; }

            public SpawnSource? Source { get; set; }

            public SurvivalWaveSpawnType SpawnType { get; set; }
            public float SpawnDelay { get; set; }
            public float AreaDistance { get; set; } = 2;

            public Vec3? CustomSpawnDirection { get; set; }

            public bool PlayScreamOnSpawn { get; set; } = true;
            public string? OverrideWorldEventFilter { get; set; }

            public string? CustomWaveID { get; set; }
        }

        public sealed class SpawnSource
        {
            public eLocalZoneIndex Zone { get; set; }
            public LG_LayerType Layer { get; set; }
            public eDimensionIndex Dimension { get; set; }
            public int? Area { get; set; }
        }
    }
}
