using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace RadFixes
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(CARGOCONTROLLER_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class RF_Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.raddude.radfixes";
        public const string PLUGIN_NAME = "RadFixes";
        public const string PLUGIN_VERSION = "1.3.3";

        public const string RADREFINEMENTS_GUID = "com.raddude.radrefinements";
        public const string CARGOCONTROLLER_GUID = "com.jakeinaboat.cargocontroller";

        internal static RF_Plugin Instance { get; private set; }
        private static ManualLogSource _logger;
        internal static Harmony HarmonyInstance { get; private set; }

        internal static void LogDebug(string message) => _logger.LogDebug(message);
        internal static void LogInfo(string message) => _logger.LogInfo(message);
        internal static void LogWarning(string message) => _logger.LogWarning(message);
        internal static void LogError(string message) => _logger.LogError(message);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _logger = Logger;

            Configs.InitializeConfigs();
            HarmonyInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);
            SceneManager.sceneLoaded += SceneFixes.SceneLoaded;

            foreach (var plugin in Chainloader.PluginInfos)
            {
                var metadata = plugin.Value.Metadata;
                if (Configs.enableModGUIFix.Value && metadata.GUID.Equals(CARGOCONTROLLER_GUID))
                {
                    LogInfo("CargoController mod found");
                    OtherModFixes.PatchCargoControllerMod();
                }
            }
        }
    }
}
