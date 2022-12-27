using System.Collections.Generic;

namespace CustomExpeditionEvents.Data
{
    public sealed class ExpeditionSettings
    {
        public List<string> RequiredChainedPuzzles { get; set; } = new();
    }
}