using HarmonyLib;
using UnityEngine;
using static RadFixes.RF_Plugin;

namespace RadFixes
{
    internal class UIFixes
    {
        [HarmonyPatch(typeof(EconomyUI))]
        private class EconomyUIPatches
        {
            // show page number correctly when opening the UI
            [HarmonyPatch("OpenUI")]
            public static void Postfix(
                EconomyUI __instance,
                EconomyUIButton[] ___goodsListButtons,
                ref TextMesh ___textPageNumber,
                int ___currentGoodsListPage)
            {
                int num = Mathf.CeilToInt((float)__instance.goodCount / ___goodsListButtons.Length);

                var currentPage = ___currentGoodsListPage;
                if (currentPage < 0)
                    currentPage = 0;
                if (currentPage > num - 1)
                    currentPage = num - 1;

                ___textPageNumber.text = $"{currentPage + 1} / {num}";
            }

            // duplicate item on last page was from going over the bounds of the goods list
            // deactivate the button if index is out of bounds
            [HarmonyPatch("RefreshGoodsList")]
            public static bool Prefix(
                EconomyUI __instance,
                EconomyUIButton[] ___goodsListButtons,
                int ___currentGoodsListPage,
                EconomyUIGoodsOrder ___goodsOrder,
                Material[] ___buttonMaterials,
                IslandMarket ___currentIsland)
            {
                int num = ___goodsListButtons.Length;
                for (int i = 0; i < num; i++)
                {
                    int buttonIndex = num * ___currentGoodsListPage + i + 1;
                    if (buttonIndex >= ___goodsOrder.goods.Length)
                    {
                        ___goodsListButtons[i].gameObject.SetActive(false);
                        continue;
                    }

                    ShipItem goodAt = ___goodsOrder.GetGoodAt(buttonIndex);
                    int goodIndex = ___goodsOrder.GetGoodIndex(buttonIndex);

                    if ((bool)goodAt && goodIndex != 51)
                    {
                        ___goodsListButtons[i].SetButtonText(goodAt.name);
                    }
                    else
                    {
                        ___goodsListButtons[i].SetButtonText("---");
                    }

                    if (!goodAt)
                    {
                        ___goodsListButtons[i].SetButtonMaterial(___buttonMaterials[1]);
                    }
                    else if (__instance.currentSelectedGood == goodIndex)
                    {
                        ___goodsListButtons[i].SetButtonMaterial(___buttonMaterials[2]);
                    }
                    else if (___currentIsland.currentPlayerGoods[goodIndex] > 0)
                    {
                        ___goodsListButtons[i].SetButtonMaterial(___buttonMaterials[3]);
                    }
                    else if (___currentIsland.HasGood(goodIndex))
                    {
                        ___goodsListButtons[i].SetButtonMaterial(___buttonMaterials[0]);
                    }
                    else
                    {
                        ___goodsListButtons[i].SetButtonMaterial(___buttonMaterials[1]);
                    }

                    ___goodsListButtons[i].SetButtonIndex(goodIndex);
                    ___goodsListButtons[i].gameObject.SetActive(true);
                }
                return false;
            }
        }

        [HarmonyBefore(RADREFINEMENTS_GUID)]
        [HarmonyPatch(typeof(GPButtonInventorySlot), "OnItemClick")]
        private class GPButtonInventorySlotPatches
        {
            // Fix for shopkeeper ui trying to buy item from player remains when item is placed in inventory slot
            public static void Prefix()
            {
                if (BuyItemUI.instance.menu.activeInHierarchy)
                {
                    BuyItemUI.instance.DeactivateUI();
                }
            }
        }
    }
}
