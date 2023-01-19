using Player;
using SNetwork;
using System;

namespace CustomExpeditionEvents.Triggers.Common
{
    internal sealed class JumpTrigger : IEventTrigger<JumpTrigger.Settings, JumpTrigger.Data>
    {
        public Action<Data>? TriggerListener
        {
            get => JumpTrigger.s_triggerListener;
            set => JumpTrigger.s_triggerListener = value;
        }

        public string Name => "Jump";

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
            JumpTrigger.s_triggerListener?.Invoke(new Data()
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
