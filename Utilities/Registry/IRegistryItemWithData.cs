using System;

namespace CustomExpeditionEvents.Utilities.Registry
{
    public interface IRegistryItemWithData : IRegistryItem
    {
        Type? DataType { get; }
    }
}
