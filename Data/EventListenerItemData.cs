using CustomExpeditionEvents.Converters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CustomExpeditionEvents.Data
{
    [JsonConverter(typeof(EventListenerItemDataJsonConverter))]
    public sealed class EventListenerItemData : ItemDataBase
    {
        public string TriggerName { get; set; } = string.Empty;
        public object? TriggerSettings { get; set; }
        public List<EventListenerItemConditionData> Conditions { get; set; } = new();
        public EventListenerItemActivationData Activation { get; set; } = new();
    }
}
