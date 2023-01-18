using CustomExpeditionEvents.Utilities.Registry;
using System;

namespace CustomExpeditionEvents.Conditions
{
    public interface ITriggerConditionBase : IRegistryItemWithData
    {
        string Name { get; }

        string IRegistryItem.ID => this.Name;

        bool IsValid(object? data);
    }

    public interface ITriggerCondition : ITriggerConditionBase
    {
        Type? IRegistryItemWithData.DataType => null;

        bool IsValid();

        bool ITriggerConditionBase.IsValid(object? data)
        {
            return this.IsValid();
        }
    }

    public interface ITriggerCondition<TData> : ITriggerConditionBase
    {
        Type? IRegistryItemWithData.DataType => typeof(TData);

        bool IsValid(TData data);

        bool ITriggerConditionBase.IsValid(object? data)
        {
            if (data is not TData tData)
            {
                return false;
            }

            return this.IsValid(tData);
        }
    }
}
