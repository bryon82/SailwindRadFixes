using HarmonyLib;

namespace RadFixes
{
    internal class ContinueMenuFixes
    {
        [HarmonyPatch(typeof(StartMenu), "ButtonClick", typeof(StartMenuButtonType))]
        private class StartMenuPatches
        {
            public static void Prefix(StartMenuButtonType button, ref int __state)
            {
                __state = SaveSlots.activeSlotsCount;
                if (button == StartMenuButtonType.Continue)
                    SaveSlots.activeSlotsCount += 1;
            }

            public static void Postfix(StartMenuButtonType button, int __state)
            {
                if (button == StartMenuButtonType.Continue)
                    SaveSlots.activeSlotsCount = __state;
            }
        }

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
