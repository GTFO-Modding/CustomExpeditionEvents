using CustomExpeditionEvents.Rundown.Jobs;
using HarmonyLib;
using LevelGeneration;

namespace CustomExpeditionEvents.Patches.Jobs
{
    [HarmonyPatch]
    internal static class PresetChainedPuzzlesJobPatches
    {
        [HarmonyPatch(typeof(LG_BuildFloorJob), nameof(LG_BuildFloorJob.Build))]
        [HarmonyPostfix]
        public static void InjectJob()
        {
            // batch to use may need to be changed.
            LG_Factory.InjectJob(JobManager.CreateJob<PresetChainedPuzzlesJob>(), LG_Factory.BatchName.LateGeomorphBuildStep);
        }
    }
}
