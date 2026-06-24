using Crest;
using HarmonyLib;
using UnityEngine;
using static RadFixes.Configs;

namespace RadFixes
{
    internal class ItemFixes
    {
        #region fishing rod

        [HarmonyPatch(typeof(ShipItemFishingRod))]
        private class ShipItemFishingRodPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnLoad")]
            public static void OnLoadSetReverseSpinner(ref float ___spinnerSpeed)
            {
                if (enableFishingReelFix.Value)
                    ___spinnerSpeed = -40f;
            }

            [HarmonyPostfix]
            [HarmonyPatch("OnBuy")]
            public static void OnBuySetReverseSpinner(ref float ___spinnerSpeed)
            {
                if (enableFishingReelFix.Value)
                    ___spinnerSpeed = -40f;
            }
        }

        #endregion

        #region spyglass

        [HarmonyPatch(typeof(GoPointer))]
        private static class GoPointerPatches
        {
            // disable interaction highlights when looking through spyglass
            [HarmonyPatch("DoRaycast")]
            public static bool Prefix(GoPointer __instance)
            {
                if (GameState.playing && !GameState.currentlyLoading)
                {
                    var heldItem = __instance.GetHeldItem();
                    if (heldItem != null &&
                        heldItem.GetComponent<ShipItem>()?.name == "spyglass" &&
                        heldItem.GetComponent<ShipItemSpyglass>().heldRotationOffset != -22f)
                    {
                        __instance.lookUI.gameObject.SetActive(false);
                        return false;
                    }
                    __instance.lookUI.gameObject.SetActive(true);
                }
                return true;
            }

            // disable placement when looking through spyglass
            [HarmonyPatch("LateUpdate")]
            public static bool Prefix(GoPointer __instance, PickupableItem ___heldItem, GoPointerButton ___pointedAtButton)
            {
                if (GameState.playing &&
                    !GameState.currentlyLoading &&
                    ___heldItem != null &&
                    ___heldItem.GetComponent<ShipItem>()?.name == "spyglass" &&
                    (bool)___pointedAtButton &&
                    ___pointedAtButton.allowPlacingItems && __instance.AltButtonDown())
                {
                    return false;
                }
                return true;
            }
        }

        #endregion

        [HarmonyPatch(typeof(ShipItemChipLog), "OnBuy")]
        private class ShipItemChipLogPatches
        {
            // chiplog was not being properly initialized when bought
            public static bool Prefix(
                ShipItemChipLog __instance,
                ref Rigidbody ___bobberBody,
                ConfigurableJoint ___bobberJoint,
                ref Vector3 ___initialBobberPos,
                ref SimpleFloatingObject ___bobberFloater)
            {
                ___bobberBody = ___bobberJoint.GetComponent<Rigidbody>();
                ___bobberJoint.transform.parent = Refs.shiftingWorld;
                ___initialBobberPos = ___bobberJoint.connectedAnchor;
                ___bobberFloater = ___bobberBody.GetComponent<SimpleFloatingObject>();
                _ = __instance.sold;

                return false;
            }
        }
    }
}
