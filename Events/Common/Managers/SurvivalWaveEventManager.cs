using System;
using SNetwork;
using System.Collections.Generic;
using System.Text;

namespace CustomExpeditionEvents.Events.Common.Managers
{
    public static class SurvivalWaveEventManager
    {
        private static readonly Dictionary<string, SurvivalWave> s_waves = new();

        public static void Stop(string waveID)
        {
            if (!SNet.IsMaster)
            {
                return;
            }

            if (!s_waves.TryGetValue(waveID, out SurvivalWave? wave))
            {
                return;
            }

            wave.Stop();
            s_waves.Remove(waveID);
        }

        public static void Register(ushort id, string waveID)
        {
            if (!s_waves.TryGetValue(waveID, out SurvivalWave? wave))
            {
                wave = new SurvivalWave();
                s_waves.Add(waveID, wave);
            }
            wave.AddID(id);
        }

        private sealed class SurvivalWave
        {
            private readonly HashSet<ushort> m_mastermindIDS;

            public SurvivalWave()
            {
                this.m_mastermindIDS = new();
            }

            public void AddID(ushort id)
            {
                this.m_mastermindIDS.Add(id);
            }

            public void Stop()
            {
                foreach (ushort mastermindID in this.m_mastermindIDS)
                {
                    if (!Mastermind.Current.TryGetEvent(mastermindID, out Mastermind.MastermindEvent mastermindEvent))
                    {
                        continue;
                    }
                    mastermindEvent.StopEvent();
                }
                this.m_mastermindIDS.Clear();
            }

            public Span<byte> GetData()
            {
                Span<byte> data = new byte[
                    sizeof(int) +
                    (this.m_mastermindIDS.Count * sizeof(ushort))
                ];

                ReadOnlySpan<byte> countSpan = BitConverter.GetBytes(this.m_mastermindIDS.Count);
                countSpan.CopyTo(data.Slice(0, sizeof(int)));

                int offset = sizeof(int);

                foreach (ushort id in this.m_mastermindIDS)
                {
                    ReadOnlySpan<byte> mastermindIDSpan = BitConverter.GetBytes(id);
                    mastermindIDSpan.CopyTo(data.Slice(offset, sizeof(ushort)));
                    offset += sizeof(ushort);
                }

                return data;
            }

            public static SurvivalWave FromData(ReadOnlySpan<byte> data)
            {
                int offset = 0;
                int count = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                SurvivalWave wave = new();

                while (count > 0)
                {
                    ushort mastermindID = BitConverter.ToUInt16(data.Slice(offset, sizeof(ushort)));
                    offset += sizeof(ushort);
                    wave.AddID(mastermindID);
                    count--;
                }

                return wave;
            }
        }

        internal static Span<byte> GetCheckpointData()
        {
            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes(s_waves.Count));
            foreach (KeyValuePair<string, SurvivalWave> wavePair in s_waves)
            {
                SurvivalWave wave = wavePair.Value;
                string waveID = wavePair.Key;

                byte[] waveIDBytes = Encoding.UTF8.GetBytes(waveID);

                bytes.AddRange(BitConverter.GetBytes(waveIDBytes.Length));
                bytes.AddRange(waveIDBytes);

                Span<byte> data = wave.GetData();
                bytes.AddRange(BitConverter.GetBytes(data.Length));
                bytes.AddRange(data.ToArray());
            }

            return bytes.ToArray();
        }

        internal static void LoadCheckpointData(ReadOnlySpan<byte> data)
        {
            s_waves.Clear();

            int offset = 0;
            int count = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
            offset += sizeof(int);

            while (count > 0)
            {
                int waveIDCount = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                string waveID = Encoding.UTF8.GetString(data.Slice(offset, waveIDCount));
                offset += waveIDCount;

                int dataLength = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                SurvivalWave wave = SurvivalWave.FromData(data.Slice(offset, dataLength));
                s_waves[waveID] = wave;

                count--;
            }
        }
    }
}
