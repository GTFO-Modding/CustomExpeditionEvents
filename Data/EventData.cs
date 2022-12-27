using CustomExpeditionEvents.Converters;
using System.Text.Json.Serialization;

namespace CustomExpeditionEvents.Data
{
    [JsonConverter(typeof(EventDataJsonConverter))]
    public sealed class EventData : ItemDataBase
    {
        public string EventName { get; set; } = string.Empty;
        public object? Data { get; set; }
        public float EventDelay { get; set; }
        public float SequenceDelay { get; set; }
        public bool SkipIfHost { get; set; }
        public bool SkipIfClient { get; set; }
        public int EventPosition { get; set; }
        public int? NextPosition { get; set; }
    }
}
