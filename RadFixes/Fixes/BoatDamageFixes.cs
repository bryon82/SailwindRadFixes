using HarmonyLib;
using System.Collections;
using UnityEngine;
using static RadFixes.RF_Plugin;

namespace RadFixes
{
    internal class BoatDamageFixes
    {
        internal static bool inIFrames = false;
        
        [HarmonyPatch(typeof(BoatDamage), "Impact")]
        public static class BoatDamagePatches
        {
            public static bool Prefix()
            {
                if (inIFrames)
                    LogDebug("In IFrames, no damage received");

                return !inIFrames;
            }
        }

        [HarmonyPatch(typeof(Shipyard), "DischargeShip")]
        public static class ShipyardPatches
        {
            public static void Postfix(Shipyard __instance)
            {
                __instance.StartCoroutine(IFrameCoroutine());
            }
        }

        private static IEnumerator IFrameCoroutine()
        {
            inIFrames = true;
            yield return new WaitForSeconds(5f);
            inIFrames = false;
        }
    }
}
