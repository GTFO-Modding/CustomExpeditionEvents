using CustomExpeditionEvents.Utilities;

namespace CustomExpeditionEvents.Events.Common
{
    public sealed class WardenIntelEvent : IEvent<WardenIntelEvent.Data>
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
            public string Text { get; set; } = string.Empty;
            public float DisplayDuration { get; set; }
            public bool IsObjectiveText { get; set; }
        }
    }
}
