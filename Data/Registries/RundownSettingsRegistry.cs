using CustomExpeditionEvents.Utilities;
using CustomExpeditionEvents.Utilities.Registry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CustomExpeditionEvents.Data.Registries
{
    public sealed class RundownSettingsRegistry : IEnumerable<RundownSettingsItemData>
    {
        private readonly Dictionary<uint, RundownSettingsItemData> m_entries = new();

        internal RundownSettingsRegistry()
        { }

        public RundownSettingsItemData GetEntry(uint id)
        {
            if (!this.TryGetEntry(id, out RundownSettingsItemData? data))
            {
                throw new KeyNotFoundException($"No such rundown settings for id '{id}'");
            }

            return data;
        }

        public bool TryGetEntry(uint id, [NotNullWhen(true)] out RundownSettingsItemData? item)
        {
            return this.m_entries.TryGetValue(id, out item);
        }

        public void RegisterAll(IEnumerable<RundownSettingsItemData> entries)
        {
            foreach (RundownSettingsItemData entry in entries)
            {
                this.Register(entry);
            }
        }

        public void Register(RundownSettingsItemData entry)
        {
            if (entry is null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (entry.Disabled)
            {
                return;
            }


            RegistryLockManager.EnsureUnlocked("rundown-settings");

            if (this.m_entries.ContainsKey(entry.RundownID))
            {
                throw new ArgumentException("Rundown settings with id '" + entry.RundownID + "' is already registered");
            }

            this.m_entries.Add(entry.RundownID, entry);
            Log.Message($"Registered rundown settings for rundown id {entry.RundownID}");
        }

        public IEnumerator<RundownSettingsItemData> GetEnumerator()
        {
            return this.m_entries.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
