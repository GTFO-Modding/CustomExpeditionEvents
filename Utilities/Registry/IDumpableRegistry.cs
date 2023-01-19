using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace CustomExpeditionEvents.Utilities.Registry
{
    public interface IDumpableRegistry<TSelf, TItem>
        where TItem : IRegistryItem
        where TSelf : RegistryBase<TSelf, TItem>, IDumpableRegistry<TSelf, TItem>, new()
    {
        void DumpItem(StringBuilder contentBuilder, TItem entry)
        {
            IDumpableRegistry<TSelf, TItem>.DumpItemDefault(contentBuilder, entry);
        }

        public static void DumpItemDefault(StringBuilder contentBuilder, TItem entry)
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

        public static void Dump()
        {
            StringBuilder contentBuilder = new();

            foreach (TItem entry in RegistryBase<TSelf, TItem>.GetEntries())
            {
                RegistryBase<TSelf, TItem>.Current.DumpItem(contentBuilder, entry);
            }

            string dumpPath = Path.Combine(BepInEx.Paths.BepInExRootPath, "dumps", "CustomExpeditionEvents");
            if (!Directory.Exists(dumpPath))
            {
                Directory.CreateDirectory(dumpPath);
            }

            string fileDumpPath = Path.Combine(dumpPath, RegistryBase<TSelf, TItem>.RegistryID + " dump.txt");
            File.WriteAllText(fileDumpPath, contentBuilder.ToString());
        }
    }
}
