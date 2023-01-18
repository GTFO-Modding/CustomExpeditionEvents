using CustomExpeditionEvents.Events;

namespace CustomExpeditionEvents.Data
{
    public sealed class EventListenerItemActivationData
    {
        public string EventSequence { get; set; } = string.Empty;

        public void Activate()
        {
            EventManager.ActivateEventSequence(this.EventSequence);
        }
    }
}