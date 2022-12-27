using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


        internal static Type? GetEventDataType(string eventName)
        {
            if (!EventRegistry.s_registeredEvents.TryGetValue(eventName, out IEventBase? ev))
            {
                return null;
            }

            return ev.DataType;
        }

        public static void ActivateEventSequence(IEnumerable<EventData> events)
        {
            CoroutineUtility.Enqueue(PerformEventSequence(events));
        }

        private static IEnumerator SequenceEvent(EventData data)
        {
            bool isMaster = SNetwork.SNet.IsMaster;
            if (isMaster && data.SkipIfHost)
            {
                yield break;
            }
            else if (!isMaster && data.SkipIfClient)
            {
                yield break;
            }

            if (data.EventDelay > 0)
            {
                yield return new WaitForSeconds(data.EventDelay);
            }

            if (!EventRegistry.s_registeredEvents.TryGetValue(data.EventName, out IEventBase? ev))
            {
                Log.Warn("Unknown event with name '" + data.EventName + "'");
                yield break;
            }

            ev.Activate(data.Data);

        }

        private static IEnumerator PerformEventSequence(IEnumerable<EventData> sequence)
        {
            foreach (EventData data in sequence)
            {
                CoroutineUtility.Enqueue(SequenceEvent(data));

                if (data.SequenceDelay > 0)
                {
                    yield return new WaitForSeconds(data.SequenceDelay);
                }
            }
        }
    }
}
