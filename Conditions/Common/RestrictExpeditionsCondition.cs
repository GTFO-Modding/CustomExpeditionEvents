using Globals;
using System.Collections.Generic;

namespace CustomExpeditionEvents.Conditions.Common
{
    internal sealed class RestrictExpeditionsCondition : ITriggerCondition<RestrictExpeditionsCondition.Data>
    {
        public string Name => "RestrictExpeditions";

        public bool IsValid(Data data)
        {
            pActiveExpedition rundownData = RundownManager.GetActiveExpeditionData();

            return data.IsValid(Global.RundownIdToLoad, rundownData.tier, rundownData.expeditionIndex);
        }

        public sealed class Data
        {
            public bool IsWhitelist { get; set; }
            public List<RundownRestriction> Rundowns { get; set; } = new();

            public bool IsValid(uint rundownID, eRundownTier tier, int expeditionIndex)
            {
                foreach (RundownRestriction rundownRestriction in this.Rundowns)
                {
                    if (rundownRestriction.RundownID != rundownID)
                    {
                        continue;
                    }

                    foreach (ExpeditionRestriction expeditionRestriction in rundownRestriction.Expeditions)
                    {
                        if (expeditionRestriction.ExpeditionIndex != expeditionIndex ||
                            expeditionRestriction.Tier != tier)
                        {
                            continue;
                        }

                        return this.IsWhitelist;
                    }
                }

                return !this.IsWhitelist;
            }
        }

        public sealed class RundownRestriction
        {
            public List<ExpeditionRestriction> Expeditions { get; set; } = new();
            public uint RundownID { get; set; }
        }

        public sealed class ExpeditionRestriction
        {
            public eRundownTier Tier { get; set; }
            public int ExpeditionIndex { get; set; }
        }
    }
}
