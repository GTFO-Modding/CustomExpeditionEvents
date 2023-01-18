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

        public static Coroutine Enqueue(IEnumerator enumerator)
            => Enqueue(enumerator.WrapToIl2Cpp());
        public static Coroutine Enqueue(Il2CppSystem.Collections.IEnumerator enumerator)
        {
            return s_instance.StartCoroutine(enumerator);
        }

        public static void Dequeue(Coroutine routine)
            => s_instance.StopCoroutine(routine);

        static CoroutineUtility()
        {
            GameObject obj = new GameObject("CEE Coroutine Utility");
            s_instance = obj.AddComponent<CoroutineUtility>();
            DontDestroyOnLoad(obj);
        }
    }
}
