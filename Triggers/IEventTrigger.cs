using System;

namespace CustomExpeditionEvents.Triggers
{
    public interface IEventTriggerBase
    {
        string Name { get; }

        Type? SettingsType { get; }

        void SetListener(Action<object?> listener);
    }

    public interface IEventTrigger : IEventTriggerBase
    {
        Action? TriggerListener { get; set; }

        void IEventTriggerBase.SetListener(Action<object?> listener)
        {
            this.TriggerListener = () => listener.Invoke(null);
        }
        Type? IEventTriggerBase.SettingsType => null;
    }

    public interface IEventTrigger<TData> : IEventTriggerBase
    {
        Action<TData?>? TriggerListener { get; set; }

        void IEventTriggerBase.SetListener(Action<object?> listener)
        {
            this.TriggerListener = (TData? data) => listener.Invoke(data);
        }
        Type? IEventTriggerBase.SettingsType => typeof(TData);
    }
}
