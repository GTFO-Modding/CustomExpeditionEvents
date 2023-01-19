﻿using CustomExpeditionEvents.Utilities;
using GameData;
using LevelGeneration;
using System.ComponentModel;

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
            [Description("The zone of the door")]
            public eLocalZoneIndex Zone { get; set; }
            [Description("The layer of the door")]
            public LG_LayerType Layer { get; set; }
            [Description("The dimension of the door")]
            public eDimensionIndex Dimension { get; set; }
        }

    }
}
