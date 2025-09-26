using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace RadFixes
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class RF_Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.raddude82.radfixes";
        public const string PLUGIN_NAME = "RadFixes";
        public const string PLUGIN_VERSION = "1.1.11";

        internal static RF_Plugin Instance { get; private set; }
        private static ManualLogSource s_logger;

        internal static void LogDebug(string message) => s_logger.LogDebug(message);
        internal static void LogInfo(string message) => s_logger.LogInfo(message);
        internal static void LogWarning(string message) => s_logger.LogWarning(message);
        internal static void LogError(string message) => s_logger.LogError(message);

        internal static ConfigEntry<bool> enableFishingReelFix;
        internal static ConfigEntry<string> boatCameraMenuZoom;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            s_logger = Logger;

            enableFishingReelFix = Config.Bind("Settings", "Enable reel fix", true, "If enabled will rotate the fishing rod reel in the direction it should.");
            boatCameraMenuZoom = Config.Bind("Settings", "Boat camera settings menu zoom fix", "DisableZoom", new ConfigDescription("While paused in boat camera mode: DisableZoom will disable zooming in and out, DisableMenu will disable the menu, None will leave it as it is.", new AcceptableValueList<string>("DisableZoom", "DisableMenu", "None")));

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "island 1 A Gold Rock")
                SceneFixes.GoldRockCity();
            if (scene.name == "island 13 E (Sage Hills)")
                SceneFixes.SageHills();
            if (scene.name == "island 18 M (Oasis)")
                SceneFixes.HappyBay();
            if (scene.name == "island 19 M (Eastwind)")
                SceneFixes.Eastwind();
            if (scene.name == "island 25 (chronos)")
                SceneFixes.Chronos();
        }
    }
}
