using System;

namespace CustomExpeditionEvents.Conditions
{
    public interface ITriggerConditionBase
    {
        string Name { get; }

        Type? DataType { get; }
    }

    public interface ITriggerCondition : ITriggerConditionBase
    {
        Type? ITriggerConditionBase.DataType => null;
    }

    public interface ITriggerCondition<TData> : ITriggerConditionBase
    {
        Type? ITriggerConditionBase.DataType => typeof(TData);
    }
}
