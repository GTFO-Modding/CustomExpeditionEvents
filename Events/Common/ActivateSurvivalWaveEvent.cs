using System;
using System.Collections;

namespace CustomExpeditionEvents.Events.Common
{
    public sealed class ActivateSurvivalWaveEvent : IEvent
    {
        public string Name => "ActivateSurvivalWave";

        public void Activate(IEventDataObject data)
        {
        }
    }
}
