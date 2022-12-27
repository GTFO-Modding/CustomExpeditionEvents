using CustomExpeditionEvents.Utilities;
using System;

namespace CustomExpeditionEvents.Triggers.Common
{
    public sealed class ObjectiveCompleteTrigger : IEventTrigger<ObjectiveCompleteTrigger.Data>
    {
        public string Name => "ObjectiveComplete";

        public Action<Data>? TriggerListener
        {
            get => ObjectiveCompleteTrigger.s_triggerListener;
            set => ObjectiveCompleteTrigger.s_triggerListener = value;
        }

        private static Action<Data>? s_triggerListener;

        internal static void Trigger(ObjectiveBitMask newSolved, ObjectiveBitMask currentSolved)
        {
            ObjectiveCompleteTrigger.s_triggerListener?.Invoke(new Data(NewSolved: newSolved, CurrentSolved: currentSolved));
        }

        public record struct Data(ObjectiveBitMask NewSolved, ObjectiveBitMask CurrentSolved);
    }
}
