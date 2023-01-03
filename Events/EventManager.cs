using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomExpeditionEvents.Events
{
    public static class EventManager
    {
        public static void ActivateEventSequence(string name)
        {
            EventSequenceItemData? data = DataManager.EventSequences.FirstOrDefault((e) => e.Name == name && !e.Disabled);
            if (data == null)
            {
                Log.Warn("No such event sequence with name '" + name + "' exists!");
                return;
            }

            Log.Verbose($"Activating Event Sequence '{data.Name}' - {data.DebugName}");
            ActivateEventSequence(data.Events);
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

            if (!EventRegistry.TryGetEntry(data.EventName, out IEventBase? ev))
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
