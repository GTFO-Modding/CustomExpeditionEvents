using CustomExpeditionEvents.Conditions;
using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Utilities;
using CustomExpeditionEvents.Utilities.Registry;
using System;
using System.Linq;
using System.Text;

namespace CustomExpeditionEvents.Triggers
{
    public sealed class EventTriggerRegistry : RegistryBase<EventTriggerRegistry, IEventTriggerBase>, IDumpableRegistry<EventTriggerRegistry, IEventTriggerBase>
    {
        protected override string RegistryName => "trigger";

        void IDumpableRegistry<EventTriggerRegistry, IEventTriggerBase>.DumpItem(StringBuilder contentBuilder, IEventTriggerBase entry)
        {
            IDumpableRegistry<EventTriggerRegistry, IEventTriggerBase>.DumpItemDefault(contentBuilder, entry);
            Type? dataType = entry.DataType;
            Type? settingsType = entry.SettingsType;

            if (dataType != null)
            {
                contentBuilder.AppendLine("Data:");
                contentBuilder.AppendLine();
                DumpingUtility.Dump(contentBuilder, dataType);
            }

            if (settingsType != null)
            {
                contentBuilder.AppendLine("Settings:");
                contentBuilder.AppendLine();
                DumpingUtility.Dump(contentBuilder, settingsType);
            }
        }

        public static Type? GetEntrySettingsType(string entryID)
        {
            if (!EventTriggerRegistry.TryGetEntry(entryID, out IEventTriggerBase? entry))
            {
                return null;
            }

            return entry.SettingsType;
        }

        protected override void OnItemRegistered(IEventTriggerBase entry)
        {
            entry.SetListener((data) =>
            {
                foreach (EventListenerItemData listener in DataManager.EventListeners)
                {
                    if (listener.Disabled)
                    {
                        continue;
                    }

                    if (listener.TriggerName != entry.Name)
                    {
                        continue;
                    }

                    if (!entry.SettingsAreValid(listener.TriggerSettings, data))
                    {
                        continue;
                    }

                    bool invalidCondition = listener.Conditions.Any((condition) =>
                    {
                        if (condition.Disabled)
                        {
                            return false;
                        }

                        if (!TriggerConditionRegistry.TryGetEntry(condition.ConditionName, out ITriggerConditionBase? triggerCondition))
                        {
                            Log.Warn(nameof(EventTriggerRegistry) + ".Trigger", $"Condition with debug name '{condition.DebugName}' has an invalid condition that couldn't be found ({condition.ConditionName}). Will be skipped!");
                            return false;
                        }

                        return !triggerCondition.IsValid(condition.Data);
                    });

                    if (invalidCondition)
                    {
                        continue;
                    }

                    listener.Activation.Activate();
                }

            });
        }
    }
}
