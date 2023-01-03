using CustomExpeditionEvents.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CustomExpeditionEvents.Events
{
    public static class EventRegistry
    {
        private static readonly Dictionary<string, IEventBase> s_registeredEvents = new();

        public static void Register<T>() where T : IEventBase, new()
        {
            RegistryLockManager.EnsureUnlocked("event");

            T ev = new();

            if (EventRegistry.s_registeredEvents.ContainsKey(ev.Name))
            {
                throw new ArgumentException("An event with name '" + ev.Name + "' is already registered");
            }

            EventRegistry.s_registeredEvents.Add(ev.Name, ev);
            Log.Message($"Registered event {ev.Name} as {ev.GetType().FullName}");
        }


        public static bool TryGetEntry(string name, [NotNullWhen(true)] out IEventBase? entry)
        {
            return EventRegistry.s_registeredEvents.TryGetValue(name, out entry);
        }

        internal static Type? GetEventDataType(string eventName)
        {
            if (!EventRegistry.s_registeredEvents.TryGetValue(eventName, out IEventBase? ev))
            {
                return null;
            }

            return ev.DataType;
        }

        
    }
}
