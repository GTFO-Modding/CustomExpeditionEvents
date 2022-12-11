using SNetwork;
using System.Collections.Generic;

namespace CustomExpeditionEvents.Events.Common.Managers
{
    public static class SurvivalWaveEventManager
    {
        private static readonly Dictionary<string, List<ushort>> s_waveToMastermindIDs = new();

        public static void Stop(string waveID)
        {
            if (!SNet.IsMaster)
            {
                return;
            }

            if (!s_waveToMastermindIDs.TryGetValue(waveID, out List<ushort>? mastermindIDs))
            {
                return;
            }

            foreach (ushort mastermindID in mastermindIDs)
            {
                if (!Mastermind.Current.TryGetEvent(mastermindID, out Mastermind.MastermindEvent mastermindEvent))
                {
                    continue;
                }
                mastermindEvent.StopEvent();
            }
            mastermindIDs.Clear();
            s_waveToMastermindIDs.Remove(waveID);
        }

        public static void Register(ushort id, string waveID)
        {
            if (!s_waveToMastermindIDs.TryGetValue(waveID, out List<ushort>? ids))
            {
                ids = new List<ushort>();
                s_waveToMastermindIDs.Add(waveID, ids);
            }
            ids.Add(id);
        }
    }
}
