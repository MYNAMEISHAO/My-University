using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SHopItem", menuName = "Scriptable Objects/SHopItem")]
public class ShopItemConfig : ScriptableObject
{
    public string Id;
    public Sprite Icon;
    public string itemName;
    public string itemDescription;
    public int basePrice;
    public int tradePrice;
    public ItemCategory Category;
    [Header("Randomization")]
    public int MinQuantityRange;
    public int MaxQuantityRange;
}
