using CustomExpeditionEvents.Utilities;
using GameData;
using LevelGeneration;

namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class UnlockSecurityDoorEvent : IEvent<UnlockSecurityDoorEvent.Data>
    {
        public string Name => "CloseSecurityDoor";

        public void Activate(Data data)
        {
            Log.Debug(nameof(UnlockSecurityDoorEvent), "Activate");
            if (!ExpeditionUtilities.TryGetSecurityDoor(data.Dimension, data.Layer, data.Zone, out LG_SecurityDoor? door))
            {
                Log.Warn(nameof(UnlockSecurityDoorEvent), $"Could not find door in dimension {data.Dimension} layer {data.Layer} zone {data.Zone}");
                return;
            }

            door.m_sync.AttemptDoorInteraction(eDoorInteractionType.Unlock);
        }

        public sealed class Data
        {
            public eLocalZoneIndex Zone { get; set; }
            public LG_LayerType Layer { get; set; }
            public eDimensionIndex Dimension { get; set; }
        }

    }
}
