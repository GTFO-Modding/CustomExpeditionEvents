using CustomExpeditionEvents.Converters;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using UnityEngine;

namespace CustomExpeditionEvents.Utilities
{
    [JsonConverter(typeof(SoundEventJsonConverter))]
    public readonly struct SoundEvent
    {
        private readonly bool m_isName;
        private readonly string? m_name;
        private readonly uint m_id;

        [MemberNotNullWhen(true, nameof(SoundEvent.Name))]
        public bool IsName => this.m_isName;
        [MemberNotNullWhen(false, nameof(SoundEvent.Name))]
        public bool IsID => !this.IsName;


        public string? Name => this.m_name;
        public uint Id => this.m_id;

        public SoundEvent(uint id)
        {
            this.m_id = id;
            this.m_isName = false;
            this.m_name = null;
        }

        public SoundEvent(string? name)
        {
            this.m_id = 0U;
            this.m_isName = name != null;
            this.m_name = name;
        }

        public void Post()
        {
            if (this.IsID)
            {
                CellSound.Post(this.Id);
            }
            else
            {
                CellSound.Post(this.Name);
            }
        }

        public void Post(Vector3 position)
        {
            if (this.IsID)
            {
                CellSound.Post(this.Id, position);
            }
            else
            {
                CellSound.Post(this.Name, position);
            }
        }

        public void Post(CellSoundPlayer player)
        {
            if (this.IsID)
            {
                player.Post(this.Id);
            }
            else
            {
                player.Post(this.Name);
            }
        }

        public void PostGlobal(CellSoundPlayer player)
        {
            if (this.IsID)
            {
                player.Post(this.Id, isGlobal: true);
            }
            else
            {
                player.Post(this.Name, isGlobal: true);
            }
        }
    }
}
