using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomExpeditionEvents.Conditions;
using CustomExpeditionEvents.Conditions.Common;
using CustomExpeditionEvents.Config;
using CustomExpeditionEvents.Events;
using CustomExpeditionEvents.Events.Common;
using CustomExpeditionEvents.Triggers;
using CustomExpeditionEvents.Triggers.Common;
using GameData;
using HarmonyLib;
using static CustomExpeditionEvents.PLUGIN_CONSTANTS;

namespace CustomExpeditionEvents
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    internal sealed class Plugin : BasePlugin
    {
        public override void Load()
        {
            PluginConfig.Load(this.Config);

            // Events
            EventRegistry.Register<ActivateChainedPuzzleEvent>();
            EventRegistry.Register<ActivateSurvivalWaveEvent>();
            EventRegistry.Register<FogTransitionEvent>();
            EventRegistry.Register<OpenSecurityDoorEvent>();
            EventRegistry.Register<PlaySoundEvent>();
            EventRegistry.Register<StopCustomSurvivalWaveEvent>();
            EventRegistry.Register<UnlockSecurityDoorEvent>();
            EventRegistry.Register<WardenIntelEvent>();
            EventRegistry.Register<SetDataEvent>();
            EventRegistry.Register<EmptyEvent>();

            // Triggers
            EventTriggerRegistry.Register<ExpeditionStartTrigger>();
            EventTriggerRegistry.Register<ObjectiveCompleteTrigger>();

            // Conditions
            TriggerConditionRegistry.Register<DataValidateCondition>();

            Harmony patcher = new Harmony(PLUGIN_GUID);
            patcher.PatchAll();
        }
    }
}