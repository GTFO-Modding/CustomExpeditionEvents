using CustomExpeditionEvents.Utilities.Registry;
using System;

namespace CustomExpeditionEvents.Events
{
    public interface IEventBase : IRegistryItemWithData
    {
        string Name { get; }

        void Activate(object? eventData);

        string IRegistryItem.ID => this.Name;
    }

    /// <summary>
    /// An event without any data.
    /// </summary>
    public interface IEvent : IEventBase
    {
        void Activate();

        Type? IRegistryItemWithData.DataType => null;

        void IEventBase.Activate(object? eventData)
        {
            this.Activate();
        }
    }

    /// <summary>
    /// An event with some data
    /// </summary>
    /// <typeparam name="TData">The data for the event</typeparam>
    public interface IEvent<TData> : IEventBase
    {

        Type IRegistryItemWithData.DataType => typeof(TData);

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
