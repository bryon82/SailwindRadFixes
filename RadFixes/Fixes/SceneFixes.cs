using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static RadFixes.RF_Plugin;

namespace RadFixes
{
    internal class SceneFixes
    {
        internal static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "island 1 A Gold Rock")
                GoldRockCity();
            if (scene.name == "island 13 E (Sage Hills)")
                SageHills();
            if (scene.name == "island 18 M (Oasis)")
                HappyBay();
            if (scene.name == "island 19 M (Eastwind)")
                Eastwind();
            if (scene.name == "island 25 (chronos)")
                Chronos();
        }

        // 2 shop keepers stay out after shops close
        private static void GoldRockCity()
        {
            var islandScenery = GameObject.Find("island 1 A (gold rock) scenery");
            SetShopkeeper(islandScenery, "shop (3)", "shopkeeper (5)");
            SetShopkeeper(islandScenery, "shop (7)", "shopkeeper (9)");
        }

        private static void SetShopkeeper(GameObject islandScenery, string shopName, string shopkeeperName)
        {
            var shop = islandScenery?.GetComponentsInChildren<ShopArea>().FirstOrDefault(k => k.name == shopName);
            var keeper = islandScenery?.GetComponentsInChildren<Shopkeeper>().FirstOrDefault(k => k.name == shopkeeperName);

            if (shop == null || keeper == null) return;

            if (shop.GetShopkeeper() == keeper && keeper.GetPrivateField<ShopArea>("shop") == shop)
            {
                LogWarning($"Gold Rock City shop fix not needed for {shopkeeperName} - {shopName}");
                return;
            }

            shop.SetPrivateField("keeper", keeper);
            keeper.SetPrivateField("shop", shop);
        }

        // tavern shop keeper doesn't stay at night and cannot buy anything
        private static void SageHills()
        {
            var islandScenery = GameObject.Find("island 13 E (sage hills) scenery");

            var shop = islandScenery?.GetComponentsInChildren<ShopArea>().FirstOrDefault(k => k.name == "shop area (2)");

            if (shop == null)
                return;

            if (shop.openAtNight)
            {
                LogWarning("Sage Hills shop fix not needed");
                return;
            }                

            shop.openAtNight = true;
        }

        // random guy at the top of town stays out all night and there are no available shops to attach him to
        private static void HappyBay()
        {
            var islandScenery = GameObject.Find("island 18 M (Oasis) scenery");

            var shopkeeper = islandScenery?.GetComponentsInChildren<Shopkeeper>().FirstOrDefault(k => k.name == "shopkeeper");

            if (shopkeeper == null)
                return;

            var npcShedule = islandScenery.gameObject.AddComponent<RF_NPCSchedule>();
            npcShedule.SetPrivateField("_keeper", shopkeeper);

            //// fix chair in ground... doesn't work. All the furniture appears to be some other item and all the same item
            //var chair = islandScenery?.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == "furniture chair M");

            //if (chair != null)
            //    chair.localPosition = new Vector3(chair.localPosition[0], 20.4989f, chair.localPosition[2]);
        }

        // tavern shopkeeper doesn't stay at night but a different shopkeeper does
        private static void Eastwind()
        {
            var islandScenery = GameObject.Find("island 19 M (Eastwind) scenery");

            var shopkeepers = islandScenery?.GetComponentsInChildren<Shopkeeper>().Where(k => k.name == "shopkeeper (1)");
            Shopkeeper keeper = null;
            foreach (var shopkeeper in shopkeepers)
            {
                keeper = shopkeeper;
                if (shopkeeper.GetPrivateField<ShopArea>("shop") == null)
                {
                    shopkeeper.name = "shopkeeper (3)";
                    break;
                }
            }

            var shop = islandScenery?.GetComponentsInChildren<ShopArea>().FirstOrDefault(k => k.name == "shop area (2)");

            if (shop == null || keeper == null) return;

            if (shop.GetShopkeeper() == keeper && keeper.GetPrivateField<ShopArea>("shop") == shop)
            {
                LogWarning($"Eastwind shop fix not needed");
                return;
            }

            shop.SetPrivateField("keeper", keeper);
            keeper.SetPrivateField("shop", shop);
        }

        // shopkeeper stays out after shops close 
        private static void Chronos()
        {
            var islandScenery = GameObject.Find("island 25 (chronos) scenery");

            var shopAreas = islandScenery?.GetComponentsInChildren<ShopArea>().Where(k => k.name == "shop area");
            ShopArea shop3 = null;
            ShopArea shop4 = null;
            foreach (var shopArea in shopAreas)
            {
                if (shopArea.itemsForSale.Count > 0 && shopArea.itemsForSale[0].name == "bun")
                    shop3 = shopArea;
                if (shopArea.itemsForSale.Count > 0 && shopArea.itemsForSale[0].name == "trout")
                    shop4 = shopArea;
            }

            var shopkeeper3 = islandScenery?.GetComponentsInChildren<Shopkeeper>().FirstOrDefault(k => k.name == "shopkeeper (3)");
            var shopkeeper4 = islandScenery?.GetComponentsInChildren<Shopkeeper>().FirstOrDefault(k => k.name == "shopkeeper (4)");

            if (shop4 != null && shopkeeper4 != null)
            {
                if (shop4.GetShopkeeper() == shopkeeper4 && shopkeeper4.GetPrivateField<ShopArea>("shop") == shop4)
                {
                    LogWarning($"Chronos shop fix not needed for {shopkeeper4.name} - {shop4.name}");                    
                }
                shop4.SetPrivateField("keeper", shopkeeper4);
                shopkeeper4.SetPrivateField("shop", shop4);
            }

            if (shop3 != null && shopkeeper3 != null) 
            {
                if (shop3.GetShopkeeper() == shopkeeper3 && shopkeeper3.GetPrivateField<ShopArea>("shop") == shop3)
                {
                    LogWarning($"Chronos shop fix not needed for {shopkeeper3.name} - {shop3.name}");                    
                }
                shop3.SetPrivateField("keeper", shopkeeper3);
                shopkeeper3.SetPrivateField("shop", shop3);
            }
        }
    }
}
