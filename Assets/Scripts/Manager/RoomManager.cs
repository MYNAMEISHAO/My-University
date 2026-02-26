using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-95)]
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    private List<RoomData> _roomList;
    private Dictionary<string, RoomData> _roomDataDict = new Dictionary<string, RoomData>();
    private Dictionary<string, RoomConfig> _configDict = new Dictionary<string, RoomConfig>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadAllConfigs();
    }
    private void LoadAllConfigs()
    {
        _configDict.Clear();
        var configs = Resources.LoadAll<RoomConfig>("Rooms");
        foreach (var cfg in configs)
        {
            if (!_configDict.ContainsKey(cfg.RoomID))
            {
                _configDict.Add(cfg.RoomID, cfg);
            }
            else
            {
                Debug.LogWarning($"[RoomManager] Phát hiện trùng ID '{cfg.RoomID}' trong file '{cfg.name}'");
            }
        }
    }

    public void Initialize(List<RoomData> roomList)
    {
        _roomList = roomList ?? new List<RoomData>();

        _roomDataDict.Clear();
        foreach (var room in _roomList)
        {
            if (room != null && !string.IsNullOrEmpty(room.roomConfigID))
            {
                _roomDataDict[room.roomConfigID] = room;
            }
        }

        CheckForNewRooms();
    }

    private static RoomData CreateDefaultData(RoomConfig config)
    {
        bool unlocked = config.UnlockCost == 0;
        return new RoomData(
            config.RoomID,
            unlocked,
            unlocked ? 1 : 0, 
            unlocked ? 1 : 0, 
            config.DefaultTeacherID
        );
    }

    public static List<RoomData> GenerateDefaultRoomData()
    {
        List<RoomData> defaultRooms = new List<RoomData>();
        var configs = Resources.LoadAll<RoomConfig>("Rooms");

        foreach (var config in configs)
        {
            defaultRooms.Add(CreateDefaultData(config));
        }
        return defaultRooms;
    }

    private void CheckForNewRooms()
    {
        bool hasNewData = false;

        foreach (var kvp in _configDict)
        {
            RoomConfig config = kvp.Value;
            if (!_roomDataDict.ContainsKey(config.RoomID))
            {
                var newData = CreateDefaultData(config);
                _roomList.Add(newData);
                _roomDataDict[config.RoomID] = newData;
                hasNewData = true;
            }
        }

        if (hasNewData) SaveManager.Instance.SaveGame();
    }

    public RoomData GetRoom(string roomID)
    {
        _roomDataDict.TryGetValue(roomID, out var data);
        return data;
    }

    public RoomConfig GetRoomConfig(string roomID)
    {
        _configDict.TryGetValue(roomID, out var config);

        if (config == null)
            Debug.LogError($"[RoomManager] Không tìm thấy Config ID: {roomID}");

        return config;
    }

    public void UnlockRoom(string roomID)
    {
        var data = GetRoom(roomID);
        data.isUnlocked = true;
        data.currentLevel = 1;      // Mở khóa xong thì lên lv 1
        data.SeatCount = 1;         // Có ít nhất 1 chỗ ngồi
        SaveAndNotify(roomID,data);
        GameEvents.OnRoomUnlocked?.Invoke(roomID);
    }

    public void TryUpgradeRoom(string roomID, RoomConfig config)
    {
        var data = GetRoom(roomID);
        if (data == null || !data.isUnlocked) return;
        if (data.currentLevel >= config.MaxLevel) return;
        data.currentLevel++;
        PlayerManager.Instance.AddFame(1);
        SaveAndNotify(roomID, data);

    }

    public void TryIncreaseCapacity(string roomID, RoomConfig config)
    {
        var data = GetRoom(roomID);
        if (data == null || !data.isUnlocked) return;
        if (data.SeatCount >= config.MaxSeat) return;
        data.SeatCount++;
        PlayerManager.Instance.AddFame(5);
        SaveAndNotify(roomID, data);

    }

    public bool AssignTeacher(string roomID, string teacherID)
    {
        var data = GetRoom(roomID);
        if (data == null) return false;

        var tData = TeacherManager.Instance.GetTeacher(teacherID);
        if (tData == null || !tData.teacherStatus) return false;

        data.currentTeacherID = teacherID;
        SaveAndNotify(roomID, data);
        return true;
    }

    private void SaveAndNotify(string roomID, RoomData data)
    {
        SaveManager.Instance.SaveGame();
        GameEvents.OnRoomLevelChanged?.Invoke(roomID, data.currentLevel);
        GameEvents.OnRoomCapacityChanged?.Invoke(roomID, data.SeatCount);
        GameEvents.OnRoomTeacherChanged?.Invoke(roomID, data.currentTeacherID);
    }

    public int CalculateRoomIncome(string roomID)
    {
        var data = GetRoom(roomID);
        if (data == null || !data.isUnlocked) return 0;

        var config = GetRoomConfig(roomID);
        if (config == null) return 0;

        // --- 1. TIỀN PHÒNG ---
        float totalIncome = config.GetIncome(data.currentLevel);

        // --- 2. TIỀN GIÁO VIÊN ---
        if (!string.IsNullOrEmpty(data.currentTeacherID))
        {
            var tConfig = TeacherManager.Instance.GetTeacherConfig(data.currentTeacherID);
            var tData = TeacherManager.Instance.GetTeacher(data.currentTeacherID);
            if (tConfig != null)
            {
                int tLevel = (tData != null) ? tData.teacherLevel : 1;
                totalIncome += tConfig.GetUpgradeValue(tLevel);
            }
        }

        // --- 3. TIỀN MÔN HỌC (SUBJECTS) ---
        if (SubjectManager.Instance != null)
        {
            var subjects = SubjectManager.Instance.GetAllSubjectOfRoom(roomID);
            if (subjects == null || subjects.Count == 0)
            {
                Debug.LogError($"❌ Phòng '{roomID}' không tìm thấy môn học nào! (Kiểm tra lại Room ID trong SubjectConfig chưa?)");
            }
            else
            {
                foreach (var subData in subjects)
                {
                    var subConfig = SubjectManager.Instance.GetConfig(subData.SubjectConfigID);

                    if (subConfig != null)
                    {
                        float curveValue = subConfig.GetIncomeAtLevel(subData.currentLevel);

                        if (subData.Status == true)
                        {
                            totalIncome += curveValue;
                        }
                    }
                    else Debug.LogError($"☠️ Có data môn '{subData.SubjectConfigID}' nhưng không tìm thấy Config!");
                }
            }
        }

        return Mathf.RoundToInt(totalIncome);
    }

    public int GetTotalIncomePerSecond()
    {
        int total = 0;
        foreach (var room in _roomList)
        {
            if (room.isUnlocked)
                total += CalculateRoomIncome(room.roomConfigID);
        }
        return total / 5;
    }
}