using CustomExpeditionEvents.Utilities;
using System;
using System.Collections.Generic;

namespace CustomExpeditionEvents.Conditions
{
    public static class TriggerConditionRegistry
    {
        private static readonly Dictionary<string, ITriggerConditionBase> s_registeredConditions = new();

        public static void Register<T>() where T : ITriggerConditionBase, new()
        {
            RegistryLockManager.EnsureUnlocked("condition");

            T ev = new();

            if (TriggerConditionRegistry.s_registeredConditions.ContainsKey(ev.Name))
            {
                throw new ArgumentException("A condition with name '" + ev.Name + "' is already registered");
            }

            TriggerConditionRegistry.s_registeredConditions.Add(ev.Name, ev);
            Log.Message($"Registered condition {ev.Name} as {ev.GetType().FullName}");
        }


        internal static Type? GetConditionDataType(string conditionName)
        {
            if (!TriggerConditionRegistry.s_registeredConditions.TryGetValue(conditionName, out ITriggerConditionBase? condition))
            {
                return null;
            }

            return condition.DataType;
        }
    }
}
