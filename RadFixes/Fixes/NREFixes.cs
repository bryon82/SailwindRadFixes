using HarmonyLib;
using UnityEngine;

namespace RadFixes
{
    internal class NREFixes
    {
        [HarmonyPatch(typeof(MouseLook), "ToggleMouseLookAndCursor")]
        private class MouseLookPatches
        {
            // startup NRE fix
            public static bool Prefix()
            {
                if (MouseButtonPointer.instance == null)
                    return false;

                return true;
            }
        }

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
            // startup NRE fix
            [HarmonyPatch("Start")]
            public static bool Prefix(NPCBoatController __instance, ref Rigidbody ___rigidbody)
            {
                int waypointIndex;
                if (__instance.currentTarget?.GetComponent<NPCBoatWaypoint>() == null)
                    waypointIndex = -1;
                else
                    waypointIndex = __instance.currentTarget.GetComponent<NPCBoatWaypoint>().index;

                ___rigidbody = __instance.GetComponent<Rigidbody>();
                __instance.currentTargetIndex = waypointIndex;
                __instance.currentDockIndex = -1;

                return false;
            }

            // OnTriggerEnter NRE fix
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

        [HarmonyPatch(typeof(Sail), "Start")]
        private class SailPatches
        {
            // startup NRE fix
            public static bool Prefix(Sail __instance, ref Rigidbody ___sailRigidbody, Rigidbody ___shipRigidbody, ref BoatDamage ___damage, ref float ___minAngle, ref float ___maxAngle, SailCategory ___category)
            {
                ___sailRigidbody = __instance.GetComponent<Rigidbody>();
                if (!___sailRigidbody)
                {
                    Debug.LogError($"{__instance.gameObject.name}: no sailRigidbody.");
                }

                ___damage = ___shipRigidbody?.gameObject.GetComponent<BoatDamage>();

                var hingeJoint = __instance.GetComponent<HingeJoint>();
                if (___minAngle == 0f && hingeJoint != null)
                {
                    ___minAngle = hingeJoint.limits.min;
                }

                if (___maxAngle == 0f && hingeJoint != null)
                {
                    ___maxAngle = hingeJoint.limits.max;
                }

                if (___category == SailCategory.staysail)
                {
                    ___sailRigidbody.mass = 0.1f;
                    ___sailRigidbody.angularDrag = 1f;
                }

                __instance.SetSailArea();
                return false;
            }
        }

        [HarmonyPatch(typeof(PortDude), "OnTriggerEnter")]
        private class PortDudePatches
        {
            // OnTriggerEnter NRE fix or index out of bounds fix
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
