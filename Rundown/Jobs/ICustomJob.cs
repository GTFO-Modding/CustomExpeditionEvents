namespace CustomExpeditionEvents.Rundown.Jobs
{
#if !IL2CPP_INHERITANCE
    internal interface ICustomJob
    {
        bool Build()
        {
            return false;
        }

        string GetName()
        {
            return this.GetType().Name;
        }

        bool TakeFullFrame()
        {
            return false;
        }
    }
#endif
}
