namespace CustomExpeditionEvents.Data
{
    public sealed class ExpeditionSettingsItemData : ItemDataBase
    {
        public eRundownTier Tier { get; set; }
        public int ExpeditionIndex { get; set; }

        public ExpeditionSettings Settings { get; set; } = new();
    }
}