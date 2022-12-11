using CustomExpeditionEvents.Events.Common.Managers;
using CustomExpeditionEvents.Utilities;
using System.Collections.Generic;

namespace CustomExpeditionEvents.Events.Common
{
    public sealed class ActivateChainedPuzzleEvent : IEvent<ActivateChainedPuzzleEvent.Data>
    {
        public string Name => "ActivateChainedPuzzle";

        public void Activate(Data data)
        {
            Log.Debug(nameof(ActivateChainedPuzzleEvent), "Activate");

            ChainedPuzzleEventManager.Trigger(data.PuzzleID);
        }

        public sealed class Data
        {
            public string PuzzleID { get; set; } = string.Empty;
        }
    }
}
