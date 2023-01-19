using System;
using System.Text;

namespace CustomExpeditionEvents.Utilities.Registry
{
    public abstract class DumpableRegistryWithDataBase<TSelf, TItem> : RegistryWithDataBase<TSelf, TItem>, IDumpableRegistry<TSelf, TItem>
        where TSelf : DumpableRegistryWithDataBase<TSelf, TItem>, new()
        where TItem : IRegistryItemWithData
    {
        protected virtual void DumpItem(StringBuilder contentBuilder, TItem entry)
        {
            IDumpableRegistry<TSelf, TItem>.DumpItemDefault(contentBuilder, entry);
            Type? dataType = entry.DataType;

            if (dataType != null)
            {
                contentBuilder.AppendLine();
                DumpingUtility.Dump(contentBuilder, dataType);
            }

        }

        void IDumpableRegistry<TSelf, TItem>.DumpItem(StringBuilder contentBuilder, TItem entry)
        {
            this.DumpItem(contentBuilder, entry);
        }
    }
}
