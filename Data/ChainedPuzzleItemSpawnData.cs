using GameData;
using LevelGeneration;

namespace CustomExpeditionEvents.Data
{
    public sealed class ChainedPuzzleItemSpawnData
    {
        public eDimensionIndex DimensionIndex { get; set; }
        public LG_LayerType LayerType { get; set; }
        public eLocalZoneIndex ZoneIndex { get; set; }
        public int AreaIndex { get; set; }
    }
}