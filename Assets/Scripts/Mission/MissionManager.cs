using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DefaultExecutionOrder(-95)]
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    // --- CÁC BIẾN THEO DÕI TIẾN ĐỘ ---
    public int maxLVTeacher = 0;
    public int maxLVSubject = 0;
    public int amountTeacher = 0;
    public int amountSubject = 0;
    public int fame = 0;
    public int coinMade = 0;

    private MissionData currentMissionData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnCoinChanged += HandleCoinChanged;
        GameEvents.OnFameChanged += HandleFameChanged;
        GameEvents.OnTeacherLevelChanged += HandleTeacherLevelChanged;
        GameEvents.OnSubjectLevelChanged += HandleSubjectLevelChanged;
        GameEvents.OnTeacherUnlocked += HandleTeacherUnlocked;
        GameEvents.OnSubjectUnlocked += HandleSubjectUnlocked;
    }

    private void OnDisable()
    {
        GameEvents.OnCoinChanged -= HandleCoinChanged;
        GameEvents.OnFameChanged -= HandleFameChanged;
        GameEvents.OnTeacherLevelChanged -= HandleTeacherLevelChanged;
        GameEvents.OnSubjectLevelChanged -= HandleSubjectLevelChanged;
        GameEvents.OnTeacherUnlocked -= HandleTeacherUnlocked;
        GameEvents.OnSubjectUnlocked -= HandleSubjectUnlocked;
    }

    // --- CÁC HÀM XỬ LÝ SỰ KIỆN ---
    // Chỉ cập nhật biến số, UI sẽ tự lắng nghe event để vẽ lại thanh bar
    private void HandleCoinChanged(int val) { coinMade = val; }
    private void HandleFameChanged(int val) { fame = val; }
    private void HandleTeacherLevelChanged(string id, int level) { UpdateMaxTeacherLevel(); }
    private void HandleSubjectLevelChanged(string id, int level) { UpdateMaxSubjectLevel(); }
    private void HandleTeacherUnlocked(string id) { UpdateTeacherCount(); }
    private void HandleSubjectUnlocked(string id) { UpdateSubjectCount(); }

    public void Initialize(MissionData missionData)
    {
        currentMissionData = missionData;
        Debug.Log("MissionManager initialized with Mission ID: " + currentMissionData.ID);
        if (PlayerManager.Instance != null)
        {
            coinMade = PlayerManager.Instance.GetCoin();
            fame = PlayerManager.Instance.GetFame();
        }

        UpdateMaxTeacherLevel();
        UpdateMaxSubjectLevel();
        UpdateTeacherCount();
        UpdateSubjectCount();
    }

    public static MissionData GenerateDefaultMissionData()
    {
        MissionData defaultMission = new MissionData(1);
        return defaultMission;
    }

    // --- CÁC HÀM TÍNH TOÁN LOGIC ---
    private void UpdateMaxTeacherLevel()
    {
        if (TeacherManager.Instance != null)
        {
            var list = TeacherManager.Instance.GetAllTeachers();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].teacherStatus && list[i].teacherLevel > maxLVTeacher)
                {
                    maxLVTeacher = list[i].teacherLevel;
                }
            }
        }
    }

    private void UpdateMaxSubjectLevel()
    {
        if (SubjectManager.Instance != null)
        {
            var list = SubjectManager.Instance.GetAllSubjects();
            if (list != null && list.Count > 0) maxLVSubject = list.Max(s => s.currentLevel);
        }
    }

    private void UpdateTeacherCount()
    {
        if (TeacherManager.Instance != null)
        {
            amountTeacher = TeacherManager.Instance.GetAllTeachers().Count(t => t.teacherStatus);
        }
    }

    private void UpdateSubjectCount()
    {
        if (SubjectManager.Instance != null)
        {
            amountSubject = SubjectManager.Instance.GetAllSubjects().Count(s => s.Status);
        }
    }

    private List<MissionConfig> cachedConfigs; // Thêm biến cache

    public List<MissionConfig> GetAllMissionConfigs()
    {
        if (cachedConfigs == null || cachedConfigs.Count == 0)
        {
            var configs = Resources.LoadAll<MissionConfig>("Missions");
            cachedConfigs = new List<MissionConfig>(configs);
            // Sắp xếp theo ID nếu cần để đảm bảo thứ tự
            cachedConfigs.Sort((a, b) => a.id.CompareTo(b.id));
        }
        return cachedConfigs;
    }

    public void UpdateMissionData()
    {
        if (currentMissionData == null) return;

        var allMissions = GetAllMissionConfigs();

        // Sửa điều kiện: Chỉ tăng nếu chưa vượt quá số lượng nhiệm vụ hiện có
        if (currentMissionData.ID <= allMissions.Count)
        {
            int finishedId = currentMissionData.ID;
            currentMissionData.ID += 1;

            if (SaveManager.Instance != null) SaveManager.Instance.SaveGame();

            // Phát sự kiện
            GameEvents.OnMissionCompleted?.Invoke(finishedId);
            Debug.Log($"[MissionManager] Nhiệm vụ {finishedId} xong. ID hiện tại: {currentMissionData.ID}");
        }
    }

    // Đã xóa hàm CheckProgress tự động tăng ID
    // Vì UI đã tự so sánh real-time rồi, không cần hàm này nữa.

    public MissionData GetCurrentMissionData()
    {
        if (currentMissionData == null)
        {
            Debug.LogWarning("MissionData is missing! Generating default data.");
            /*currentMissionData = GenerateDefaultMissionData();*/
        }
        else
        {
            Debug.Log("Current Mission ID: " + currentMissionData.ID);
        }
        return currentMissionData;
    }
}