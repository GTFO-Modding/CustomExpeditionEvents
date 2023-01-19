using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CustomExpeditionEvents.Utilities;
using CustomExpeditionEvents.Utilities.Registry;

namespace CustomExpeditionEvents.Data.Registries
{
    public sealed class ChainedPuzzleRegistry : IEnumerable<ChainedPuzzleItemData>
    {
        private readonly Dictionary<string, ChainedPuzzleItemData> m_entries = new();

        internal ChainedPuzzleRegistry()
        { }

        public ChainedPuzzleItemData GetEntry(string id)
        {
            if (!this.TryGetEntry(id, out ChainedPuzzleItemData? data))
            {
                throw new KeyNotFoundException($"No such puzzle with id '{id}'");
            }

            return data;
        }

        public bool TryGetEntry(string id, [NotNullWhen(true)] out ChainedPuzzleItemData? item)
        {
            return this.m_entries.TryGetValue(id, out item);
        }

        public void RegisterAll(IEnumerable<ChainedPuzzleItemData> entries)
        {
            foreach (ChainedPuzzleItemData entry in entries)
            {
                this.Register(entry);
            }
        }

        public void Register(ChainedPuzzleItemData entry)
        {
            if (entry is null)
            {
                throw new ArgumentNullException(nameof(entry));
            }


            string registryName = "chained-puzzle";
            RegistryLockManager.EnsureUnlocked(registryName);

            if (this.m_entries.ContainsKey(entry.Name))
            {
                throw new ArgumentException("A chained puzzle with id '" + entry.Name + "' is already registered");
            }

            this.m_entries.Add(entry.Name, entry);
            Log.Message($"Registered chained puzzle {entry.Name}");
        }

        public IEnumerator<ChainedPuzzleItemData> GetEnumerator()
        {
            return this.m_entries.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
