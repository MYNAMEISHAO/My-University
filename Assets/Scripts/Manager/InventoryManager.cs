using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.STP;

[DefaultExecutionOrder(-95)] // Chạy sau SaveManager
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private List<ItemStackData> _items;
    private Dictionary<string, ItemStackData> _itemDict = new Dictionary<string, ItemStackData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _items = new List<ItemStackData>();
        _itemDict = new Dictionary<string, ItemStackData>();
    }

    public void Initialize(List<ItemStackData> data)
    {
        _items = data ?? new List<ItemStackData>();

        _itemDict.Clear();
        foreach (var item in _items)
        {
            if (item != null)
                _itemDict[item.itemID] = item;
        }
        Debug.Log("InventoryManager đã khởi tạo xong.");
    }

    public static List<ItemStackData> GenerateDefaultItemStackData()
    {
        List<ItemStackData> defaultItems = new List<ItemStackData>();
        var configs = GetShopItemConfigs();
        foreach (var config in configs)
        {
            bool unlockedByDefault = false;
            var newData = new ItemStackData(
                config.Id,
                0
            );
            defaultItems.Add(newData);
        }
        //Debug.Log($"[SubjectManager] Tạo dữ liệu môn học mặc định, tổng số môn học: {defaultItems.Count}");
        return defaultItems;
    }

    public int GetAmount(string itemId)
    {
        if (_itemDict.TryGetValue(itemId, out var item))
        {
            return item.amount;
        }
        return 0;
    }

    public void AddItem(string itemID, int amount)
    {
        if (amount <= 0) return;

        if (_itemDict.TryGetValue(itemID, out var item))
        {
            item.amount += amount;
        }
        else
        {
            var newItem = new ItemStackData(itemID = itemID, amount = amount);
            _items.Add(newItem);
            _itemDict[itemID] = newItem;
        }

        if (SaveManager.Instance != null)
            SaveManager.Instance.gameData.inventory.items = _items;

        SaveManager.Instance?.SaveGame();
        GameEvents.OnItemChanged?.Invoke(itemID, amount);
    }


    public bool ConsumeItem(string itemId, int amount)
    {
        if (amount <= 0) return true;

        if (_itemDict.TryGetValue(itemId, out var item) && item.amount >= amount)
        {
            item.amount -= amount;
            SaveManager.Instance.SaveGame();
            GameEvents.OnItemChanged?.Invoke(itemId, -amount);
            return true;
        }

        Debug.LogWarning($"Không đủ vật phẩm: {itemId}");
        return false;
    }

    public List<ItemStackData> GetAllItems()
    {
        return _items;
    }

    public static List<ShopItemConfig> GetShopItemConfigs()
    {
        List<ShopItemConfig> shopItemConfigs = new List<ShopItemConfig>();
        var configs = Resources.LoadAll<ShopItemConfig>("ShopItems/MissionItems");
        foreach(var config in configs)
{           if(config.itemName != "Vàng")
            {
                bool isDuplicate = false;
                foreach (var item in shopItemConfigs)
                {
                    if (item.Id == config.Id)
                    {
                        isDuplicate = true;
                        break; // Chỉ thoát vòng lặp nhỏ để đánh dấu là trùng
                    }
                }
                if (!isDuplicate)
                {
                    shopItemConfigs.Add(config);
                }
            }
        }
        return shopItemConfigs;
    }

    public string GetItemName(string itemID)
    {
        var config = GetItemConfig(itemID);
        return config != null ? config.itemName : itemID;
    }

    public ShopItemConfig GetItemConfig(string itemID)
    {
        var configs = GetShopItemConfigs();
        return configs.Find(c => c.Id == itemID);
    }

}
