using CustomExpeditionEvents.Config;
using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace CustomExpeditionEvents.Events
{
    public static class EventManager
    {
        private sealed class Sequence : IEnumerable
        {
            private readonly Dictionary<int, EventData> m_data;
            private readonly List<pSequenceEventState> m_events = new();
            private readonly string? m_sequenceName;
            private Coroutine? m_routine;
            private pSequenceState m_state;

            public Sequence(IEnumerable<EventData> events, string? sequenceName)
            {
                this.m_data = new();
                int startPosition = 1;
                foreach (EventData data in events)
                {
                    this.m_data[data.EventPosition] = data;
                    startPosition = Mathf.Min(startPosition, data.EventPosition);
                }
                this.m_state = new()
                {
                    delayTime = 0,
                    position = startPosition
                };

                this.m_sequenceName = sequenceName;
            }

            public void Start()
            {
                this.Stop();
                this.m_routine = CoroutineUtility.Enqueue(this.GetEnumerator());

                lock (EventManager.s_sequencesLock)
                {
                    EventManager.s_sequences.Add(this);
                }
            }

            public void Stop()
            {
                if (this.m_routine != null)
                {
                    CoroutineUtility.Dequeue(this.m_routine);
                    this.m_routine = null;

                    lock (EventManager.s_sequencesLock)
                    {
                        EventManager.s_sequences.Remove(this);
                    }
                }
            }

            public bool Tick()
            {
                return this.m_state.Tick(this) | this.m_events.Aggregate(false, (v, e) => e.Tick(this) | v);
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct pSequenceState
            {
                public int position;
                public bool finished;
                public float delayTime;

                public bool Tick(Sequence sequence)
                {
                    if (this.finished)
                    {
                        return false;
                    }

                    sequence.m_data.TryGetValue(this.position, out EventData? data);
                    bool isMaster = SNetwork.SNet.IsMaster;

                    int eventTicks = 0;
                    int maxEventTicks = PluginConfig.Current.MaxEventTicks;

                    while (data != null && // data exists
                        this.delayTime >= data.SequenceDelay && // not delayed
                        (
                            maxEventTicks <= 0 || // infinite event ticks
                            eventTicks < maxEventTicks // normal cost check
                        ) // make sure we dont have too many ticks
                    )
                    {
                        this.delayTime -= data.SequenceDelay;
                        if (!(isMaster && data.SkipIfHost) && !(!isMaster && data.SkipIfClient))
                        {
                            sequence.m_events.Add(new pSequenceEventState()
                            {
                                eventPosition = data.EventPosition,
                                delayTime = 0
                            });
                        }

                        if (data.NextPosition.HasValue)
                        {
                            this.position = data.NextPosition.Value;
                            sequence.m_data.TryGetValue(this.position, out data);
                        }
                        else
                        {
                            this.finished = true;
                            data = null;
                        }
                        eventTicks++;
                    }

                    if (data == null)
                    {
                        this.finished = true;
                        this.position = int.MinValue;
                    }
                    else
                    {
                        this.delayTime += Time.deltaTime;
                    }

                    return !this.finished;
                }
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct pSequenceEventState
            {
                public int eventPosition;
                public float delayTime;

                public bool Tick(Sequence sequence)
                {
                    if (!sequence.m_data.TryGetValue(this.eventPosition, out EventData? data))
                    {
                        return false;
                    }

                    if (this.delayTime < data.EventDelay)
                    {
                        this.delayTime += Time.deltaTime;
                        return true;
                    }

                    if (!EventRegistry.TryGetEntry(data.EventName, out IEventBase? ev))
                    {
                        Log.Warn(nameof(EventManager), "Unknown event with name '" + data.EventName + "'");
                    }
                    else
                    {
                        ev.Activate(data.Data);
                    }
                    return false;
                }
            }

            public Enumerator GetEnumerator()
            {
                return new(this);
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public unsafe Span<byte> GetData()
            {
                int STATE_SIZE = Marshal.SizeOf<pSequenceState>();
                int EVENT_STATE_SIZE = Marshal.SizeOf<pSequenceEventState>();
                byte[] sequenceNameBytes = this.m_sequenceName == null ?
                    Array.Empty<byte>() :
                    Encoding.UTF8.GetBytes(this.m_sequenceName);

                Span<byte> buffer = new byte[
                    sizeof(int) +
                    sequenceNameBytes.Length +
                    STATE_SIZE +
                    sizeof(int) +
                    (EVENT_STATE_SIZE * this.m_events.Count)
                ];


                int nameOffset = sizeof(int) + sequenceNameBytes.Length;

                ReadOnlySpan<byte> nameSizeSpan = new(BitConverter.GetBytes(sequenceNameBytes.Length));
                ReadOnlySpan<byte> sequenceNameSpan = new(sequenceNameBytes);

                // todo: figure out how to remove unneccessary allocation
                IntPtr statePtr = Marshal.AllocHGlobal(STATE_SIZE);
                Marshal.StructureToPtr(this.m_state, statePtr, false);
                ReadOnlySpan<byte> stateSpan = new((void*)statePtr, STATE_SIZE);

                ReadOnlySpan<byte> sizeSpan = new(BitConverter.GetBytes(this.m_events.Count));
                nameSizeSpan.CopyTo(buffer.Slice(0, sizeof(int)));
                sequenceNameSpan.CopyTo(buffer.Slice(sizeof(int), sequenceNameBytes.Length));

                stateSpan.CopyTo(buffer.Slice(nameOffset, STATE_SIZE));
                sizeSpan.CopyTo(buffer.Slice(STATE_SIZE + nameOffset, sizeof(int)));

                Marshal.FreeHGlobal(statePtr);

                for (int index = 0; index < this.m_events.Count; index++)
                {
                    IntPtr eventStatePtr = Marshal.AllocHGlobal(EVENT_STATE_SIZE);
                    Marshal.StructureToPtr(this.m_events[index], eventStatePtr, false);
                    ReadOnlySpan<byte> eventStateSpan = new((void*)eventStatePtr, EVENT_STATE_SIZE);

                    int offset = nameOffset +
                        (STATE_SIZE + sizeof(int)) +
                        (index * EVENT_STATE_SIZE);

                    eventStateSpan.CopyTo(buffer.Slice(offset, EVENT_STATE_SIZE));
                    Marshal.FreeHGlobal(eventStatePtr);
                }

                return buffer;
            }

            public readonly struct Enumerator : IEnumerator
            {
                private readonly Sequence m_sequence;

                public Enumerator(Sequence sequence)
                {
                    this.m_sequence = sequence;
                }

                public bool MoveNext() => this.m_sequence.Tick();

                public object? Current => null;

                public void Reset()
                {

                }
            }

            public static unsafe Sequence? FromData(ReadOnlySpan<byte> data)
            {
                int nameSize = BitConverter.ToInt32(data);
                if (nameSize == 0)
                {
                    return null;
                }

                int offset = sizeof(int);

                string name = Encoding.UTF8.GetString(data.Slice(offset, nameSize));
                offset += nameSize;

                EventSequenceItemData? eventSequence = DataManager.EventSequences.FirstOrDefault((e) => e.Name == name && !e.Disabled);
                if (eventSequence == null)
                {
                    Log.Warn(nameof(EventManager), "No such event sequence with name '" + name + "' exists!");
                    return null;
                }

                int STATE_SIZE = Marshal.SizeOf<pSequenceState>();
                int EVENT_STATE_SIZE = Marshal.SizeOf<pSequenceEventState>();

                Sequence result = new(eventSequence.Events, name);

                fixed (byte* statePtr = &MemoryMarshal.GetReference(data.Slice(offset, STATE_SIZE)))
                {
                    result.m_state = Marshal.PtrToStructure<pSequenceState>((nint)(void*)statePtr);
                }
                offset += STATE_SIZE;

                int count = BitConverter.ToInt32(data.Slice(offset));

                offset += sizeof(int);

                for (int index = 0; index < count; index++)
                {
                    fixed (byte* eventStatePtr = &MemoryMarshal.GetReference(data.Slice(offset, EVENT_STATE_SIZE)))
                    {
                        pSequenceEventState eventState = Marshal.PtrToStructure<pSequenceEventState>((nint)(void*)eventStatePtr);
                        result.m_events.Add(eventState);
                    }
                    offset += EVENT_STATE_SIZE;

                }

                return result;
            }
        }

        private static readonly object s_sequencesLock = new();
        private static readonly List<Sequence> s_sequences = new();

        /// <summary>
        /// Activates an event sequence with the given name.
        /// </summary>
        /// <param name="name">The name of the event sequence</param>
        public static void ActivateEventSequence(string name)
        {
            EventSequenceItemData? data = DataManager.EventSequences.FirstOrDefault((e) => e.Name == name && !e.Disabled);
            if (data == null)
            {
                Log.Warn(nameof(EventManager), "No such event sequence with name '" + name + "' exists!");
                return;
            }

            Log.Verbose(nameof(EventManager), $"Activating Event Sequence '{data.Name}' - {data.DebugName}");

            Sequence sequence = new(data.Events, data.Name);
            sequence.Start();
        }

        /// <summary>
        /// Activates the event sequence, but wont be synced by the checkpoint system.
        /// </summary>
        /// <param name="events">All events</param>
        public static void ActivateEventSequenceUnsynced(IEnumerable<EventData> events)
        {
            Sequence sequence = new(events, null);
            sequence.Start();
        }

        public static void ActivateEvent(string eventName, object? data)
        {
            if (!EventRegistry.TryGetEntry(eventName, out IEventBase? ev))
            {
                Log.Warn(nameof(EventManager), "Unknown event with name '" + eventName + "'");
                return;
            }

            ev.Activate(data);
        }

        internal static Span<byte> GetCheckpointData()
        {
            List<byte> bytes = new();
            lock (s_sequencesLock)
            {
                bytes.AddRange(BitConverter.GetBytes(s_sequences.Count));
                foreach (Sequence sequence in s_sequences)
                {
                    Span<byte> data = sequence.GetData();
                    bytes.AddRange(BitConverter.GetBytes(data.Length));
                    bytes.AddRange(data.ToArray());
                }
            }

            return bytes.ToArray();
        }

        internal static void LoadCheckpointData(ReadOnlySpan<byte> data)
        {
            static bool HasSequence()
            {
                lock (s_sequencesLock)
                {
                    return s_sequences.Count > 0;
                }
            }

            while (HasSequence())
            {
                s_sequences[0].Stop();
            }

            int count = BitConverter.ToInt32(data.Slice(0, sizeof(int)));
            int offset = sizeof(int);
            for (int c = 0; c < count; c++)
            {
                int dataSize = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                Sequence? s = Sequence.FromData(data.Slice(offset, dataSize).ToArray());
                offset += dataSize;

                if (s == null)
                {
                    continue;
                }

                s.Start();
            }
        }
    }
}
