using CustomExpeditionEvents.Events.Common.Managers;
using CustomExpeditionEvents.Utilities;

namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class StopCustomSurvivalWaveEvent : IEvent<StopCustomSurvivalWaveEvent.Data>
    {
        public string Name => "StopCustomSurvivalWave";

        public void Activate(Data data)
        {
            Log.Debug(nameof(StopCustomSurvivalWaveEvent), "Activate");
            SurvivalWaveEventManager.Stop(data.WaveID);
        }

        public sealed class Data
        {
            public string WaveID { get; set; } = string.Empty;
        }
    }
}
