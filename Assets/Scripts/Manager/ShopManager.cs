using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-95)]
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int missionSlotCount = 6;
    [SerializeField] private int vipSlotCount = 6;
    [SerializeField] private int studySlotCount = 6;

    // ---- CONFIGS ----
    private ShopItemConfig[] missionConfigs;
    private ShopItemConfig[] vipConfigs;
    private ShopItemConfig[] toolConfigs;

    // ---- DATA ----
    private ShopSaveData shopData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LoadAllConfigs();
    }

    private void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.gameData != null)
        {
            Initialize(SaveManager.Instance.gameData.shop);
        }
        else
        {
            Debug.LogWarning("[ShopManager] SaveManager chưa sẵn sàng hoặc không có data. Khởi tạo data mới.");
            Initialize(new ShopSaveData());
        }

        CheckForDailyReset();
        GameEvents.OnShopRefreshed?.Invoke();
    }

    public void Initialize(ShopSaveData data)
    {
        if (data == null) data = new ShopSaveData();

        if (data.dailyMissionItems == null) data.dailyMissionItems = new List<ShopItemData>();
        if (data.dailyVipItems == null) data.dailyVipItems = new List<ShopItemData>();
        if (data.studyToolItems == null) data.studyToolItems = new List<ShopItemData>();

        shopData = data;

        CheckForDailyReset();
    }

    private void LoadAllConfigs()
    {
        missionConfigs = Resources.LoadAll<ShopItemConfig>("ShopItems/MissionItems");
        vipConfigs = Resources.LoadAll<ShopItemConfig>("ShopItems/VIPItems");
        toolConfigs = Resources.LoadAll<ShopItemConfig>("ShopItems/StudyTools");
    }

    public void CheckForDailyReset()
    {
        if (shopData == null) shopData = new ShopSaveData();
        var today = DateTime.Now.Date;
        var lastReset = shopData.lastResetTicks > 0 ? new DateTime(shopData.lastResetTicks) : DateTime.MinValue;

        bool needReset = (shopData.dailyMissionItems == null || shopData.dailyMissionItems.Count == 0 || lastReset < today);
        //Debug.Log($"[ShopManager] LastReset: {lastReset}, Today: {today} -> Need Reset? {needReset}");

        if (needReset)
        {
            shopData.dailyMissionItems = GenerateRandomItems(missionConfigs, missionSlotCount);
            shopData.dailyVipItems = GenerateRandomItems(vipConfigs, vipSlotCount);
            shopData.studyToolItems = GenerateRandomItems(toolConfigs, studySlotCount);
            shopData.lastResetTicks = today.Ticks;

            SaveManager.Instance?.SaveGame();
            //Debug.Log($"[ShopManager] ĐÃ TẠO MỚI {shopData.dailyMissionItems.Count} vật phẩm nhiệm vụ.");

            GameEvents.OnShopRefreshed?.Invoke();
        }
    }

    private List<ShopItemData> GenerateRandomItems(ShopItemConfig[] source, int count)
    {
        if (source == null || source.Length == 0) return new List<ShopItemData>();

        return source
            .OrderBy(_ => Guid.NewGuid())
            .Take(count)
            .Select(CreateItemData)
            .ToList();
    }

    private ShopItemData CreateItemData(ShopItemConfig config)
    {
        int quantity = UnityEngine.Random.Range(config.MinQuantityRange, config.MaxQuantityRange + 1);
        float priceModifier = UnityEngine.Random.Range(0.8f, 1.2f);
        int finalCost = Mathf.CeilToInt(config.basePrice * quantity * priceModifier);

        return new ShopItemData
        {
            IdConfig = string.IsNullOrEmpty(config.Id) ? config.name : config.Id,
            itemName = config.itemName,
            Category = config.Category,
            DisplayQuantity = quantity,
            FinalDiamondCost = Mathf.Max(1, finalCost),
            IsPurchased = false
        };
    }


    public List<ShopItemData> GetDailyMissionItems()
    {
        if (shopData == null)
        {
            return new List<ShopItemData>();
        }

        return shopData.dailyMissionItems ?? new List<ShopItemData>();
    }

    public List<ShopItemData> GetVipItems() => shopData?.dailyVipItems ?? new List<ShopItemData>();

    public List<ShopItemData> GetStudyToolItems() => shopData?.studyToolItems ?? new List<ShopItemData>();

    public ShopItemConfig GetItemConfig(string id)
    {
        return missionConfigs?.FirstOrDefault(x => x.Id == id || x.name == id)
            ?? vipConfigs?.FirstOrDefault(x => x.Id == id || x.name == id)
            ?? toolConfigs?.FirstOrDefault(x => x.Id == id || x.name == id);
    }
}