using HarmonyLib;
using UnityEngine;
using static RadFixes.RF_Plugin;

namespace RadFixes
{
    internal class NREFixes
    {
        [HarmonyPatch(typeof(Refs), "SetPlayerControl")]
        private class RefsPatches
        {
            // startup NRE fix
            public static bool Prefix()
            {
                if (Refs.ovrController == null || Refs.charController == null)
                    return false;

                return true;
            }
        }

        [HarmonyPatch(typeof(NPCBoatController))]
        private class NPCBoatControllerPatches
        {
            // OnTriggerEnter NRE fix. Happens when out at sea
            [HarmonyPatch("OnTriggerEnter")]
            public static bool Prefix(Collider other, NPCBoatController __instance)
            {
                if (!GameState.currentlyLoading && GameState.playing && other.transform == __instance.currentTarget)
                {
                    NPCBoatWaypoint component = other.GetComponent<NPCBoatWaypoint>();
                    if ((bool)component && component.navigationWaypoint)
                    {
                        __instance.currentTarget = component.GetNextDestination().transform;
                        __instance.currentTargetIndex = component.GetNextDestination().index;
                        return false;
                    }

                    int waypointIndex;
                    if (__instance.currentTarget?.GetComponent<NPCBoatWaypoint>() == null)
                        waypointIndex = -1;
                    else
                        waypointIndex = __instance.currentTarget.GetComponent<NPCBoatWaypoint>().index;

                    __instance.currentDock = __instance.currentTarget;
                    __instance.currentDockIndex = waypointIndex;
                    __instance.currentTarget = null;
                    __instance.currentTargetIndex = -1;
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(PortDude), "OnTriggerEnter")]
        private class PortDudePatches
        {
            // OnTriggerEnter NRE fix or index out of bounds fix.
            // Occurs when a good that is not a mission good triggers PortDude's collider.
            public static bool Prefix(Collider other)
            {
                if (other.CompareTag("Player") || !other.CompareTag("Good"))
                    return false;

                var good = other.GetComponent<Good>();
                if (good == null || good.GetMissionIndex() == -1)
                    return false;

                return true;
            }
        }
    }
}
