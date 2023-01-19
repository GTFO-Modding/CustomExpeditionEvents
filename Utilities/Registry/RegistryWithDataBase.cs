using System;

namespace CustomExpeditionEvents.Utilities.Registry
{
    public abstract class RegistryWithDataBase<TSelf, TItem> : RegistryBase<TSelf, TItem>
        where TSelf : RegistryWithDataBase<TSelf, TItem>, new()
        where TItem : IRegistryItemWithData
    {
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
