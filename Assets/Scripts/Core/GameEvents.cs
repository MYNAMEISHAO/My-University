using System;

public static class GameEvents
{
    // ----- RESOURCE EVENTS -----
    public static Action<int> OnCoinChanged;
    public static Action<int> OnDiamondChanged;
    public static Action<int> OnFameChanged;
    public static Action<int> OnResearchPointChanged;
    public static Action<int> OnEnergyChanged;

    // ----- TEACHER EVENTS -----
    public static Action<string> OnTeacherUnlocked;
    public static Action<string, int> OnTeacherLevelChanged;

    // ----- ROOM EVENTS -----
    public static Action<string> OnRoomUnlocked;
    public static Action<string, int> OnRoomLevelChanged;
    public static Action<string, int> OnRoomCapacityChanged;
    public static Action<string, string> OnRoomTeacherChanged;

    // ----- SPAWN EVENTS -----
    public static Action<int> OnSpawnerLevelChanged;

    // ----- MISSION EVENTS -----
    public static Action<int> OnMissionCompleted;

    // ----- SUBJECT EVENTS -----
    public static Action<string, int> OnSubjectLevelChanged;
    public static Action<string> OnSubjectUnlocked;

    // ----- SHOP EVENT
    public static Action OnShopRefreshed;
    public static Action<string, int> OnShopItemBought;
    public static Action<ShopItemData> OnShopItemPurchased;

    // ----- ITEM EVENTS -----
    public static Action<string, int> OnItemChanged;

    // ----- OFFLINE REWARD EVENT -----
    public static Action<int> OnOfflineGoldCalculated;
}
