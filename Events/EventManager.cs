using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomExpeditionEvents.Events
{
    internal static class EventManager
    {
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
