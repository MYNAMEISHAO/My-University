using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public PlayerSaveData player;
    public SchoolSaveData school;
    public ShopSaveData shop;
    public InventorySaveData inventory;
}
[Serializable]
public class SchoolSaveData
{
    public List<TeacherData> teachers;
    public List<BuildingData> buildings;
    public List<FloorData> floors;
    public List<RoomData> rooms;
    public List<SubjectData> subjects;
    public MissionData currentMission;
    public SpawnerData studentSpawner;
}

[Serializable]
public class ShopSaveData
{
    public List<ShopItemData> dailyMissionItems;
    public List<ShopItemData> dailyVipItems;      
    public List<ShopItemData> studyToolItems;
    public long lastResetTicks;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemStackData> items;
    public List<BookData> books;
}
