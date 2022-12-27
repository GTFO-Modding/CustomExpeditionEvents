using CustomExpeditionEvents.Utilities;
using GameData;
using LevelGeneration;
using Player;

namespace CustomExpeditionEvents.Events.Common
{
    public sealed class PlaySoundEvent : IEvent<PlaySoundEvent.Data>
    {
        public string Name => "PlaySound";

        public enum PlaySoundFrom
        {
            Global,
            LocalPlayer,
            Area
        }

        public void Activate(Data data)
        {
            Log.Debug(nameof(PlaySoundEvent), "Activate");

            switch (data.From)
            {
                case PlaySoundFrom.Global:
                    this.ActivateFromGlobal(data.SoundID);
                    break;
                case PlaySoundFrom.LocalPlayer:
                    this.ActivateFromLocalPlayer(data.SoundID);
                    break;
                case PlaySoundFrom.Area:
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
            public SoundEvent SoundID { get; set; }
            public PlaySoundFrom From { get; set; }
            public eLocalZoneIndex ZoneIndex { get; set; }
            public LG_LayerType LayerType { get; set; }
            public eDimensionIndex DimensionIndex { get; set; }
            public int AreaIndex { get; set; }
        }
    }
}
