using UnityEngine;

public enum ItemCategory
{
    SchoolSupply,
    VIPItem,
    MissionItem
}
[System.Serializable]
public class ShopItemData
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string IdConfig;
    public string itemName;

    public ItemCategory Category;

    public int DisplayQuantity;
    public int FinalDiamondCost;

    public bool IsPurchased;
}
