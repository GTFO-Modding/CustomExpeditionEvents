using CustomExpeditionEvents.Utilities;

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
            public uint FogID { get; set; }
            public float TransitionDuration { get; set; }
            public eDimensionIndex Dimension { get; set; }
        }
    }
}
