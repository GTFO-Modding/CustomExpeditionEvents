namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class EmptyEvent : IEvent
    {
        public string Name => "Empty";

        public void Activate()
        { }
    }
}