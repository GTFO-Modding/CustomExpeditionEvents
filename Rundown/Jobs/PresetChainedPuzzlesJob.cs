using ChainedPuzzles;
using CustomExpeditionEvents.Data;
using CustomExpeditionEvents.Utilities;
using Globals;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            uint rundownID = Global.RundownIdToLoad;
            RundownSettingsItemData? data = DataManager.RundownSettings.FirstOrDefault((settings) => settings.RundownID == rundownID);

            if (data == null)
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

            IEnumerable<ChainedPuzzleItemData> chainedPuzzlesToBuild = DataManager.ChainedPuzzles.Where((puzzle) => !puzzle.Disabled &&  expeditionData.Settings.RequiredChainedPuzzles.Contains(puzzle.Name));

            foreach (ChainedPuzzleItemData puzzleData in chainedPuzzlesToBuild)
            {
                // todo: use ChainedPuzzleManager.CreatePuzzleInstance
                //       to create chained puzzles.
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
