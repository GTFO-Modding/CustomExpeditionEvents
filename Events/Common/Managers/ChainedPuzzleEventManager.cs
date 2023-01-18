using ChainedPuzzles;
using CustomExpeditionEvents.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CustomExpeditionEvents.Events.Common.Managers
{
    /// <summary>
    /// Manages storing the state of all chained puzzles defined in the
    /// datablock for the level. Is used in <see cref="ActivateChainedPuzzleEvent"/>
    /// where a specific chain puzzle name is passed.
    /// </summary>
    public static class ChainedPuzzleEventManager
    {
        private static readonly Dictionary<string, Puzzle> s_puzzles = new();

        public static void Trigger(string name)
        {
            if (!ChainedPuzzleEventManager.s_puzzles.TryGetValue(name, out Puzzle? puzzle))
            {
                Log.Warn(nameof(ChainedPuzzleEventManager), $"Attempted to activate chained puzzle with id '{name}' when it doesn't exist");
            }
        }

        internal static void RegisterInstance(string name, ChainedPuzzleInstance chainedPuzzle)
        {
            s_puzzles[name] = new Puzzle(chainedPuzzle);
        }

        private sealed class Puzzle
        {
            private readonly ChainedPuzzleInstance m_instance;
            private pPuzzleState m_state;

            public Puzzle(ChainedPuzzleInstance instance)
            {
                this.m_instance = instance;
            }

            public void Trigger()
            {

            }

            public unsafe Span<byte> GetData()
            {
                int STATE_SIZE = Marshal.SizeOf<pPuzzleState>();
                byte[] puzzleUIDBytes = Encoding.UTF8.GetBytes(this.m_instance.m_puzzleUID);

                Span<byte> byteData = new byte[
                    sizeof(int) + // puzzleUID length
                    puzzleUIDBytes.Length + // puzzleUID
                    STATE_SIZE
                ];


                ReadOnlySpan<byte> puzzleUIDSizeSpan = new(BitConverter.GetBytes(puzzleUIDBytes.Length));
                ReadOnlySpan<byte> puzzleUIDSpan = new(puzzleUIDBytes);

                int nameOffset = puzzleUIDBytes.Length + sizeof(int);

                // todo: figure out how to remove unneccessary allocation
                IntPtr statePtr = Marshal.AllocHGlobal(STATE_SIZE);
                Marshal.StructureToPtr(this.m_state, statePtr, false);
                ReadOnlySpan<byte> stateSpan = new((void*)statePtr, STATE_SIZE);

                puzzleUIDSizeSpan.CopyTo(byteData.Slice(0, sizeof(int)));
                puzzleUIDSpan.CopyTo(byteData.Slice(sizeof(int), puzzleUIDBytes.Length));

                stateSpan.CopyTo(byteData.Slice(nameOffset, STATE_SIZE));

                return byteData;
            }

            public static unsafe Puzzle? FromData(ReadOnlySpan<byte> data)
            {
                int offset = 0;

                // puzzleUID
                int puzzleUIDLength = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                string puzzleUID = Encoding.UTF8.GetString(data.Slice(offset, puzzleUIDLength));
                offset += puzzleUIDLength;

                ChainedPuzzleInstance? instance = null;

                for (int index = 0, count = ChainedPuzzleManager.Current.m_instances.Count; index < count; index++)
                {
                    ChainedPuzzleInstance potentialInstance = ChainedPuzzleManager.Current.m_instances[index];

                    if (potentialInstance.m_puzzleUID == puzzleUID)
                    {
                        instance = potentialInstance;
                        break;
                    }
                }

                if (instance == null)
                {
                    return null;
                }

                Puzzle puzzle = new(instance);
                int STATE_SIZE = Marshal.SizeOf<pPuzzleState>();

                fixed (byte* statePtr = &MemoryMarshal.GetReference(data.Slice(offset, STATE_SIZE)))
                {
                    puzzle.m_state = Marshal.PtrToStructure<pPuzzleState>((nint)(void*)statePtr);
                }

                return puzzle;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct pPuzzleState
            {

            }
        }

        internal static Span<byte> GetCheckpointData()
        {
            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes(s_puzzles.Count));
            foreach (KeyValuePair<string, Puzzle> puzzlePair in s_puzzles)
            {
                Puzzle puzzle = puzzlePair.Value;
                string puzzleID = puzzlePair.Key;

                byte[] nameBytes = Encoding.UTF8.GetBytes(puzzleID);

                bytes.AddRange(BitConverter.GetBytes(nameBytes.Length));
                bytes.AddRange(nameBytes);

                Span<byte> data = puzzle.GetData();
                bytes.AddRange(BitConverter.GetBytes(data.Length));
                bytes.AddRange(data.ToArray());
            }

            return bytes.ToArray();
        }

        internal static void LoadCheckpointData(ReadOnlySpan<byte> data)
        {
            s_puzzles.Clear();

            int offset = 0;

            int count = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
            offset += sizeof(int);

            while (count > 0)
            {
                int nameCount = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                string puzzleName = Encoding.UTF8.GetString(data.Slice(offset, nameCount));
                offset += nameCount;

                int dataLength = BitConverter.ToInt32(data.Slice(offset, sizeof(int)));
                offset += sizeof(int);

                Puzzle? puzzle = Puzzle.FromData(data.Slice(offset, dataLength));
                if (puzzle == null)
                {
                    Log.Warn(nameof(ChainedPuzzleEventManager), $"Failed to load puzzle '{puzzle}' from checkpoint");
                }
                else
                {
                    s_puzzles[puzzleName] = puzzle;
                }

                count--;
            }
        }
    }
}
