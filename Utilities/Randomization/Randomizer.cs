using System;

namespace CustomExpeditionEvents.Utilities.Randomization
{
    internal sealed class Randomizer
    {
        private Random random;
        private int seed;
        private ulong draws;

        public Randomizer()
            : this((int)(DateTime.Now.Ticks % int.MaxValue))
        { }

        public Randomizer(int seed)
            : this(new Random(seed), seed)
        { }

        public Randomizer(Random random, int seed)
        {
            this.random = random;
            this.seed = seed;

            ResetData();
        }

        public void UpdateSeed(int newSeed)
        {
            if (this.seed == newSeed)
            {
                return;
            }

            this.ResetData();

            this.seed = newSeed;
            this.random = new Random(seed);
        }

        private void ResetData()
        {
            this.draws = 0;
        }

        public byte[] NextBytes(int count)
        {
            byte[] buffer = new byte[count];
            this.NextBytes(buffer);
            return buffer;
        }

        public void NextBytes(byte[] buffer)
        {
            this.random.NextBytes(buffer);
        }

        public byte NextByte()
        {
            return this.NextBytes(1)[0];
        }

        public sbyte NextSByte()
        {
            return (sbyte)(this.NextByte() - 128);
        }

        public short NextInt16()
        {
            return BitConverter.ToInt16(this.NextBytes(sizeof(short)), 0);
        }

        public ushort NextUInt16()
        {
            return BitConverter.ToUInt16(this.NextBytes(sizeof(ushort)), 0);
        }

        public int NextInt32()
        {
            return BitConverter.ToInt32(this.NextBytes(sizeof(int)), 0);
        }

        public uint NextUInt32()
        {
            return BitConverter.ToUInt32(this.NextBytes(sizeof(uint)), 0);
        }

        public long NextInt64()
        {
            return BitConverter.ToInt64(this.NextBytes(sizeof(long)), 0);
        }

        public ulong NextUInt64()
        {
            return BitConverter.ToUInt64(this.NextBytes(sizeof(ulong)), 0);
        }

        public float NextSingle()
        {
            return this.random.NextSingle();
        }

        public double NextDouble()
        {
            return this.random.NextDouble();
        }

        public float NextRangeSingle(float max)
            => this.NextRangeSingle(0, max);
        public float NextRangeSingle(float min, float max)
        {
            return (float)((this.NextDouble() * (max - min)) + min);
        }

        public double NextRangeDouble(double max)
            => this.NextRangeDouble(0, max);
        public double NextRangeDouble(double min, double max)
        {
            return (this.NextDouble() * (max - min)) + min;
        }

        public byte NextRangeByte(byte max)
            => this.NextRangeByte(0, max);
        public byte NextRangeByte(byte min, byte max)
        {
            return (byte)((this.NextDouble() * (max - min)) + min);
        }

        public sbyte NextRangeSByte(sbyte max)
            => this.NextRangeSByte(0, max);
        public sbyte NextRangeSByte(sbyte min, sbyte max)
        {
            return (sbyte)((this.NextDouble() * (max - min)) + min);
        }

        public short NextRangeInt16(short max)
            => this.NextRangeInt16(0, max);
        public short NextRangeInt16(short min, short max)
        {
            return (short)((this.NextDouble() * (max - min)) + min);
        }

        public ushort NextRangeUInt16(ushort max)
            => this.NextRangeUInt16(0, max);
        public ushort NextRangeUInt16(ushort min, ushort max)
        {
            return (ushort)((this.NextDouble() * (max - min)) + min);
        }

        public int NextRangeInt32(int max)
            => this.NextRangeInt32(0, max);
        public int NextRangeInt32(int min, int max)
        {
            return (int)((this.NextDouble() * (max - min)) + min);
        }

        public uint NextRangeUInt32(uint max)
            => this.NextRangeUInt32(0, max);
        public uint NextRangeUInt32(uint min, uint max)
        {
            return (uint)((this.NextDouble() * (max - min)) + min);
        }

        public long NextRangeInt64(long max)
            => this.NextRangeInt64(0, max);
        public long NextRangeInt64(long min, long max)
        {
            return (long)((this.NextDouble() * (max - min)) + min);
        }

        public ulong NextRangeUInt64(ulong max)
            => this.NextRangeUInt64(0, max);
        public ulong NextRangeUInt64(ulong min, ulong max)
        {
            return (ulong)((this.NextDouble() * (max - min)) + min);
        }
    }
}
