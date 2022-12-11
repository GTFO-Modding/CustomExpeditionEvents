using System;

namespace CustomExpeditionEvents.Events
{
    public interface IEventBase
    {
        string Name { get; }

        Type? DataType { get; }

        void Activate(object? eventData);
    }

    public interface IEvent : IEventBase
    {
        void Activate();

        Type? IEventBase.DataType => null;

        void IEventBase.Activate(object? eventData)
        {
            this.Activate();
        }
    }

    public interface IEvent<TData> : IEventBase
    {

        Type IEventBase.DataType => typeof(TData);

        void IEventBase.Activate(object? data)
        {
            TData? tDat = (TData?)data;
            if (tDat == null)
            {
                return;
            }

            this.Activate(tDat);
        }

        void Activate(TData data);
    }
}
