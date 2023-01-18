using System;
using System.Text;

namespace CustomExpeditionEvents.Utilities.Registry
{
    public abstract class RegistryWithDataBase<TSelf, TItem> : RegistryBase<TSelf, TItem>
        where TSelf : RegistryWithDataBase<TSelf, TItem>, new()
        where TItem : IRegistryItemWithData
    {
        protected override void DumpItem(StringBuilder contentBuilder, TItem entry)
        {
            base.DumpItem(contentBuilder, entry);
            Type? dataType = entry.DataType;

            if (dataType != null)
            {
                contentBuilder.AppendLine();
                DumpingUtility.Dump(contentBuilder, dataType);
            }
        }

        public static Type? GetEntryDataType(string entryID)
        {
            if (!RegistryWithDataBase<TSelf, TItem>.TryGetEntry(entryID, out TItem? entry))
            {
                return null;
            }

            return entry.DataType;
        }
    }
}
