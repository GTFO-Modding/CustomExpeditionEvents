using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Events;
using CustomExpeditionEvents.Extensions;
using System;
using System.Text.Json;

namespace CustomExpeditionEvents.Converters
{
    public sealed class EventDataJsonConverter : ItemDataBaseJsonConverter<EventData>
    {
        protected override bool ReadProperty(EventData item, string propertyName, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (options.ArePropertiesEqual(propertyName, nameof(EventData.EventName)))
            {
                reader.AssertPropertyType(nameof(EventData.EventName), JsonTokenType.String);

                item.EventName = reader.GetString()!;
                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventData.EventDelay)))
            {
                reader.AssertPropertyType(nameof(EventData.EventDelay), JsonTokenType.Number);

                item.EventDelay = reader.GetSingle();
                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventData.SequenceDelay)))
            {
                reader.AssertPropertyType(nameof(EventData.SequenceDelay), JsonTokenType.Number);

                item.SequenceDelay = reader.GetSingle();
                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventData.SkipIfClient)))
            {
                reader.AssertPropertyType(nameof(EventData.SkipIfClient), JsonTokenType.True, JsonTokenType.False);

                item.SkipIfClient = reader.GetBoolean();
                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventData.SkipIfHost)))
            {
                reader.AssertPropertyType(nameof(EventData.SkipIfHost), JsonTokenType.True, JsonTokenType.False);

                item.SkipIfHost = reader.GetBoolean();
                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventData.EventPosition)))
            {
                reader.AssertPropertyType(nameof(EventData.EventPosition), JsonTokenType.Number);

                item.EventPosition = reader.GetInt32();
                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventData.NextPosition)))
            {
                reader.AssertPropertyType(nameof(EventData.EventPosition), JsonTokenType.Number, JsonTokenType.Null);

                if (reader.TokenType == JsonTokenType.Null)
                {
                    item.NextPosition = null;
                }
                else
                {
                    item.NextPosition = reader.GetInt32();
                }
                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventData.Data)))
            {
                if (string.IsNullOrEmpty(item.EventName))
                {
                    throw new JsonException($"Error reading property {nameof(EventData.Data)}: {nameof(EventData.EventName)} must be defined before the data is.");
                }

                Type? dataType = EventRegistry.GetEventDataType(item.EventName);
                if (dataType == null)
                {
                    throw new JsonException($"Error reading property {nameof(EventData.Data)}: Event '{item.EventName}' does not support data.");
                }

                item.Data = JsonSerializer.Deserialize(ref reader, dataType, options);
                return true;
            }

            return false;
        }

        protected override bool IsItemValid(EventData item)
        {
            return !string.IsNullOrEmpty(item.EventName);
        }

        protected override void WriteProperties(Utf8JsonWriter writer, EventData value, JsonSerializerOptions options)
        {
            writer.WriteString(nameof(EventData.EventName), value.EventName);
            Type? eventDataType = EventRegistry.GetEventDataType(value.EventName);
            if (eventDataType != null)
            {
                writer.WritePropertyName(nameof(EventData.Data));
                JsonSerializer.Serialize(writer, value.Data, eventDataType, options);
            }

            writer.WriteNumber(nameof(EventData.EventDelay), value.EventDelay);
            writer.WriteNumber(nameof(EventData.SequenceDelay), value.SequenceDelay);
        }
    }
}
