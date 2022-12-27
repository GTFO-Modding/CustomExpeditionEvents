using CustomExpeditionEvents.Utilities;
using System;
using System.Collections.Generic;

namespace CustomExpeditionEvents.Triggers
{
    public static class EventTriggerRegistry
    {
        private static readonly Dictionary<string, IEventTriggerBase> s_registeredTriggers = new();

        public static void Register<T>()
            where T : IEventTriggerBase, new()
        {
            RegistryLockManager.EnsureUnlocked("trigger");

            T trigger = new();

            if (EventTriggerRegistry.s_registeredTriggers.ContainsKey(trigger.Name))
            {
                throw new ArgumentException("A trigger with name '" + trigger.Name + "' is already registered");
            }

            EventTriggerRegistry.s_registeredTriggers.Add(trigger.Name, trigger);
            Log.Message($"Registered trigger {trigger.Name} as {trigger.GetType().FullName}");
        }


        internal static Type? GetTriggerSettingsType(string triggerName)
        {
            if (!EventTriggerRegistry.s_registeredTriggers.TryGetValue(triggerName, out IEventTriggerBase? trigger))
            {
                return null;
            }

            return trigger.SettingsType;
        }
    }
}
