using CustomExpeditionEvents.Utilities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomExpeditionEvents.Converters
{
    internal sealed class SoundEventJsonConverter : JsonConverter<SoundEvent>
    {
        public override SoundEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return new SoundEvent(reader.GetString());
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return new SoundEvent(reader.GetUInt32());
            }
            else
            {
                throw new JsonException($"Expected tokens {JsonTokenType.String} or {JsonTokenType.Number}, instead got {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, SoundEvent value, JsonSerializerOptions options)
        {
            if (value.IsName)
            {
                writer.WriteStringValue(value.Name);
            }
            else
            {
                writer.WriteNullValue(value.Id);
            }
        }
    }
}
