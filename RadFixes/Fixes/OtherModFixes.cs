using HarmonyLib;
using System;
using UnityEngine;
using static RadFixes.RF_Plugin;

namespace RadFixes
{
    internal class OtherModFixes
    {
        internal static Type cargoControllerType;
        internal static Type cargoControllerUIType;
        public static void PatchCargoControllerMod()
        {
            cargoControllerUIType = AccessTools.TypeByName("CargoControllerUI");
            cargoControllerType = AccessTools.TypeByName("CargoController.CargoController");

            var originalApplyMyGuiStylesMethod = AccessTools.Method(cargoControllerUIType, "OnGUI");
            var patchApplyMyGuiStylesMethod = AccessTools.Method(typeof(CargoControllerPatches), "ApplyMyGuiStyles");
            HarmonyInstance.Patch(originalApplyMyGuiStylesMethod, new HarmonyMethod(patchApplyMyGuiStylesMethod));
        }
    }

    public class CargoControllerPatches
    {
        private static GUISkin myGuiSkin;

        [HarmonyPrefix]
        public static bool ApplyMyGuiStyles(object __instance, bool ___m_visible,
        Font ___immortalFont, Texture2D ___reddishTexture,
        Texture2D ___lightBrownTexture, Font ___architectsFont)
        {
            if (!OtherModFixes.cargoControllerType.GetStaticProperty<bool>("Enabled") || !___m_visible)
            {
                return false;
            }

            if (myGuiSkin == null)
            {
                myGuiSkin = GameObject.Instantiate(GUI.skin);
                myGuiSkin.label.font = ___immortalFont;
                myGuiSkin.label.fontSize = 18;
                myGuiSkin.label.normal.textColor = Color.black;
                myGuiSkin.label.normal.background = ___reddishTexture;
                myGuiSkin.label.alignment = TextAnchor.MiddleCenter;

                myGuiSkin.button.font = ___architectsFont;
                myGuiSkin.button.fontSize = 14;
                myGuiSkin.button.normal.background = ___lightBrownTexture;
                myGuiSkin.button.normal.textColor = Color.black;
                myGuiSkin.button.alignment = TextAnchor.MiddleCenter;
            }

            GUISkin previousSkin = GUI.skin;
            GUI.skin = myGuiSkin;

            try
            {
                __instance.InvokePrivateMethod("DoBoatGoodsUI");
                __instance.InvokePrivateMethod("DoPortGoodsUI");
            }
            finally
            {
                GUI.skin = previousSkin;
            }

            return false;
        }
    }
}
