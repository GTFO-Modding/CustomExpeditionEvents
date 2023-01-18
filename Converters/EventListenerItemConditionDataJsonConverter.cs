using CustomExpeditionEvents.Conditions;
using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Extensions;
using System;
using System.Text.Json;

namespace CustomExpeditionEvents.Converters
{
    public sealed class EventListenerItemConditionDataJsonConverter : ItemDataBaseJsonConverter<EventListenerItemConditionData>
    {
        protected override bool ReadProperty(EventListenerItemConditionData item, string propertyName, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (options.ArePropertiesEqual(propertyName, nameof(EventListenerItemConditionData.ConditionName)))
            {
                reader.AssertPropertyType(nameof(EventListenerItemConditionData.ConditionName), JsonTokenType.String);

                item.ConditionName = reader.GetString()!;
                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventListenerItemConditionData.Data)))
            {
                if (string.IsNullOrEmpty(item.ConditionName))
                {
                    throw new JsonException($"Error reading property {nameof(EventListenerItemConditionData.Data)}: {nameof(EventListenerItemConditionData.ConditionName)} must be defined before the data is.");
                }

                Type? dataType = TriggerConditionRegistry.GetEntryDataType(item.ConditionName);
                if (dataType == null)
                {
                    throw new JsonException($"Error reading property {nameof(EventListenerItemConditionData.Data)}: Condition '{item.ConditionName}' does not support data.");
                }

                item.Data = JsonSerializer.Deserialize(ref reader, dataType, options);
                return true;
            }
            return false;
        }

        protected override bool IsItemValid(EventListenerItemConditionData item)
        {
            return !string.IsNullOrEmpty(item.ConditionName);
        }

        protected override void WriteProperties(Utf8JsonWriter writer, EventListenerItemConditionData value, JsonSerializerOptions options)
        {
            writer.WriteString(nameof(EventListenerItemConditionData.ConditionName), value.ConditionName);
            Type? conditionDataType = TriggerConditionRegistry.GetEntryDataType(value.ConditionName);
            if (conditionDataType != null)
            {
                writer.WritePropertyName(nameof(EventListenerItemConditionData.Data));
                JsonSerializer.Serialize(writer, value.Data, conditionDataType, options);
            }
        }
    }
}
