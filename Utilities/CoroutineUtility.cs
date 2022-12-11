using BepInEx.Unity.IL2CPP.Utils.Collections;
using System.Collections;
using UnityEngine;

namespace CustomExpeditionEvents.Utilities
{
    internal sealed class CoroutineUtility : MonoBehaviour
    {
        private static readonly CoroutineUtility s_instance;

        public CoroutineUtility(nint ptr) : base(ptr)
        { }

        public static void Enqueue(IEnumerator enumerator)
            => Enqueue(enumerator.WrapToIl2Cpp());
        public static void Enqueue(Il2CppSystem.Collections.IEnumerator enumerator)
        {
            s_instance.StartCoroutine(enumerator);
        }

        static CoroutineUtility()
        {
            GameObject obj = new GameObject("CEE Coroutine Utility");
            s_instance = obj.AddComponent<CoroutineUtility>();
            DontDestroyOnLoad(obj);
        }
    }
}
