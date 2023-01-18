using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditionEvents.Events.Common.Managers
{
    public static class DataStoreManager
    {
        private static readonly ConcurrentDictionary<string, bool> s_dataBoolean = new();
        private static readonly ConcurrentDictionary<string, int> s_dataInt32 = new();
        private static readonly ConcurrentDictionary<string, long> s_dataInt64 = new();
        private static readonly ConcurrentDictionary<string, float> s_dataSingle = new();
        private static readonly ConcurrentDictionary<string, double> s_dataDouble = new();

        public static void SetBoolean(string name, bool value)
        {
            DataStoreManager.s_dataBoolean[name] = value;
        }
        public static void SetInt32(string name, int value)
        {
            DataStoreManager.s_dataInt32[name] = value;
        }
        public static void SetInt64(string name, long value)
        {
            DataStoreManager.s_dataInt64[name] = value;
        }
        public static void SetSingle(string name, float value)
        {
            DataStoreManager.s_dataSingle[name] = value;
        }
        public static void SetDouble(string name, double value)
        {
            DataStoreManager.s_dataDouble[name] = value;
        }

        public static bool GetBoolean(string name)
        {
            return DataStoreManager.s_dataBoolean.TryGetValue(name, out bool value) ? value : default;
        }
        public static int GetInt32(string name)
        {
            return DataStoreManager.s_dataInt32.TryGetValue(name, out int value) ? value : default;
        }
        public static long GetInt64(string name)
        {
            return DataStoreManager.s_dataInt64.TryGetValue(name, out long value) ? value : default;
        }
        public static float GetSingle(string name)
        {
            return DataStoreManager.s_dataSingle.TryGetValue(name, out float value) ? value : default;
        }
        public static double GetDouble(string name)
        {
            return DataStoreManager.s_dataDouble.TryGetValue(name, out double value) ? value : default;
        }
    }
}
