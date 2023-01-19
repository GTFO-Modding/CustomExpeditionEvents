using CustomExpeditionEvents.Utilities;
using System.ComponentModel;

namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class FogTransitionEvent : IEvent<FogTransitionEvent.Data>
    {
        public string Name => "FogTransition";
        
        public void Activate(Data data)
        {
            Log.Debug(nameof(FogTransitionEvent), "Activate");

            EnvironmentStateManager.AttemptStartFogTransition(data.FogID, data.TransitionDuration, data.Dimension);
        }

        public sealed class Data
        {
            [Description("The Fog Settings DataBlock ID")]
            public uint FogID { get; set; }
            [Description("The duration (in seconds) for transitioning between the current fog settings and the new settings")]
            public float TransitionDuration { get; set; }
            [Description("The dimension the fog settings are for")]
            public eDimensionIndex Dimension { get; set; }
        }
    }
}
