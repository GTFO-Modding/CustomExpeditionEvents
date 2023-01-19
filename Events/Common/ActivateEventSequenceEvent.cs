using System.ComponentModel;

namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class ActivateEventSequenceEvent : IEvent<ActivateEventSequenceEvent.Data>
    {
        public string Name => "ActivateEventSequence";

        public void Activate(Data data)
        {
            EventManager.ActivateEventSequence(data.SequenceID);
        }

        public sealed class Data
        {
            [Description("The ID of the Event Sequence to Activate")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public string SequenceID { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        }
    }
}
