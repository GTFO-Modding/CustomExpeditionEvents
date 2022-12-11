using CustomExpeditionEvents.Events.Data;
using CustomExpeditionEvents.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomExpeditionEvents.Events
{
    public static class EventManager
    {
        private static readonly Dictionary<string, IEventBase> s_registeredEvents = new();

        internal static Type? GetEventDataType(string eventName)
        {
            if (!s_registeredEvents.TryGetValue(eventName, out IEventBase? ev))
            {
                return null;
            }

            return ev.DataType;
        }

        public static void Register<T>() where T : IEventBase, new()
        {
            T ev = new();

            if (EventManager.s_registeredEvents.ContainsKey(ev.Name))
            {
                throw new ArgumentException("An event with name '" + ev.Name + "' is already registered");
            }

            EventManager.s_registeredEvents.Add(ev.Name, ev);
            Log.Message($"Registered event {ev.Name} as {ev.GetType().FullName}");
        }

        public static void ActivateEventSequence(IEnumerable<EventData> events)
        {
            CoroutineUtility.Enqueue(PerformEventSequence(events));
        }

        private static IEnumerator SequenceEvent(EventData data)
        {
            bool isMaster = SNetwork.SNet.IsMaster;
            if (isMaster && data.SkipIfMaster)
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

            if (!EventManager.s_registeredEvents.TryGetValue(data.EventName, out IEventBase? ev))
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
