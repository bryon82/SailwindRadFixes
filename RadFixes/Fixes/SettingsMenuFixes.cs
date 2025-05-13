using HarmonyLib;
using UnityEngine;
using System.Linq;
using static RadFixes.RF_Plugin;

namespace RadFixes
{
    internal class SettingsMenuFixes
    {
        [HarmonyPatch(typeof(StartMenu))]
        private class StartMenuPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Awake")]
            public static void DestroyDeco(GameObject ___settingsUI)
            {
                var deco = ___settingsUI.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == "deco (1)");
                if (deco == null)
                {
                    LogWarning("Settings menu tree removal fix not needed");
                    return;
                }
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
            [HarmonyPatch("EnableSettingsMenu")]
            public static bool BoatCameraNoMenu(GameObject ___logo)
            {
                if (BoatCamera.on && !(bool)GameState.currentShipyard && boatCameraMenuZoom.Value.Equals("DisableMenu"))
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
                    boatCameraMenuZoom.Value.Equals("DisableMenu"))
                {
                    __instance.InvokePrivateMethod("SettingsToGame");
                    return false;
                }
                return true;
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
                    boatCameraMenuZoom.Value.Equals("DisableZoom"))
                {
                    return false;
                }

                if (GameState.wasInSettingsMenu &&
                    !(bool)GameState.currentShipyard &&
                    ___on &&
                    GameInput.GetKeyDown((InputName)16) &&
                    boatCameraMenuZoom.Value.Equals("DisableMenu"))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
