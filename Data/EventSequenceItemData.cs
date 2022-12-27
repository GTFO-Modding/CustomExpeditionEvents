using System.Collections.Generic;

namespace CustomExpeditionEvents.Data
{
    public sealed class EventSequenceItemData : ItemDataBase
    {
        public string Name { get; set; } = string.Empty;
        public List<EventData> Events { get; set; } = new();
    }
}
