using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomExpeditionEvents.Events.Common.Managers
{
    /// <summary>
    /// Manages storing the state of all chained puzzles defined in the
    /// datablock for the level. Is used in <see cref="ActivateChainedPuzzleEvent"/>
    /// where a specific chain puzzle name is passed.
    /// </summary>
    public static class ChainedPuzzleEventManager
    {

        public static void Trigger(string name)
        {
            //
        }
    }
}
