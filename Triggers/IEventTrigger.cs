using CustomExpeditionEvents.Utilities.Registry;
using System;

namespace CustomExpeditionEvents.Triggers
{
    public interface IEventTriggerBase : IRegistryItemWithData
    {
        string Name { get; }

        Type? SettingsType { get; }
        new Type? DataType { get; }

        void SetListener(Action<object?> listener);

        bool SettingsAreValid(object? settings, object? triggerData)
        {
            return true;
        }

        string IRegistryItem.ID => this.Name;

        Type? IRegistryItemWithData.DataType => this.SettingsType;
    }

    public interface IEventTrigger : IEventTriggerBase
    {
        Action? TriggerListener { get; set; }

        void IEventTriggerBase.SetListener(Action<object?> listener)
        {
            this.TriggerListener = () => listener.Invoke(null);
        }
        Type? IEventTriggerBase.SettingsType => null;
        Type? IEventTriggerBase.DataType => null;
    }

    public interface IEventTrigger<TData> : IEventTrigger<TData, TData>
    { }

    public interface IEventTrigger<TSettings, TData> : IEventTriggerBase
    {
        Action<TData?>? TriggerListener { get; set; }

        void IEventTriggerBase.SetListener(Action<object?> listener)
        {
            this.TriggerListener = (TData? data) => listener.Invoke(data);
        }

        bool SettingsAreValid(TSettings settings, TData triggerData);

        bool IEventTriggerBase.SettingsAreValid(object? settings, object? triggerData)
        {
            if (settings is not TSettings tSettings || triggerData is not TData tTriggerData)
            {
                return false;
            }

            return this.SettingsAreValid(tSettings, tTriggerData);
        }


        Type? IEventTriggerBase.SettingsType => typeof(TSettings);
        Type? IEventTriggerBase.DataType => typeof(TData);
    }
}
