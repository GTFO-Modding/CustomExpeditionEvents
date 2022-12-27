namespace CustomExpeditionEvents.Data
{
    public sealed class ChainedPuzzleItemData : ItemDataBase
    {
        public string Name { get; set; } = string.Empty;
        public ChainedPuzzleItemSpawnData SpawnData { get; set; } = new();
    }
}
