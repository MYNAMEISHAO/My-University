using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-95)]
public class SubjectManager : MonoBehaviour
{
    public static SubjectManager Instance;

    public List<SubjectData> allSubjects { get; private set; } = new List<SubjectData>();
    private Dictionary<string, SubjectData> _subjectDataDict = new Dictionary<string, SubjectData>();
    private Dictionary<string, SubjectConfig> _configDict = new Dictionary<string, SubjectConfig>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadAllConfigsToCache();
    }

    private void LoadAllConfigsToCache()
    {
        _configDict.Clear();
        var configs = Resources.LoadAll<SubjectConfig>("Subjects");

        foreach (var config in configs)
        {
            if (config != null && !string.IsNullOrEmpty(config.ID))
            {
                if (!_configDict.ContainsKey(config.ID))
                {
                    _configDict.Add(config.ID, config);
                }
                else
                {
                    Debug.LogWarning($"⚠️ Trùng ID Môn Học '{config.ID}' ở file: {config.name}");
                }
            }
        }
        Debug.Log($" SubjectManager: Đã cache xong {_configDict.Count} môn học!");
    }

    public void Initialize(List<SubjectData> subjects)
    {
        allSubjects = subjects ?? new List<SubjectData>();
        RefreshDataDictionary();
        SyncSubjectsWithConfigs();
    }

    private void RefreshDataDictionary()
    {
        _subjectDataDict.Clear();
        foreach (var s in allSubjects)
        {
            if (s != null && !string.IsNullOrEmpty(s.SubjectConfigID))
            {
                _subjectDataDict[s.SubjectConfigID] = s;
            }
        }
    }

    private void SyncSubjectsWithConfigs()
    {
        if (_configDict.Count == 0) LoadAllConfigsToCache();

        bool hasNewData = false;

        foreach (var config in _configDict.Values)
        {
            if (!_subjectDataDict.ContainsKey(config.ID))
            {
                bool unlocked = false; 

                var newSub = new SubjectData(config.ID, unlocked, 0, config.RoomID);

                allSubjects.Add(newSub);
                _subjectDataDict[config.ID] = newSub;
                hasNewData = true;
                Debug.Log($"[SubjectManager] Phát hiện môn mới: {config.ID}");
            }
        }

        if (hasNewData)
        {
            SaveManager.Instance.SaveGame();
        }
    }

    public SubjectData GetSubject(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        _subjectDataDict.TryGetValue(id, out var data);
        return data;
    }

    public SubjectConfig GetConfig(string subjectID)
    {
        if (string.IsNullOrEmpty(subjectID)) return null;

        if (_configDict.ContainsKey(subjectID))
        {
            return _configDict[subjectID];
        }

        Debug.LogError($"❌ Không tìm thấy Config Môn Học ID: '{subjectID}'");
        return null;
    }

    public SubjectConfig GetRandomSubjectConfig()
    {
        int randomIndex = Random.Range(0, _configDict.Count);
        return _configDict.Values.ElementAt(randomIndex);
    }

    public void UnlockSubject(string subjectID)
    {
        var subject = GetSubject(subjectID);
        if (subject != null && !subject.Status)
        {
            subject.Status = true;
            subject.currentLevel = 1;
            SaveManager.Instance.SaveGame();
            Debug.Log($"[SubjectManager] Mở khóa môn học: {subjectID}");
            GameEvents.OnSubjectUnlocked?.Invoke(subjectID);
        }
    }

    public List<SubjectConfig> GetAllSubjectConfigs()
    {
        return new List<SubjectConfig>(_configDict.Values);
    }

    public List<SubjectData> GetAllSubjects()
    {
        return allSubjects;
    }

    public List<SubjectData> GetAllSubjectOfRoom(string roomID)
    {
        List<SubjectData> subjectsInRoom = new List<SubjectData>();
        if (allSubjects == null) return subjectsInRoom;

        foreach (var subject in allSubjects)
        {
            if (subject.roomID == roomID)
            {
                subjectsInRoom.Add(subject);
            }
        }
        return subjectsInRoom;
    }

    public static List<SubjectData> GenerateDefaultSubjectData()
    {
        List<SubjectData> defaultSubjects = new List<SubjectData>();
        var configs = Resources.LoadAll<SubjectConfig>("Subjects");
        foreach (var config in configs)
        {
            var newData = new SubjectData(
                config.ID,
                false,
                0,
                config.RoomID
            );
            defaultSubjects.Add(newData);
        }
        return defaultSubjects;
    }
    public void UpgradeSubject(string subjectID) 
    {
        var data = GetSubject(subjectID);
        if (data == null)
            return;
        data.currentLevel += 1;
        SaveManager.Instance?.SaveGame();
        GameEvents.OnSubjectLevelChanged?.Invoke(subjectID,data.currentLevel);
    }

    
       
    

}