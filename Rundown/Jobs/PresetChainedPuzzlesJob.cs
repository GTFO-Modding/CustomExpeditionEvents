using ChainedPuzzles;
using CustomExpeditionEvents.Components;
using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Events.Common.Managers;
using CustomExpeditionEvents.Utilities;
using Globals;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomExpeditionEvents.Rundown.Jobs
{
    internal sealed class PresetChainedPuzzlesJob :
#if IL2CPP_INHERITANCE
        LG_FactoryJob
#else
        ICustomJob
#endif
    {
#if IL2CPP_INHERITANCE
        public PresetChainedPuzzlesJob(nint ptr) : base(ptr)
        { }

        public PresetChainedPuzzlesJob()
            : this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<PresetChainedPuzzlesJob>.NativeClassPtr))
        #else
        public PresetChainedPuzzlesJob()
#endif
        {
        }

#if IL2CPP_INHERITANCE
        new 
#endif

        public bool Build()
        {
            // this should be checked for consistency
            uint rundownID = Global.RundownIdToLoad;

            if (!DataManager.RundownSettings.TryGetEntry(rundownID, out RundownSettingsItemData? data))
            {
                Log.Verbose(nameof(PresetChainedPuzzlesJob), "No rundown information specified for rundown id '" + rundownID + "'");
                return true;
            }

            pActiveExpedition expedition = RundownManager.GetActiveExpeditionData();

            ExpeditionSettingsItemData? expeditionData = data.Expeditions.FirstOrDefault((e) => !e.Disabled && e.Tier == expedition.tier && e.ExpeditionIndex == expedition.expeditionIndex);

            if (expeditionData == null)
            {
                Log.Verbose(nameof(PresetChainedPuzzlesJob), "No expedition information specified for rundown id '" + rundownID + "' for tier '" + expedition.tier + "', index '" + expedition.expeditionIndex + "'");
                return true;
            }

            IEnumerable<ChainedPuzzleItemData> chainedPuzzlesToBuild = expeditionData.Settings.RequiredChainedPuzzles.Select((puzzleName) => DataManager.ChainedPuzzles.GetEntry(puzzleName));

            foreach (ChainedPuzzleItemData puzzleData in chainedPuzzlesToBuild)
            {
                if (!ExpeditionUtilities.TryGetZone(puzzleData.SpawnData.DimensionIndex, puzzleData.SpawnData.LayerType, puzzleData.SpawnData.ZoneIndex, out LG_Zone? zone))
                {
                    Log.Warn(nameof(PresetChainedPuzzlesJob), $"Failed to build puzzle '{puzzleData.Name}' ({puzzleData.DebugName}): Failed to fetch zone!");
                    continue;
                }

                int areaIndex;
                if (!puzzleData.SpawnData.AreaIndex.HasValue)
                {
                    areaIndex = Builder.SessionSeedRandom.Range(0, zone.m_areas.Count);
                }
                else
                {
                    areaIndex = puzzleData.SpawnData.AreaIndex.Value;
                }

                areaIndex = Math.Max(Math.Min(areaIndex, zone.m_areas.Count - 1), 0);

                LG_Area area = zone.m_areas[areaIndex];

                string puzzleName = puzzleData.Name;

                Vector3 sourcePos = area.m_courseNode.GetRandomPositionInside_SessionSeed();

                ChainedPuzzleInstance puzzleInstance = ChainedPuzzleManager.CreatePuzzleInstance(puzzleData.ChainedPuzzleID, area, sourcePos, area.transform);
                CustomChainedPuzzleDataComponent componentData = puzzleInstance.gameObject.AddComponent<CustomChainedPuzzleDataComponent>();

                componentData.PuzzleName = puzzleName;

                ChainedPuzzleEventManager.RegisterInstance(puzzleName, puzzleInstance);

                Log.Debug($"Built puzzle '{puzzleData.Name}' ({puzzleData.DebugName})");
            }

            return true;
        }


#if IL2CPP_INHERITANCE
        static PresetChainedPuzzlesJob()
        {
            ClassInjector.RegisterTypeInIl2Cpp<PresetChainedPuzzlesJob>();
        }
#endif
    }
}
