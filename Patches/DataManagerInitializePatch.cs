using CustomExpeditionEvents.Data;
using HarmonyLib;

namespace CustomExpeditionEvents.Patches
{
    [HarmonyPatch]
    internal sealed class DataManagerInitializePatch
    {
        [HarmonyPatch(typeof(GS_Offline), nameof(GS_Offline.Enter))]
        [HarmonyPostfix]
        public static void Load()
        {
            DataManager.Initialize();
        }
    }
}
