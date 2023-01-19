using CustomExpeditionEvents.Events.Common.Managers;
using CustomExpeditionEvents.Utilities;
using System.ComponentModel;

namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class ActivateChainedPuzzleEvent : IEvent<ActivateChainedPuzzleEvent.Data>
    {
        public string Name => "ActivateChainedPuzzle";

        public void Activate(Data data)
        {
            Log.Debug(nameof(ActivateChainedPuzzleEvent), "Activate");

            ChainedPuzzleEventManager.Trigger(data.PuzzleID);
        }

        public sealed class Data
        {
            [Description("The ChainedPuzzle ID specified in ChainedPuzzles.json. Must be included in your rundown settings!")]
            public string PuzzleID { get; set; } = string.Empty;
        }
    }
}
