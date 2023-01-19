using CustomExpeditionEvents.Utilities.Registry;

namespace CustomExpeditionEvents.Conditions
{
    public sealed class TriggerConditionRegistry : DumpableRegistryWithDataBase<TriggerConditionRegistry, ITriggerConditionBase>
    {
        /// <inheritdoc/>
        protected override string RegistryName => "condition";
    }
}
