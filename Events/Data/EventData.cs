using CustomExpeditionEvents.Events.Data.Converters;
using System.Text.Json.Serialization;

namespace CustomExpeditionEvents.Events.Data
{
    [JsonConverter(typeof(EventDataJsonConverter))]
    public sealed class EventData
    {
        public string EventName { get; set; }
        public object? Data { get; set; }
        public float EventDelay { get; set; }
        public float SequenceDelay { get; set; }
        public bool SkipIfMaster { get; set; }
        public bool SkipIfClient { get; set; }
    }
}
