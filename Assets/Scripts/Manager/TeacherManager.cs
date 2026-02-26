using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-95)]
public class TeacherManager : MonoBehaviour
{
    public static TeacherManager Instance;

    private List<TeacherData> teacherListData;

    private Dictionary<string, TeacherData> teacherDict = new();
    private Dictionary<string, TeacherConfig> configDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadAllConfigsFromResources();
    }

    private void LoadAllConfigsFromResources()
    {
        configDict.Clear();
        TeacherConfig[] allConfigs = Resources.LoadAll<TeacherConfig>("Teachers");

        foreach (var config in allConfigs)
        {
            if (config != null && !string.IsNullOrEmpty(config.TeacherID))
            {
                if (!configDict.ContainsKey(config.TeacherID))
                {
                    configDict.Add(config.TeacherID, config);
                }
            }
        }
    }

    public static List<TeacherData> GenerateDefaultTeacherData()
    {
        List<TeacherData> defaultTeachers = new List<TeacherData>();
        var configs = Resources.LoadAll<TeacherConfig>("Teachers");
        foreach (var config in configs)
        {
            bool unlockedByDefault = config.IsInitiallyUnlocked;
            var newData = new TeacherData(
                config.TeacherID,
                unlockedByDefault,
                unlockedByDefault ? config.InitialLevel : 0
            );
            defaultTeachers.Add(newData);
        }
        return defaultTeachers;
    }

    public void Initialize(List<TeacherData> loadedData)
    {
        teacherListData = loadedData ?? new List<TeacherData>();

        teacherDict.Clear();
        foreach (var t in teacherListData)
        {
            if (!string.IsNullOrEmpty(t.teacherConfigID))
                teacherDict[t.teacherConfigID] = t;
        }

        SyncWithConfigs();
    }

    private void SyncWithConfigs()
    {
        if (configDict.Count == 0) LoadAllConfigsFromResources();

        bool hasNewData = false;

        foreach (var config in configDict.Values)
        {
            if (!teacherDict.ContainsKey(config.TeacherID))
            {
                var newData = new TeacherData(
                    config.TeacherID,
                    config.IsInitiallyUnlocked,
                    config.IsInitiallyUnlocked ? config.InitialLevel : 0
                );

                teacherListData.Add(newData);
                teacherDict[config.TeacherID] = newData;
                hasNewData = true;
                Debug.Log($"[TeacherManager] Đã thêm data mới cho GV: {config.TeacherID}");
            }
        }

        if (hasNewData)
        {
            SaveManager.Instance?.SaveGame();
        }
    }

    public TeacherData GetTeacher(string teacherID)
    {
        teacherDict.TryGetValue(teacherID, out var data);
        return data;
    }

    public TeacherConfig GetTeacherConfig(string teacherID)
    {
        if (string.IsNullOrEmpty(teacherID)) return null;

        if (configDict.ContainsKey(teacherID))
        {
            return configDict[teacherID];
        }

        return null;
    }

    public List<TeacherConfig> GetAllTeacherConfigs()
    {
        var list = new List<TeacherConfig>(configDict.Values);
        return list;
    }
    public List<TeacherData> GetAllTeachers()
    {
        return teacherListData;
    }
    public void Unlock(string teacherID)
    {
        var data = GetTeacher(teacherID);
        if (data == null) return;

        data.teacherStatus = true;
        if (data.teacherLevel <= 0)
            data.teacherLevel = 1;

        SaveManager.Instance?.SaveGame();
        GameEvents.OnTeacherUnlocked?.Invoke(teacherID);
    }

    public void Upgrade(string teacherID)
    {
        var data = GetTeacher(teacherID);
        if (data == null) return;

        data.teacherLevel++;

        SaveManager.Instance?.SaveGame();
        GameEvents.OnTeacherLevelChanged?.Invoke(teacherID, data.teacherLevel);
    }

    public bool IsUnlocked(string teacherID)
    {
        var data = GetTeacher(teacherID);
        return data != null && data.teacherStatus;
    }

    public int GetLevel(string teacherID)
    {
        var data = GetTeacher(teacherID);
        return data != null ? data.teacherLevel : 0;
    }

    public void GetUnlockRequirements(string teacherID, out int cost, out int fame)
    {
        cost = 0;
        fame = 0;
        var config = GetTeacherConfig(teacherID);
        if (config != null)
        {
            cost = config.UnlockCost;
            fame = config.UnlockFame;
        }
    }
    public UpgradeStep GetNextLevelRequirements(string teacherID)
    {
        var data = GetTeacher(teacherID);
        int currentLevel = (data != null) ? data.teacherLevel : 0;
        var config = GetTeacherConfig(teacherID);
        if (config == null) return null;
        return config.GetRequirementForNextLevel(currentLevel);
    }
}