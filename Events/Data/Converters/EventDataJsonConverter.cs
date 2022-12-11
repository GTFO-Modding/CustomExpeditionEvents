using CustomExpeditionEvents.Extensions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomExpeditionEvents.Events.Data.Converters
{
    public sealed class EventDataJsonConverter : JsonConverter<EventData>
    {
        public override EventData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Expected {JsonTokenType.StartObject}, instead got {reader.TokenType}");
            }

            string propertyName = string.Empty;
            string? eventName = null;
            object? eventData = null;
            bool skipIfMaster = false;
            bool skipIfClient = false;
            float eventDelay = 0;
            float sequenceDelay = 0;

            bool readingProperty = true;
            while (reader.Read())
            {
                if (readingProperty)
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException($"Expected {JsonTokenType.PropertyName}, instead got {reader.TokenType}");
                    }

                    propertyName = reader.GetString()!;
                    readingProperty = !readingProperty;
                    continue;
                }

                if (options.ArePropertiesEqual(propertyName, nameof(EventData.EventName)))
                {
                    if (reader.TokenType != JsonTokenType.String)
                    {
                        throw new JsonException($"Expected {JsonTokenType.String}, instead got {reader.TokenType}");
                    }

                    eventName = reader.GetString()!;
                }
                else if (options.ArePropertiesEqual(propertyName, nameof(EventData.EventDelay)))
                {
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException($"Expected {JsonTokenType.Number}, instead got {reader.TokenType}");
                    }

                    eventDelay = reader.GetSingle();
                }
                else if (options.ArePropertiesEqual(propertyName, nameof(EventData.SequenceDelay)))
                {
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException($"Expected {JsonTokenType.Number}, instead got {reader.TokenType}");
                    }

                    sequenceDelay = reader.GetSingle();
                }
                else if (options.ArePropertiesEqual(propertyName, nameof(EventData.SkipIfClient)))
                {
                    if (reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False)
                    {
                        throw new JsonException($"Expected {JsonTokenType.True} or {JsonTokenType.False}, instead got {reader.TokenType}");
                    }

                    skipIfClient = reader.GetBoolean();
                }
                else if (options.ArePropertiesEqual(propertyName, nameof(EventData.SkipIfMaster)))
                {
                    if (reader.TokenType != JsonTokenType.True && reader.TokenType != JsonTokenType.False)
                    {
                        throw new JsonException($"Expected {JsonTokenType.True} or {JsonTokenType.False}, instead got {reader.TokenType}");
                    }

                    skipIfMaster = reader.GetBoolean();
                }
                else if (options.ArePropertiesEqual(propertyName, nameof(EventData.Data)))
                {
                    if (string.IsNullOrEmpty(eventName))
                    {
                        throw new JsonException("EventName must be defined before the data is.");
                    }

                    Type? dataType = EventManager.GetEventDataType(eventName);
                    if (dataType == null)
                    {
                        throw new JsonException($"Event '{eventName}' does not support data.");
                    }

                    eventData = JsonSerializer.Deserialize(ref reader, dataType, options);
                }
                else
                {
                    throw new JsonException("Unknown property '" + propertyName + "'");
                }

                readingProperty = !readingProperty;
            }

            if (string.IsNullOrEmpty(eventName))
            {
                throw new JsonException("Unexpected end of event.");
            }

            EventData data = new()
            {
                EventDelay = eventDelay,
                EventName = eventName,
                Data = eventData,
                SequenceDelay = sequenceDelay,
                SkipIfClient = skipIfClient,
                SkipIfMaster = skipIfMaster
            };

            return data;
        }

        public override void Write(Utf8JsonWriter writer, EventData value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(EventData.EventName), value.EventName);
            Type? eventDataType = EventManager.GetEventDataType(value.EventName);
            if (eventDataType != null)
            {
                writer.WritePropertyName(nameof(EventData.Data));
                JsonSerializer.Serialize(writer, value.Data, eventDataType, options);
            }

            writer.WriteNumber(nameof(EventData.EventDelay), value.EventDelay);
            writer.WriteNumber(nameof(EventData.SequenceDelay), value.SequenceDelay);
            writer.WriteEndObject();
        }
    }
}
