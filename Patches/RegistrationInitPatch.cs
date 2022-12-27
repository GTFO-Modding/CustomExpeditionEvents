using CustomExpeditionEvents.Triggers;
using CustomExpeditionEvents.Utilities;
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
        }
    }
}
