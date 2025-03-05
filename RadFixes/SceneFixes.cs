using System.Linq;
using UnityEngine;

namespace RadFixes
{
    internal class SceneFixes
    {
        internal static void GoldRockCity()
        {
            // 2 shop keepers stay out after shops close
            GameObject islandScenery = GameObject.Find("island 1 A (gold rock) scenery");
            setShopkeeper(islandScenery, "shop (3)", "shopkeeper (5)");
            setShopkeeper(islandScenery, "shop (7)", "shopkeeper (9)");
        }

        private static void setShopkeeper(GameObject islandScenery, string shopName, string shopkeeperName)
        {
            var shop = islandScenery?.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == shopName);
            var shopArea = shop?.gameObject.GetComponent<ShopArea>();

            var shopkeeper = islandScenery?.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == shopkeeperName);
            var keeper = shopkeeper?.gameObject.GetComponent<Shopkeeper>();

            if (shopArea == null || keeper == null) return;

            shopArea.SetPrivateField("keeper", keeper);
            keeper.SetPrivateField("shop", shopArea);
        }

        internal static void SageHills()
        {
            // tavern shop keepers doesn't stay at night and cannot buy anything
            GameObject islandScenery = GameObject.Find("island 13 E (sage hills) scenery");

            var shop = islandScenery?.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == "shop area (2)");
            var shopArea = shop?.gameObject.GetComponent<ShopArea>();

            if (shopArea == null) return;

            shopArea.openAtNight = true;
        }

        internal static void HappyBay()
        {
            // random guy at the top of town stays out all night and there are no available shops to attach him to
            GameObject islandScenery = GameObject.Find("island 18 M (Oasis) scenery");

            var shopkeeper = islandScenery?.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == "shopkeeper");

            if (shopkeeper == null) return;

            islandScenery.gameObject.AddComponent<IslandNPCSchedule>();
            islandScenery.gameObject.GetComponent<IslandNPCSchedule>().SetPrivateField("keeper", shopkeeper.gameObject.GetComponent<Shopkeeper>());

            //// fix chair in ground... doesn't work
            //var chair = islandScenery?.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == "furniture chair M");

            //if (chair != null)
            //    chair.localPosition = new Vector3(chair.localPosition[0], 20.4989f, chair.localPosition[2]);
        }

        internal static void Eastwind()
        {
            // tavern shopkeeper doesn't stay at night but a different shopkeeper does
            GameObject islandScenery = GameObject.Find("island 19 M (Eastwind) scenery");

            var shopkeepers = islandScenery?.GetComponentsInChildren<Transform>().Where(k => k.name == "shopkeeper (1)");
            Shopkeeper keeper = null;
            foreach (var sk in shopkeepers)
            {
                keeper = sk.gameObject.GetComponent<Shopkeeper>();
                if (sk.GetPrivateField<ShopArea>("shop") == null)
                {
                    sk.name = "shopkeeper (3)";
                    break;
                }
            }

            var shop = islandScenery?.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == "shop area (2)");
            var shopArea = shop?.gameObject.GetComponent<ShopArea>();

            if (shopArea == null || keeper == null) return;
            
            shopArea.SetPrivateField("keeper", keeper);
            keeper.SetPrivateField("shop", shopArea);
        }

        internal static void Chronos()
        {
            // shopkeeper stays out after shops close
            GameObject islandScenery = GameObject.Find("island 25 (chronos) scenery");            

            var shops = islandScenery?.GetComponentsInChildren<Transform>().Where(k => k.name == "shop area");
            ShopArea shopArea = null;
            foreach (var shop in shops)
            {
                shopArea = shop.gameObject.GetComponent<ShopArea>();
                if (shopArea.itemsForSale.Count > 0 && shopArea.itemsForSale[0].name == "bun")
                    break;
            }

            var shopkeeper = islandScenery.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == "shopkeeper (3)");
            var keeper = shopkeeper.gameObject.GetComponent<Shopkeeper>();

            if (shopArea == null || keeper == null) return;

            shopArea.SetPrivateField("keeper", keeper);
            keeper.SetPrivateField("shop", shopArea);
        }               
    }
}
