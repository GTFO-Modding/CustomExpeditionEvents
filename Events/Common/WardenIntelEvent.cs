using CustomExpeditionEvents.Utilities;
using System.ComponentModel;

namespace CustomExpeditionEvents.Events.Common
{
    internal sealed class WardenIntelEvent : IEvent<WardenIntelEvent.Data>
    {
        public string Name => "WardenIntel";

        public void Activate(Data data)
        {
            Log.Debug(nameof(WardenIntelEvent), "Activate");

            PUI_WardenIntel intel = GuiManager.PlayerLayer.m_wardenIntel;
            if (data.IsObjectiveText)
            {
                intel.SetWardenObjectiveText(data.Text);
            }
            else
            {
                intel.SetIntelText(data.Text);
            }

            intel.SetVisible(true, data.DisplayDuration);
        }

        public sealed class Data
        {
            [Description("The intel text")]
            public string Text { get; set; } = string.Empty;
            [Description("The duration (in seconds) to display the intel")]
            public float DisplayDuration { get; set; }
            [Description("Whether or not it will be prefixed with the objective MSGCAT header and footer")]
            public bool IsObjectiveText { get; set; }
        }
    }
}
