using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomExpeditionEvents.Events;
using CustomExpeditionEvents.Events.Common;
using GameData;
using static CustomExpeditionEvents.PLUGIN_CONSTANTS;

namespace CustomExpeditionEvents
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    internal sealed class Plugin : BasePlugin
    {
        public override void Load()
        {
            EventManager.Register<ActivateChainedPuzzleEvent>();
            EventManager.Register<ActivateSurvivalWaveEvent>();
            EventManager.Register<FogTransitionEvent>();
            EventManager.Register<OpenSecurityDoorEvent>();
            EventManager.Register<PlaySoundEvent>();
            EventManager.Register<UnlockSecurityDoorEvent>();
            EventManager.Register<WardenIntelEvent>();
        }
    }
}