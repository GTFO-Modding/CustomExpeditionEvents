using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomExpeditionEvents.Events;
using CustomExpeditionEvents.Events.Common;
using CustomExpeditionEvents.Triggers;
using CustomExpeditionEvents.Triggers.Common;
using GameData;
using static CustomExpeditionEvents.PLUGIN_CONSTANTS;

namespace CustomExpeditionEvents
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    internal sealed class Plugin : BasePlugin
    {
        public override void Load()
        {

            // Events
            EventRegistry.Register<ActivateChainedPuzzleEvent>();
            EventRegistry.Register<ActivateSurvivalWaveEvent>();
            EventRegistry.Register<FogTransitionEvent>();
            EventRegistry.Register<OpenSecurityDoorEvent>();
            EventRegistry.Register<PlaySoundEvent>();
            EventRegistry.Register<StopCustomSurvivalWaveEvent>();
            EventRegistry.Register<UnlockSecurityDoorEvent>();
            EventRegistry.Register<WardenIntelEvent>();

            // Triggers
            EventTriggerRegistry.Register<ExpeditionStartTrigger>();
            EventTriggerRegistry.Register<ObjectiveCompleteTrigger>();
        }
    }
}