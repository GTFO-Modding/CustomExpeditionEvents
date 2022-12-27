using CustomExpeditionEvents.Converters;
using System.Text.Json.Serialization;

namespace CustomExpeditionEvents.Data
{
    [JsonConverter(typeof(EventListenerItemConditionDataJsonConverter))]
    public sealed class EventListenerItemConditionData : ItemDataBase
    {
        public string ConditionName { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}