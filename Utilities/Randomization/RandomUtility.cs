using System;
using System.Collections.Generic;

namespace CustomExpeditionEvents.Utilities.Randomization
{
    internal static class RandomUtility
    {
        private static readonly Dictionary<RandomCategory, Randomizer> s_randomGens = new();

        public static Randomizer GenericRandomizer
        {
            get => RandomUtility.GetRandomizer(RandomCategory.Generic);
        }

        public static Randomizer GetRandomizer(RandomCategory category, int? seed = default)
        {
            if (!s_randomGens.TryGetValue(category, out Randomizer? randomizer))
            {
                if (!seed.HasValue)
                {
                    randomizer = new Randomizer();
                }
                else
                {
                    randomizer = new Randomizer(seed.Value);
                }

                s_randomGens.Add(category, randomizer);
            }

            if (seed.HasValue)
            {
                randomizer.UpdateSeed(seed.Value);
            }

            return randomizer;


        }
    }
}
