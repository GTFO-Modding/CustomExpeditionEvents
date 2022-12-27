using System;

namespace CustomExpeditionEvents.Utilities
{
    internal static class RegistryLockManager
    {
        private static bool s_locked = false;

        internal static void Lock()
        {
            RegistryLockManager.s_locked = true;
        }

        public static bool IsLocked => RegistryLockManager.s_locked;

        /// <summary>
        /// Ensures that the registry lock manager is unlocked, throwing an exception
        /// if it was.
        /// </summary>
        /// <param name="registryName">The name of the registry.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void EnsureUnlocked(string registryName)
        {
            if (RegistryLockManager.s_locked)
            {
                throw new InvalidOperationException($"The {registryName} registry is locked, and thus doesn't allow new registrations. Try registering your {registryName} before GS_NoLobby.Enter.");
            }
        }
    }
}
