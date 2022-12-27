using CustomExpeditionEvents.Triggers.Common;
using CustomExpeditionEvents.Utilities;
using HarmonyLib;

namespace CustomExpeditionEvents.Patches
{
    [HarmonyPatch]
    internal static class CommonTriggerPatches
    {
        [HarmonyPatch(typeof(WardenObjectiveManager), nameof(WardenObjectiveManager.OnLocalPlayerStartExpedition))]
        [HarmonyPostfix]
        public static void OnExpeditionStart()
        {
            ExpeditionStartTrigger.Trigger();
        }

        [HarmonyPatch(typeof(WardenObjectiveManager), nameof(WardenObjectiveManager.OnStateChange))]
        [HarmonyPrefix]
        public static void CompleteObjectivePatch(pWardenObjectiveState oldState, pWardenObjectiveState newState)
        {
            static ObjectiveBitMask GetSolvedBitMask(ObjectiveBitMask mask, eWardenObjectiveStatus status)
            {
                if (status == eWardenObjectiveStatus.WardenObjectiveItemSolved)
                {
                    return mask;
                }
                return 0;
            }

            static ObjectiveBitMask GetChangedBitMask(ObjectiveBitMask mask, eWardenObjectiveStatus oldStatus, eWardenObjectiveStatus newStatus)
            {
                if (oldStatus == newStatus)
                {
                    return mask;
                }

                return 0;
            }

            ObjectiveBitMask currentObjectiveState =
                GetSolvedBitMask(ObjectiveBitMask.MAIN, newState.main_status) |
                GetSolvedBitMask(ObjectiveBitMask.SECOND, newState.second_status) |
                GetSolvedBitMask(ObjectiveBitMask.THIRD, newState.third_status);

            ObjectiveBitMask changedObjectiveStateMask =
                GetChangedBitMask(ObjectiveBitMask.MAIN, oldState.main_status, newState.main_status) |
                GetChangedBitMask(ObjectiveBitMask.SECOND, oldState.second_status, newState.second_status) |
                GetChangedBitMask(ObjectiveBitMask.THIRD, oldState.third_status, newState.third_status);

            ObjectiveBitMask solvedObjectives = currentObjectiveState & changedObjectiveStateMask;

            ObjectiveCompleteTrigger.Trigger(solvedObjectives, currentObjectiveState);
        }
    }
}
