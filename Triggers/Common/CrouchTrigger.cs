using System;
using SNetwork;
using Player;

namespace CustomExpeditionEvents.Triggers.Common
{
    internal sealed class CrouchTrigger : IEventTrigger<CrouchTrigger.Settings, CrouchTrigger.Data>
    {
        public Action<Data>? TriggerListener
        {
            get => CrouchTrigger.s_triggerListener;
            set => CrouchTrigger.s_triggerListener = value;
        }

        public string Name => "Crouch";

        public bool SettingsAreValid(Settings settings, Data triggerData)
        {
            if (settings.IsBot.HasValue && settings.IsBot.Value != triggerData.Player.IsBot)
            {
                return false;
            }
            if (settings.IsLocalPlayer.HasValue && settings.IsLocalPlayer.Value != triggerData.Player.IsLocal)
            {
                return false;
            }
            return true;
        }

        internal static Action<Data>? s_triggerListener;

        internal static void Trigger(PlayerAgent player)
        {
            CrouchTrigger.s_triggerListener?.Invoke(new Data()
            {
                Player = player.Owner
            });
        }

        public sealed class Data
        {
            public SNet_Player Player { get; set; }
        }

        public sealed class Settings
        {
            public bool? IsLocalPlayer { get; set; }
            public bool? IsBot { get; set; }
        }
    }
}
