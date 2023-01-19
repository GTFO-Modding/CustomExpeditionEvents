using CustomExpeditionEvents.Conditions;
using CustomExpeditionEvents.Config;
using CustomExpeditionEvents.Events;
using CustomExpeditionEvents.Triggers;
using CustomExpeditionEvents.Utilities.Registry;
using HarmonyLib;

namespace CustomExpeditionEvents.Patches
{
    [HarmonyPatch]
    internal static class RegistrationInitPatch
    {
        [HarmonyPatch(typeof(GS_Offline), nameof(GS_Offline.Exit))]
        [HarmonyPostfix]
        public static void Hook()
        {
            RegistryLockManager.Lock();

            if (!PluginConfig.Current.DumpingEnabled)
            {
                return;
            }

            if (PluginConfig.Current.DumpEvents)
            {
                IDumpableRegistry<EventRegistry, IEventBase>.Dump();
            }

            if (PluginConfig.Current.DumpConditions)
            {
                IDumpableRegistry<TriggerConditionRegistry, ITriggerConditionBase>.Dump();
            }

            if (PluginConfig.Current.DumpTriggers)
            {
                IDumpableRegistry<EventTriggerRegistry, IEventTriggerBase>.Dump();
            }
        }
    }
}
