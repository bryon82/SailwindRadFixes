using HarmonyLib;
using UnityEngine;
using System.Linq;
using Crest;

namespace RadFixes
{
    internal class Patches
    {
        [HarmonyPatch(typeof(ShipItemFishingRod))]
        private class ShipItemFishingRodPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnLoad")]
            public static void OnLoadSetReverseSpinner(ref float ___spinnerSpeed)
            {
                if (Plugin.enableFishingReelFix.Value)
                    ___spinnerSpeed = -40f;
            }

            [HarmonyPostfix]
            [HarmonyPatch("OnBuy")]
            public static void OnBuySetReverseSpinner(ref float ___spinnerSpeed)
            {
                if (Plugin.enableFishingReelFix.Value)
                    ___spinnerSpeed = -40f;
            }
        }

        [HarmonyPatch(typeof(StartMenu))]
        private class StartMenuPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Awake")]
            public static void DestroyDeco(GameObject ___settingsUI)
            {
                var deco = ___settingsUI.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == "deco (1)");
                Object.Destroy(deco.gameObject);
            }

            [HarmonyPrefix]
            [HarmonyPatch("GameToSettings")]
            public static void PauseSound()
            {
                AudioListener.pause = true;
            }

            [HarmonyPrefix]
            [HarmonyPatch("SettingsToGame")]
            public static void UnPauseSound()
            {
                AudioListener.pause = false;
            }

            [HarmonyPrefix]
            [HarmonyPatch("ButtonClick", typeof(StartMenuButtonType))]
            public static void ContinueLoadSlotMenuPre(StartMenuButtonType button, ref int __state)
            {
                __state = SaveSlots.activeSlotsCount;
                if (button == StartMenuButtonType.Continue)
                    SaveSlots.activeSlotsCount += 1;
            }

            [HarmonyPostfix]
            [HarmonyPatch("ButtonClick", typeof(StartMenuButtonType))]
            public static void ContinueLoadSlotMenuPost(StartMenuButtonType button, int __state)
            {
                if (button == StartMenuButtonType.Continue)
                    SaveSlots.activeSlotsCount = __state;
            }

            [HarmonyPrefix]
            [HarmonyPatch("EnableSettingsMenu")]
            public static bool BoatCameraNoMenu(GameObject ___logo)
            {
                if (BoatCamera.on && !(bool)GameState.currentShipyard && Plugin.boatCameraMenuZoom.Value.Equals("DisableMenu"))
                {
                    ___logo.SetActive(false);
                    return false;
                }
                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch("LateUpdate")]
            public static bool BoatCameraNoMenuSettingsToGame(StartMenu __instance)
            {
                if ((Input.GetKeyDown("escape") || Input.GetKeyDown("f10") || Input.GetKeyDown(KeyCode.JoystickButton6)) && 
                    GameState.playing && 
                    !GameState.currentlyLoading && 
                    !GameState.currentShipyard && 
                    BoatCamera.on && 
                    GameState.wasInSettingsMenu && 
                    Plugin.boatCameraMenuZoom.Value.Equals("DisableMenu"))
                {
                    __instance.InvokePrivateMethod("SettingsToGame");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(BackupSavesListUI))]
        private class BackupSavesListUIPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Awake")]
            public static void SetShowingListFor(ref int ___showingListFor)
            {
                ___showingListFor = -1;
            }
        }

        [HarmonyPatch(typeof(MouseButtonPointer))]
        private class MouseButtonPointerPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("LateUpdate")]
            public static bool DisableMouseInputLostFocus()
            {
                if (!Application.isFocused)
                    return false;
                return true;
            }
        }

        [HarmonyPatch(typeof(BoatCamera))]
        private class BoatCameraPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("Update")]
            public static bool BoatCameraSettingsMenu(bool ___on)
            {
                // Settings menu and try to switch to boat camera
                if (GameState.playing &&
                    !GameState.currentlyLoading &&
                    !GameState.currentShipyard &&
                    GameState.wasInSettingsMenu &&
                    !___on &&
                    GameInput.GetKeyDown(InputName.CameraMode))
                {
                    return false;
                }

                if (GameState.wasInSettingsMenu && 
                    !(bool)GameState.currentShipyard && 
                    ___on &&
                    Plugin.boatCameraMenuZoom.Value.Equals("DisableZoom"))
                {
                    return false;
                }

                if (GameState.wasInSettingsMenu && 
                    !(bool)GameState.currentShipyard && 
                    ___on && 
                    GameInput.GetKeyDown((InputName)16) && 
                    Plugin.boatCameraMenuZoom.Value.Equals("DisableMenu"))
                {
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GoPointer))]
        private static class GoPointerPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("DoRaycast")]
            public static bool DisableRaycastSpyglass(GoPointer __instance)
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

            [HarmonyPrefix]
            [HarmonyPatch("LateUpdate")]
            public static bool DisableAltActivatePlacing(GoPointer __instance, PickupableItem ___heldItem, GoPointerButton ___pointedAtButton)
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

        [HarmonyPatch(typeof(ShipItemChipLog))]
        private class ShipItemChipLogPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("OnBuy")]
            public static bool ChipLogOnBuy(
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

        [HarmonyPatch(typeof(CargoCarrier))]
        private class CargoCarrierPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("Awake")]
            public static bool SunspireFixHelper(CargoCarrier __instance)
            {
                if (__instance.portIndex == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
