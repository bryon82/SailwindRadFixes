using HarmonyLib;
using UnityEngine;
using static RadFixes.RF_Plugin;

namespace RadFixes
{
    internal class HangableFixes
    {
        [HarmonyPatch(typeof(HangableItem), "DisconnectJoint")]
        private class HangableItemPatches
        {
            // shipItem.ToggleDisallowDisembarking(true); -> shipItem.ToggleDisallowDisembarking(false);
            [HarmonyPatch("DisconnectJoint")]
            public static bool Prefix(ItemRigidbody ___itemRigidbodyC, ref Collider ___currentHook, ShipItem ___shipItem)
            {
                if (___currentHook == null)
                {
                    return false;
                }
                ___currentHook.GetComponent<ShipItemLampHook>().RemoveJoint();
                ___currentHook = null;
                ___itemRigidbodyC.attached = false;
                ___itemRigidbodyC.disableCol = false;
                ___itemRigidbodyC.inStove = false;
                ___shipItem.ToggleDisallowDisembarking(false);
                LogInfo("Disconnected joint.");
                return false;
            }

            // Fixes items hanging to close hooks when in a shop
            [HarmonyPatch("OnTriggerEnter")]
            public static bool Prefix(ShipItem ___shipItem)
            {
                if (!___shipItem.sold)
                    return false;

                return true;
            }
        }
    }
}
