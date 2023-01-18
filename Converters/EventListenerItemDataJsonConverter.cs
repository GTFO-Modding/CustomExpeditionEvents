using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Extensions;
using CustomExpeditionEvents.Triggers;
using System.Collections.Generic;
using System;
using System.Text.Json;

namespace CustomExpeditionEvents.Converters
{
    public sealed class EventListenerItemDataJsonConverter : ItemDataBaseJsonConverter<EventListenerItemData>
    {
        protected override bool ReadProperty(EventListenerItemData item, string propertyName, ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (options.ArePropertiesEqual(propertyName, nameof(EventListenerItemData.TriggerName)))
            {
                reader.AssertPropertyType(nameof(EventListenerItemData.TriggerName), JsonTokenType.String);

                item.TriggerName = reader.GetString()!;
                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventListenerItemData.Conditions)))
            {
                reader.AssertPropertyType(nameof(EventListenerItemData.Conditions), JsonTokenType.StartArray);

                List<EventListenerItemConditionData>? conditions = JsonSerializer.Deserialize<List<EventListenerItemConditionData>>(ref reader, options);

                if (conditions != null)
                {
                    item.Conditions = conditions;
                }

                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventListenerItemData.Activation)))
            {
                reader.AssertPropertyType(nameof(EventListenerItemData.Activation), JsonTokenType.StartObject);

                EventListenerItemActivationData? activationData = JsonSerializer.Deserialize<EventListenerItemActivationData>(ref reader, options);

                if (activationData != null)
                {
                    item.Activation = activationData;
                }

                return true;
            }
            else if (options.ArePropertiesEqual(propertyName, nameof(EventListenerItemData.TriggerSettings)))
            {
                if (string.IsNullOrEmpty(item.TriggerName))
                {
                    throw new JsonException($"Error reading property {nameof(EventListenerItemData.TriggerSettings)}: {nameof(EventListenerItemData.TriggerName)} must be defined before the settings are.");
                }

                Type? settingsType = EventTriggerRegistry.GetEntrySettingsType(item.TriggerName);
                if (settingsType == null)
                {
                    throw new JsonException($"Error reading property {nameof(EventListenerItemData.TriggerSettings)}: Trigger '{item.TriggerName}' does not support settings.");
                }

                item.TriggerSettings = JsonSerializer.Deserialize(ref reader, settingsType, options);
                return true;
            }

            return false;
        }

        protected override bool IsItemValid(EventListenerItemData item)
        {
            return !string.IsNullOrEmpty(item.TriggerName);
        }

        protected override void WriteProperties(Utf8JsonWriter writer, EventListenerItemData value, JsonSerializerOptions options)
        {
            writer.WriteString(nameof(EventListenerItemData.TriggerName), value.TriggerName);
            Type? triggerSettingsType = EventTriggerRegistry.GetEntrySettingsType(value.TriggerName);
            if (triggerSettingsType != null)
            {
                writer.WritePropertyName(nameof(EventListenerItemData.TriggerSettings));
                JsonSerializer.Serialize(writer, value.TriggerSettings, triggerSettingsType, options);
            }
            writer.WritePropertyName(nameof(EventListenerItemData.Conditions));
            writer.WriteStartArray();
            foreach (EventListenerItemConditionData condition in value.Conditions)
            {
                JsonSerializer.Serialize(writer, condition, options);
            }
            writer.WriteEndArray();
            writer.WritePropertyName(nameof(EventListenerItemData.Activation));
            JsonSerializer.Serialize(writer, value.Activation, options);
        }
    }
}
