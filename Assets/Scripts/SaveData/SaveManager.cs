using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
[DefaultExecutionOrder(-100)] // Chạy sớm nhất
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public GameSaveData gameData;
    private string saveFileName = "game_save.json";
    private string saveFilePath;

    private void Awake()
    {
        Instance = this;
        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        LoadGame();

    }
    private void Start()
    {
        if (PlayerManager.Instance != null) PlayerManager.Instance.Initialize(gameData.player);
        if (TeacherManager.Instance != null) TeacherManager.Instance.Initialize(gameData.school.teachers);
        if (RoomManager.Instance != null) RoomManager.Instance.Initialize(gameData.school.rooms);
        if (SubjectManager.Instance != null) SubjectManager.Instance.Initialize(gameData.school.subjects);
        if (BuildingManager.Instance != null) BuildingManager.Instance.Initialize(gameData.school.buildings);
        if (InventoryManager.Instance != null) InventoryManager.Instance.Initialize(gameData.inventory.items);
        if (MissionManager.Instance != null) MissionManager.Instance.Initialize(gameData.school.currentMission);
        if (BookManager.Instance != null) BookManager.Instance.Initialize(gameData.inventory.books);
        if (ShopManager.Instance != null) ShopManager.Instance.Initialize(gameData.shop);
        if (SpawnerManager.Instance != null) SpawnerManager.Instance.Initialize(gameData.school.studentSpawner);

    }
    private void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            CreateNewSaveFile();
            SaveGame();

            return;
        }

        string json = File.ReadAllText(saveFilePath);
        gameData = JsonUtility.FromJson<GameSaveData>(json);

        if (gameData == null)
        {
            CreateNewSaveFile();
        }
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        //Debug.Log(json);
        File.WriteAllText(saveFilePath, json);
    }

    private void CreateNewSaveFile()
    {
        gameData = new GameSaveData
        {
            player = new PlayerSaveData
            {
                ID = Guid.NewGuid().ToString(),
                Name = "New Player",
                playerCoin = 100,
                playerDiamond = 500,
                playerFame = 1000,
                Energy = 100,
                researchPoints = 0
            },
            school = new SchoolSaveData
            {
                teachers = TeacherManager.GenerateDefaultTeacherData(),
                buildings = BuildingManager.GenerateDefaultBuildingData(),
                floors = new List<FloorData>(),
                rooms = RoomManager.GenerateDefaultRoomData(),

                subjects = SubjectManager.GenerateDefaultSubjectData(),
                currentMission = MissionManager.GenerateDefaultMissionData(),
                studentSpawner = SpawnerManager.GenerateDefaultSpawnData()
               
            },
            inventory = new InventorySaveData
            {
                items = InventoryManager.GenerateDefaultItemStackData(),
                books = BookManager.GenerateDefaultBookData()
            },
            shop = new ShopSaveData
            {
                dailyMissionItems = new List<ShopItemData>(),
                dailyVipItems = new List<ShopItemData>(),
                studyToolItems = new List<ShopItemData>(),
                lastResetTicks = 0
            }
        };
        Debug.Log($"Đã tạo file save mới với {gameData.school.rooms.Count} phòng.");
    }
}