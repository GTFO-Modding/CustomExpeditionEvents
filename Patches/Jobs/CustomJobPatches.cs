using CustomExpeditionEvents.Rundown.Jobs;
using HarmonyLib;
using LevelGeneration;

namespace CustomExpeditionEvents.Patches.Jobs
{
#if !IL2CPP_INHERITANCE
    [HarmonyPatch]
    internal static class CustomJobPatches
    {
        [HarmonyPatch(typeof(LG_FactoryJob), nameof(LG_FactoryJob.Build))]
        [HarmonyPrefix]
        public static bool Build(LG_FactoryJob __instance, ref bool __result)
        {
            bool? result = JobManager.InvokeBuild(__instance);
            if (result.HasValue)
            {
                __result = result.Value;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(LG_FactoryJob), nameof(LG_FactoryJob.GetName))]
        [HarmonyPrefix]
        public static bool GetName(LG_FactoryJob __instance, ref string __result)
        {
            string? result = JobManager.InvokeGetName(__instance);
            if (result != null)
            {
                __result = result;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(LG_FactoryJob), nameof(LG_FactoryJob.TakeFullFrame))]
        [HarmonyPrefix]
        public static bool TakeFullFrame(LG_FactoryJob __instance, ref bool __result)
        {
            bool? result = JobManager.InvokeTakeFullFrame(__instance);
            if (result.HasValue)
            {
                __result = result.Value;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(GS_ReadyToStopElevatorRide), nameof(GS_ReadyToStopElevatorRide.Enter))]
        [HarmonyPatch(typeof(GS_AfterLevel), nameof(GS_AfterLevel.Enter))]
        [HarmonyPatch(typeof(GS_Lobby), nameof(GS_Lobby.Enter))]
        [HarmonyPrefix]
        public static void Cleanup()
        {
            JobManager.Cleanup();
        }
    }
#endif
}
