using CustomExpeditionEvents.Utilities.Registry;

namespace CustomExpeditionEvents.Events
{
    public sealed class EventRegistry : DumpableRegistryWithDataBase<EventRegistry, IEventBase>
    {
        /// <inheritdoc/>
        protected override string RegistryName => "event";
    }
}
