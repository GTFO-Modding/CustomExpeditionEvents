using CustomExpeditionEvents.Utilities;
using System;

namespace CustomExpeditionEvents.Triggers.Common
{
    internal sealed class ObjectiveCompleteTrigger : IEventTrigger<ObjectiveCompleteTrigger.Settings, ObjectiveCompleteTrigger.Data>
    {
        public string Name => "ObjectiveComplete";

        public Action<Data>? TriggerListener
        {
            get => ObjectiveCompleteTrigger.s_triggerListener;
            set => ObjectiveCompleteTrigger.s_triggerListener = value;
        }

        private static Action<Data>? s_triggerListener;

        public bool SettingsAreValid(Settings settings, Data activationData)
        {
            return ObjectiveCompleteTrigger.IsSolveSettingValid(settings.MainObjective, activationData.NewSolved, activationData.CurrentSolved, ObjectiveBitMask.MAIN) &&
                ObjectiveCompleteTrigger.IsSolveSettingValid(settings.SecondObjective, activationData.NewSolved, activationData.CurrentSolved, ObjectiveBitMask.SECOND) &&
                ObjectiveCompleteTrigger.IsSolveSettingValid(settings.ThirdObjective, activationData.NewSolved, activationData.CurrentSolved, ObjectiveBitMask.THIRD);
        }

        private static bool IsSolveSettingValid(SolveSetting setting, ObjectiveBitMask newSolved, ObjectiveBitMask oldSolved)
        {
            return setting switch
            {
                SolveSetting.Ignore => true,
                SolveSetting.WasSolved => newSolved != oldSolved,
                SolveSetting.IsSolved => oldSolved != 0,
                _ => true
            };
        }

        private static bool IsSolveSettingValid(SolveSetting setting, ObjectiveBitMask newSolved, ObjectiveBitMask oldSolved, ObjectiveBitMask mask)
        {
            return ObjectiveCompleteTrigger.IsSolveSettingValid(setting, newSolved & mask, oldSolved & mask);
        }

        internal static void Trigger(ObjectiveBitMask newSolved, ObjectiveBitMask currentSolved)
        {
            ObjectiveCompleteTrigger.s_triggerListener?.Invoke(new Data(NewSolved: newSolved, CurrentSolved: currentSolved));
        }

        public record struct Data(ObjectiveBitMask NewSolved, ObjectiveBitMask CurrentSolved);

        public sealed class Settings
        {
            public SolveSetting MainObjective { get; set; }
            public SolveSetting SecondObjective { get; set; }
            public SolveSetting ThirdObjective { get; set; }
        }

        public enum SolveSetting : byte
        {
            Ignore,
            WasSolved,
            IsSolved
        }
    }
}
