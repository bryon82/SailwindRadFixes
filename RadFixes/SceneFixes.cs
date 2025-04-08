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
            SetShopkeeper(islandScenery, "shop (3)", "shopkeeper (5)");
            SetShopkeeper(islandScenery, "shop (7)", "shopkeeper (9)");
        }

        private static void SetShopkeeper(GameObject islandScenery, string shopName, string shopkeeperName)
        {
            var shop = islandScenery?.GetComponentsInChildren<ShopArea>().FirstOrDefault(k => k.name == shopName);
            var keeper = islandScenery?.GetComponentsInChildren<Shopkeeper>().FirstOrDefault(k => k.name == shopkeeperName);

            if (shop == null || keeper == null) return;

            shop.SetPrivateField("keeper", keeper);
            keeper.SetPrivateField("shop", shop);
        }

        internal static void SageHills()
        {
            // tavern shop keepers doesn't stay at night and cannot buy anything
            GameObject islandScenery = GameObject.Find("island 13 E (sage hills) scenery");

            var shop = islandScenery?.GetComponentsInChildren<ShopArea>().FirstOrDefault(k => k.name == "shop area (2)");

            if (shop == null) return;

            shop.openAtNight = true;
        }

        internal static void HappyBay()
        {
            // random guy at the top of town stays out all night and there are no available shops to attach him to
            GameObject islandScenery = GameObject.Find("island 18 M (Oasis) scenery");

            var shopkeeper = islandScenery?.GetComponentsInChildren<Shopkeeper>().FirstOrDefault(k => k.name == "shopkeeper");

            if (shopkeeper == null) return;

            islandScenery.gameObject.AddComponent<IslandNPCSchedule>();
            islandScenery.gameObject.GetComponent<IslandNPCSchedule>().SetPrivateField("keeper", shopkeeper);

            //// fix chair in ground... doesn't work. All the furniture appears to be some other item and all the same item
            //var chair = islandScenery?.GetComponentsInChildren<Transform>().FirstOrDefault(k => k.name == "furniture chair M");

            //if (chair != null)
            //    chair.localPosition = new Vector3(chair.localPosition[0], 20.4989f, chair.localPosition[2]);
        }

        internal static void Eastwind()
        {
            // tavern shopkeeper doesn't stay at night but a different shopkeeper does
            GameObject islandScenery = GameObject.Find("island 19 M (Eastwind) scenery");

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

            shop.SetPrivateField("keeper", keeper);
            keeper.SetPrivateField("shop", shop);
        }

        internal static void Chronos()
        {            
            // shopkeeper stays out after shops close 
            GameObject islandScenery = GameObject.Find("island 25 (chronos) scenery");            

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
                shop4.SetPrivateField("keeper", shopkeeper4);
                shopkeeper4.SetPrivateField("shop", shop4);
            }

            if (shop3 != null && shopkeeper3 != null) 
            {
                shop3.SetPrivateField("keeper", shopkeeper3);
                shopkeeper3.SetPrivateField("shop", shop3);
            }
        }
        
        internal static void FeyValley()
        {
            // cargo transport dude hire button broken
            GameObject islandScenery = GameObject.Find("scenery");

            var cargoTransportDude = islandScenery?.GetComponentsInChildren<CargoTransportDude>().FirstOrDefault(k => k.name == "transport dude");
            
            if (cargoTransportDude == null) return;

            cargoTransportDude.carrierIndex = 35;
        }
    }
}
