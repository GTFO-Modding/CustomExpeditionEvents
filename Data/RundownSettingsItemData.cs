using System.Collections.Generic;

namespace CustomExpeditionEvents.Data
{
    public sealed class RundownSettingsItemData : ItemDataBase
    {
        public uint RundownID { get; set; }
        public List<ExpeditionSettingsItemData> Expeditions { get; set; } = new();
    }
}
