using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomExpeditionEvents.Components
{
    internal sealed class CustomChainedPuzzleDataComponent : MonoBehaviour
    {
        public CustomChainedPuzzleDataComponent(nint ptr) : base(ptr)
        { }
        public Il2CppStringField m_puzzleName;

        [HideFromIl2Cpp]
        public string PuzzleName
        {
            get => this.m_puzzleName.Value;
            set => this.m_puzzleName.Value = value;
        }


        static CustomChainedPuzzleDataComponent()
        {
            ClassInjector.RegisterTypeInIl2Cpp<CustomChainedPuzzleDataComponent>();
        }
    }
}
