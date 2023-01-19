using CustomExpeditionEvents.Utilities;
using CustomExpeditionEvents.Utilities.Registry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CustomExpeditionEvents.Data.Registries
{
    public sealed class EventSequenceRegistry : IEnumerable<EventSequenceItemData>
    {
        private readonly Dictionary<string, EventSequenceItemData> m_entries = new();

        internal EventSequenceRegistry()
        { }

        public EventSequenceItemData GetEntry(string id)
        {
            if (!this.TryGetEntry(id, out EventSequenceItemData? data))
            {
                throw new KeyNotFoundException($"No such event sequence with id '{id}'");
            }

            return data;
        }

        public bool TryGetEntry(string id, [NotNullWhen(true)] out EventSequenceItemData? item)
        {
            return this.m_entries.TryGetValue(id, out item);
        }

        public void RegisterAll(IEnumerable<EventSequenceItemData> entries)
        {
            foreach (EventSequenceItemData entry in entries)
            {
                this.Register(entry);
            }
        }

        public void Register(EventSequenceItemData entry)
        {
            if (entry is null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (entry.Disabled)
            {
                return;
            }


            string registryName = "event-sequence";
            RegistryLockManager.EnsureUnlocked(registryName);

            if (this.m_entries.ContainsKey(entry.Name))
            {
                throw new ArgumentException("An event sequence with id '" + entry.Name + "' is already registered");
            }

            this.m_entries.Add(entry.Name, entry);
            Log.Message($"Registered event sequence {entry.Name}");
        }

        public IEnumerator<EventSequenceItemData> GetEnumerator()
        {
            return this.m_entries.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
