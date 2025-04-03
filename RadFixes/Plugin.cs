using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace RadFixes
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.raddude82.radfixes";
        public const string PLUGIN_NAME = "RadFixes";
        public const string PLUGIN_VERSION = "1.1.1";

        internal static Plugin instance;
        internal static ManualLogSource logger;

        private void Awake()
        {
            instance = this;
            logger = Logger;

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
            if (scene.name == "island 35 M (valley)")
                SceneFixes.FeyValley();
        }
    }
}
