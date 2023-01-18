using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.IO;

namespace CustomExpeditionEvents.Utilities.Registry
{
    public abstract class RegistryBase<TSelf, TItem>
        where TSelf : RegistryBase<TSelf, TItem>, new()
        where TItem : IRegistryItem
    {
        private readonly Dictionary<string, TItem> m_entries = new();

        protected abstract string RegistryName { get; }

        protected virtual void OnItemRegistered(TItem entry)
        { }

        protected void DumpItemDefault(StringBuilder contentBuilder, TItem entry)
        {
            contentBuilder.AppendLine("=== " + entry.ID + " ===");
            Type entryType = entry.GetType();
            DescriptionAttribute? descriptionAttr = entryType.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttr != null)
            {
                contentBuilder.Append("Description: ");
                contentBuilder.AppendLine(descriptionAttr.Description);
            }
            else
            {
                contentBuilder.AppendLine("No description");
            }
        }

        protected virtual void DumpItem(StringBuilder contentBuilder, TItem entry)
        {
            this.DumpItemDefault(contentBuilder, entry);
        }

        public static void Dump()
        {
            StringBuilder contentBuilder = new();

            foreach (TItem entry in RegistryBase<TSelf, TItem>.Entries.Values)
            {
                RegistryBase<TSelf, TItem>.Current.DumpItem(contentBuilder, entry);
            }

            string dumpPath = Path.Combine(BepInEx.Paths.BepInExRootPath, "dumps", "CustomExpeditionEvents");
            if (!Directory.Exists(dumpPath))
            {
                Directory.CreateDirectory(dumpPath);
            }

            string fileDumpPath = Path.Combine(dumpPath, RegistryBase<TSelf, TItem>.Current.RegistryName + " dump.txt");
            File.WriteAllText(fileDumpPath, contentBuilder.ToString());
        }

        public static bool TryGetEntry(string id, [NotNullWhen(true)] out TItem? item)
        {
            return RegistryBase<TSelf, TItem>.Entries.TryGetValue(id, out item);
        }

        public static void Register<T>()
            where T : TItem, new()
        {
            Dictionary<string, TItem> entries = RegistryBase<TSelf, TItem>.Entries;
            string registryName = RegistryBase<TSelf, TItem>.Current.RegistryName;
            RegistryLockManager.EnsureUnlocked(registryName);

            T entry = new();

            if (entries.ContainsKey(entry.ID))
            {
                throw new ArgumentException("A " + registryName + " with id '" + entry.ID + "' is already registered");
            }

            entries.Add(entry.ID, entry);
            Log.Message($"Registered {registryName} {entry.ID} as {entry.GetType().FullName}");

            RegistryBase<TSelf, TItem>.Current.OnItemRegistered(entry);
        }

        public static TSelf Current { get; }

        protected static Dictionary<string, TItem> Entries => RegistryBase<TSelf, TItem>.Current.m_entries;

        static RegistryBase()
        {
            RegistryBase<TSelf, TItem>.Current = new();
        }
    }
}
