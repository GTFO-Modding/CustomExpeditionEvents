using CustomExpeditionEvents.Utilities;
using GameData;
using LevelGeneration;
using Player;
using System.ComponentModel;

namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class PlaySoundEvent : IEvent<PlaySoundEvent.Data>
    {
        public string Name => "PlaySound";

        public void Activate(Data data)
        {
            Log.Debug(nameof(PlaySoundEvent), "Activate");

            switch (data.From)
            {
                case BuiltInEvent.PlaySoundEventSource.Global:
                    this.ActivateFromGlobal(data.SoundID);
                    break;
                case BuiltInEvent.PlaySoundEventSource.LocalPlayer:
                    this.ActivateFromLocalPlayer(data.SoundID);
                    break;
                case BuiltInEvent.PlaySoundEventSource.Area:
                    this.ActivateFromArea(data.SoundID, data.DimensionIndex, data.LayerType, data.ZoneIndex, data.AreaIndex);
                    break;
                default:
                    Log.Error(nameof(PlaySoundEvent), "Unknown From source of " + data.From);
                    break;
            }
        }

        private void ActivateFromGlobal(SoundEvent soundEvent)
        {
            PlayerAgent? player = PlayerManager.GetLocalPlayerAgent();
            if (player == null)
            {
                Log.Warn(nameof(PlaySoundEvent), "Failed to find local player");
                return;
            }

            soundEvent.PostGlobal(player.Sound);
        }

        private void ActivateFromLocalPlayer(SoundEvent soundEvent)
        {
            PlayerAgent? player = PlayerManager.GetLocalPlayerAgent();
            if (player == null)
            {
                Log.Warn(nameof(PlaySoundEvent), "Failed to find local player");
                return;
            }

            soundEvent.Post(player.Sound);
        }

        private void ActivateFromArea(SoundEvent soundEvent, eDimensionIndex dimensionIndex, LG_LayerType layerType, eLocalZoneIndex zoneIndex, int areaIndex)
        {
            if (!ExpeditionUtilities.TryGetDimension(dimensionIndex, out Dimension? dimension))
            {
                Log.Warn(nameof(PlaySoundEvent), "Failed to get dimension " + dimensionIndex);
                return;
            }

            if (!ExpeditionUtilities.TryGetLayer(dimension, layerType, out LG_Layer? layer))
            {
                Log.Warn(nameof(PlaySoundEvent), "Failed to get layer " + layerType + " in dimension " + dimensionIndex);
                return;
            }

            if (!ExpeditionUtilities.TryGetZone(layer, zoneIndex, out LG_Zone? zone))
            {
                Log.Warn(nameof(PlaySoundEvent), "Failed to get zone " + zoneIndex + " in layer " + layerType + " in dimension " + dimensionIndex);
                return;
            }

            if (areaIndex < 0 || areaIndex >= zone.m_areas.Count)
            {
                Log.Warn(nameof(PlaySoundEvent), "Failed to get area of index " + areaIndex + " in zone " + zoneIndex + " in layer " + layerType + " in dimension " + dimensionIndex);
                return;
            }


            LG_Area area = zone.m_areas[areaIndex];

            soundEvent.Post(area.Position);
        }

        public sealed class Data
        {
            [Description("The ID of the sound")]
            public SoundEvent SoundID { get; set; }
            [Description("The source to play the sound from")]
            public BuiltInEvent.PlaySoundEventSource From { get; set; }
            [Description("The zone index to use (if playing from an area)")]
            public eLocalZoneIndex ZoneIndex { get; set; }
            [Description("The layer to use (if playing from an area)")]
            public LG_LayerType LayerType { get; set; }
            [Description("The dimension to use (if playing from an area)")]
            public eDimensionIndex DimensionIndex { get; set; }
            [Description("The area index to use (if playing from an area)")]
            public int AreaIndex { get; set; }
        }
    }
}
