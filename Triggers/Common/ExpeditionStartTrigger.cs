using System;

namespace CustomExpeditionEvents.Triggers.Common
{
    public sealed class ExpeditionStartTrigger : IEventTrigger
    {
        public string Name => "ExpeditionStart";

        public Action? TriggerListener
        {
            get => ExpeditionStartTrigger.s_triggerListener;
            set => ExpeditionStartTrigger.s_triggerListener = value;
        }

        private static Action? s_triggerListener;

        internal static void Trigger()
        {
            ExpeditionStartTrigger.s_triggerListener?.Invoke();
        }
    }
}
