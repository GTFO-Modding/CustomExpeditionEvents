namespace CustomExpeditionEvents.Data
{
    /// <summary>
    /// Base properties for items.
    /// </summary>
    public abstract class ItemDataBase
    {
        public bool Disabled { get; set; }
        public string? DebugName { get; set; }
    }
}
