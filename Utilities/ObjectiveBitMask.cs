namespace CustomExpeditionEvents.Utilities
{
    public enum ObjectiveBitMask : byte
    {
        NONE = 0,

        HIGH = 1 << 0,
        EXTREME = 1 << 1,
        OVERLOAD = 1 << 2,

        MAIN = HIGH,
        SECOND = EXTREME,
        THIRD = OVERLOAD
    }
}
