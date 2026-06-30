using HarmonyLib;
using static RadFixes.RF_Plugin;

namespace RadFixes.Fixes
{
    internal class OtherModFixes
    {
        public static void PatchCargoControllerMod()
        {
            var cargoControllerUIType = AccessTools.TypeByName("CargoControllerUI");
            var originalMethod = AccessTools.Method(cargoControllerUIType, "ApplyGuiStyles");
            var patchMethod = AccessTools.Method(typeof(CargoControllerPatches), "DoNotRun");
            HarmonyInstance.Patch(originalMethod, new HarmonyMethod(patchMethod));
        }
    }

    public class CargoControllerPatches
    {
        [HarmonyPrefix]
        public static bool DoNotRun()
        {
            return false;
        }
    }
}
