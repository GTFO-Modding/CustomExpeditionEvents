using CustomExpeditionEvents.Events;
using CustomExpeditionEvents.Events.Common.Managers;
using System;
using System.Collections.Generic;

namespace CustomExpeditionEvents.Networking
{
    /// <summary>
    /// manages all the checkpoint related data
    /// </summary>
    internal static class CheckpointDataManager
    {

        private sealed class CheckpointEvent // : ICheckpointEvent
        {
            public string Name => "CustomExpeditionEventsData";
            public byte[] GetData() => this.GetDataSpan().ToArray();

            public Span<byte> GetDataSpan()
            {
                // for now use lists, but should use spans
                List<byte> data = new();

                // survival waves
                Span<byte> survivalWaveData = ChainedPuzzleEventManager.GetCheckpointData();

                data.AddRange(BitConverter.GetBytes(survivalWaveData.Length));
                data.AddRange(survivalWaveData.ToArray());

                // chained puzzles
                Span<byte> chainedPuzzleData = ChainedPuzzleEventManager.GetCheckpointData();

                data.AddRange(BitConverter.GetBytes(chainedPuzzleData.Length));
                data.AddRange(chainedPuzzleData.ToArray());

                // event data
                Span<byte> eventData = EventManager.GetCheckpointData();

                data.AddRange(BitConverter.GetBytes(eventData.Length));
                data.AddRange(eventData.ToArray());

                return data.ToArray();
            }

            public void OnRecall(byte[] data)
            {
                this.OnRecall(new ReadOnlySpan<byte>(data));
            }

            public void OnRecall(ReadOnlySpan<byte> data)
            {
                int offset = 0;

                // survival waves
                int survivalWaveSize = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                SurvivalWaveEventManager.LoadCheckpointData(data.Slice(offset, survivalWaveSize));


                // chained puzzles
                int chainedPuzzleDataSize = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                ChainedPuzzleEventManager.LoadCheckpointData(data.Slice(offset, chainedPuzzleDataSize));

                // event data
                int eventDataSize = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                EventManager.LoadCheckpointData(data.Slice(offset, eventDataSize));
            }
        }
    }
}
