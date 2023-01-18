using CustomExpeditionEvents.Utilities.Registry;

namespace CustomExpeditionEvents.Conditions
{
    public sealed class TriggerConditionRegistry : RegistryWithDataBase<TriggerConditionRegistry, ITriggerConditionBase>
    {
        /// <inheritdoc/>
        protected override string RegistryName => "condition";
    }
}
