using GameData;
using LevelGeneration;
using System.Diagnostics.CodeAnalysis;

namespace CustomExpeditionEvents.Utilities
{
    internal static class ExpeditionUtilities
    {
        public static bool TryGetDimension(eDimensionIndex dimensionIndex, [NotNullWhen(true)] out Dimension? dimension)
        {
            LG_Floor currentFloor = Builder.CurrentFloor;
            if (currentFloor == null)
            {
                dimension = null;
                return false;
            }

            if (!currentFloor.GetDimension(dimensionIndex, out Dimension dim))
            {
                dimension = null;
                return false;
            }

            dimension = dim;
            return true;
        }

        public static bool TryGetLayer(eDimensionIndex dimensionIndex, LG_LayerType layerIndex, [NotNullWhen(true)] out LG_Layer? layer)
        {
            if (!TryGetDimension(dimensionIndex, out Dimension? dimension))
            {
                layer = null;
                return false;
            }

            return TryGetLayer(dimension, layerIndex, out layer);
        }

        public static bool TryGetLayer(Dimension dimension, LG_LayerType layerIndex, [NotNullWhen(true)] out LG_Layer? layer)
        {
            if ((int)layerIndex < 0 || (int)layerIndex >= dimension.Layers.Count)
            {
                layer = null;
                return false;
            }

            layer = dimension.GetLayer(layerIndex);

            return layer != null;
        }

        public static bool TryGetZone(eDimensionIndex dimensionIndex, LG_LayerType layerIndex, eLocalZoneIndex zoneIndex, [NotNullWhen(true)] out LG_Zone? zone)
        {
            if (!TryGetLayer(dimensionIndex, layerIndex, out LG_Layer? layer))
            {
                zone = null;
                return false;
            }

            return TryGetZone(layer, zoneIndex, out zone);
        }

        public static bool TryGetZone(Dimension dimension, LG_LayerType layerIndex, eLocalZoneIndex zoneIndex, [NotNullWhen(true)] out LG_Zone? zone)
        {
            if (!TryGetLayer(dimension, layerIndex, out LG_Layer? layer))
            {
                zone = null;
                return false;
            }

            return TryGetZone(layer, zoneIndex, out zone);
        }

        public static bool TryGetZone(LG_Layer layer, eLocalZoneIndex zoneIndex, [NotNullWhen(true)] out LG_Zone? zone)
        {
            if (!layer.m_zonesByLocalIndex.ContainsKey(zoneIndex))
            {
                zone = null;
                return false;
            }

            zone = layer.m_zonesByLocalIndex[zoneIndex];
            return zone != null;
        }

        public static bool TryGetSecurityDoor(eDimensionIndex dimensionIndex, LG_LayerType layerIndex, eLocalZoneIndex zoneIndex, [NotNullWhen(true)] out LG_SecurityDoor? securityDoor)
        {
            if (!TryGetZone(dimensionIndex, layerIndex, zoneIndex, out LG_Zone? zone))
            {
                securityDoor = null;
                return false;
            }

            return TryGetSecurityDoor(zone, out securityDoor);
        }

        public static bool TryGetSecurityDoor(Dimension dimension, LG_LayerType layerIndex, eLocalZoneIndex zoneIndex, [NotNullWhen(true)] out LG_SecurityDoor? securityDoor)
        {
            if (!TryGetZone(dimension, layerIndex, zoneIndex, out LG_Zone? zone))
            {
                securityDoor = null;
                return false;
            }

            return TryGetSecurityDoor(zone, out securityDoor);
        }

        public static bool TryGetSecurityDoor(LG_Layer layer, eLocalZoneIndex zoneIndex, [NotNullWhen(true)] out LG_SecurityDoor? securityDoor)
        {
            if (!TryGetZone(layer, zoneIndex, out LG_Zone? zone))
            {
                securityDoor = null;
                return false;
            }

            return TryGetSecurityDoor(zone, out securityDoor);
        }

        public static bool TryGetSecurityDoor(LG_Zone zone, [NotNullWhen(true)] out LG_SecurityDoor? securityDoor)
        {
            if (zone.m_areas.Count == 0)
            {
                securityDoor = null;
                return false;
            }

            LG_Area firstArea = zone.m_areas[0];
            if (firstArea == null)
            {
                securityDoor = null;
                return false;
            }

            foreach (LG_Gate gate in firstArea.m_gates)
            {
                iLG_Door_Core? spawnedDoor = gate.SpawnedDoor;
                if (spawnedDoor == null)
                {
                    continue;
                }

                if (spawnedDoor.DoorType != eLG_DoorType.Apex && spawnedDoor.DoorType != eLG_DoorType.Security)
                {
                    continue;
                }

                LG_SecurityDoor? door = spawnedDoor.TryCast<LG_SecurityDoor>();
                if (door == null)
                {
                    continue;
                }

                // make sure door links to current zone
                if (door.LinksToLayerType != zone.m_layer.m_type ||
                    door.LinkedToZoneData.LocalIndex != zone.LocalIndex)
                {
                    continue;
                }

                securityDoor = door;
                return true;
            }

            securityDoor = null;
            return false;
        }
    }
}
