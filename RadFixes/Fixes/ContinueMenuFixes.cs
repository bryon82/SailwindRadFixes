using HarmonyLib;

namespace RadFixes
{
    internal class ContinueMenuFixes
    {
        // fixes backup saves for save slot one not popping up until you hover over another save slot.
        [HarmonyPatch(typeof(BackupSavesListUI), "Awake")]
        private class BackupSavesListUIPatches
        {
            public static void Postfix(ref int ___showingListFor)
            {
                ___showingListFor = -1;
            }
        }
    }
}