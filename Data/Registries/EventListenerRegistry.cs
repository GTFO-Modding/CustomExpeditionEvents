using CustomExpeditionEvents.Utilities;
using CustomExpeditionEvents.Utilities.Registry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomExpeditionEvents.Data.Registries
{
    public sealed class EventListenerRegistry : IEnumerable<EventListenerItemData>
    {
        private readonly List<EventListenerItemData> m_items = new();

        internal EventListenerRegistry()
        { }

        public void RegisterAll(IEnumerable<EventListenerItemData> items)
        {
            foreach (EventListenerItemData item in items)
            {
                this.Register(item);
            }
        }

        public void Register(EventListenerItemData item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            RegistryLockManager.EnsureUnlocked("event-listener");

            this.m_items.Add(item);
            Log.Message($"Registered event listener '{item.DebugName}'");
        }

        public IEnumerable<EventListenerItemData> GetEntries() => new ReadOnlyCollection<EventListenerItemData>(this.m_items);

        public IEnumerator<EventListenerItemData> GetEnumerator() => this.m_items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
