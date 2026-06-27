using BepInEx.Configuration;

namespace RadFixes
{
    internal class Configs
    {
        internal static ConfigEntry<bool> enableFishingReelFix;
        internal static ConfigEntry<string> boatCameraMenuZoom;
        internal static ConfigEntry<bool> enableSinkingItemsFix;

        internal static void InitializeConfigs()
        {
            var config = RF_Plugin.Instance.Config;

            enableFishingReelFix = config.Bind(
                "Settings", 
                "Fishing rod reel fix",
                true,
                "If enabled will rotate the fishing rod reel in the direction it should.");

            enableSinkingItemsFix = config.Bind(
                "Settings",
                "Sinking items fix",
                true);

            boatCameraMenuZoom = config.Bind(
                "Settings",
                "Boat camera settings menu zoom fix",
                "DisableZoom",
                new ConfigDescription(
                    "While paused in boat camera mode: DisableZoom will disable zooming in and out, DisableMenu will disable the menu, None will leave it as it is.",
                    new AcceptableValueList<string>(
                        "DisableZoom",
                        "DisableMenu",
                        "None")));
        }
    }
}
