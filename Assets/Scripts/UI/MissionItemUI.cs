using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MissionItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI missionNameText;
    public TextMeshProUGUI descriptionText;
    public Image progressBarImage2;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI rewardText;

    [Header("Buttons")]
    public Button receiveButton;

    private List<MissionConfig> allMissions;

    private void Awake()
    {
        // Cache lại danh sách nhiệm vụ ngay từ đầu
        if (MissionManager.Instance != null)
        {
            allMissions = MissionManager.Instance.GetAllMissionConfigs();
        }

        if (receiveButton != null) receiveButton.onClick.AddListener(OnReceiveButton);
    }

    // Sửa lỗi gạch đỏ: Dùng ngoặc nhọn thay vì dấu =>
    private void OnEnable()
    {
        SubscribeEvents();
        ShowMission(); // Cập nhật UI ngay khi bảng hiện lên
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    // Các hàm phụ trợ để khớp với delegate của GameEvents
    private void UpdateUIInt(int val) => ShowMission();
    private void 
        UpdateUIStringInt(string id, int lv) => ShowMission();
    private void UpdateUIString(string id) => ShowMission();

    void ShowMission()
    {
        if (MissionManager.Instance == null || allMissions == null) return;

        MissionData currentData = MissionManager.Instance.GetCurrentMissionData();
        int currentId = currentData.ID;

        // 1. Kiểm tra nếu đã hoàn thành tất cả nhiệm vụ
        if (currentId > allMissions.Count)
        {
            missionNameText.text = "HOÀN TẤT!";
            descriptionText.text = "Bạn là hiệu trưởng tài ba nhất!";
            rewardText.text = "";
            amountText.text = "MAX";
            if (progressBarImage2 != null) progressBarImage2.fillAmount = 1f;
            receiveButton.gameObject.SetActive(false);
            return;
        }

        // 2. Lấy dữ liệu nhiệm vụ hiện tại
        var m = allMissions[currentId - 1];
        missionNameText.text = m.missionName;
        descriptionText.text = m.description;
        rewardText.text = $"Thưởng: {m.rewardAmount} {m.reward}";

        float currentVal = GetCurrentValue(m.objective);
        float targetVal = Mathf.Max(1, m.objectiveAmount);

        if (progressBarImage2 != null) progressBarImage2.fillAmount = Mathf.Clamp01(currentVal / targetVal);
        amountText.text = $"{(int)currentVal} / {(int)targetVal}";

        // 3. Cập nhật trạng thái nút bấm
        bool isComplete = currentVal >= targetVal;
        receiveButton.gameObject.SetActive(true);
        var btnText = receiveButton.GetComponentInChildren<TextMeshProUGUI>();

        if (isComplete)
        {
            if (btnText) btnText.text = "Nhận Thưởng";
            receiveButton.image.color = Color.green;
        }
        else
        {
            if (btnText) btnText.text = "Làm Ngay";
            receiveButton.image.color = Color.white;
        }
    }

    private bool isProcessingReward = false; // Biến khóa bảo vệ mới

    // Sửa lại OnReceiveButton
    public void OnReceiveButton()
    {
        if (isProcessingReward) return; // Nếu đang xử lý thì thoát ngay

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");

        MissionData currentData = MissionManager.Instance.GetCurrentMissionData();
        int currentID = currentData.ID;

        if (allMissions == null || currentID > allMissions.Count) return;

        MissionConfig m = allMissions[currentID - 1];

        if (GetCurrentValue(m.objective) >= m.objectiveAmount)
        {
            StartCoroutine(ProcessRewardRoutine(m)); // Chuyển sang dùng Coroutine để kiểm soát luồng
        }
        else
        {
            NavigateToMission(m);
        }
    }

    private System.Collections.IEnumerator ProcessRewardRoutine(MissionConfig m)
    {
        isProcessingReward = true; // Khóa lại để chặn click đúp

        // --- NGẮT KẾT NỐI EVENT ĐỂ TRÁNH UI RELOAD ---
        UnsubscribeEvents();

        // 1. Kiểm tra và cộng thưởng dựa trên loại reward ghi trong Config
        if (PlayerManager.Instance != null)
        {
            switch (m.reward)
            {
                case "coin":
                    PlayerManager.Instance.AddCoin(m.rewardAmount); //
                    break;
                case "diamond":
                    PlayerManager.Instance.AddDiamond(m.rewardAmount); //
                    break;
                case "fame":
                    PlayerManager.Instance.AddFame(m.rewardAmount); //
                    break;
                case "researchPoint":
                    PlayerManager.Instance.AddResearchPoint(m.rewardAmount); //
                    break;
                case "energy":
                    PlayerManager.Instance.AddEnergy(m.rewardAmount); //
                    break;
                default:
                    Debug.LogWarning($"[MissionItemUI] Loại phần thưởng lạ: '{m.reward}'. Mặc định sẽ cộng Coin.");
                    PlayerManager.Instance.AddCoin(m.rewardAmount);
                    break;
            }
        }

        // 2. Tăng ID nhiệm vụ (Lưu game + Bắn event OnMissionCompleted)
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.UpdateMissionData(); //
        }

        // Đợi một khung hình để đảm bảo các Manager khác đã xử lý xong dữ liệu
        yield return null;

        // 3. Kết nối lại và cập nhật UI (ShowMission sẽ được gọi)
        SubscribeEvents();
        ShowMission();

        isProcessingReward = false; // Mở khóa
        Debug.Log($"[MissionItemUI] Đã nhận thưởng {m.rewardAmount} {m.reward} an toàn.");
    }

    private void SubscribeEvents()
    {
        GameEvents.OnCoinChanged += UpdateUIInt;
        GameEvents.OnFameChanged += UpdateUIInt;
        GameEvents.OnTeacherLevelChanged += UpdateUIStringInt;
        GameEvents.OnSubjectLevelChanged += UpdateUIStringInt;
        GameEvents.OnTeacherUnlocked += UpdateUIString;
        GameEvents.OnSubjectUnlocked += UpdateUIString;
        GameEvents.OnMissionCompleted += UpdateUIInt;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnCoinChanged -= UpdateUIInt;
        GameEvents.OnFameChanged -= UpdateUIInt;
        GameEvents.OnTeacherLevelChanged -= UpdateUIStringInt;
        GameEvents.OnSubjectLevelChanged -= UpdateUIStringInt;
        GameEvents.OnTeacherUnlocked -= UpdateUIString;
        GameEvents.OnSubjectUnlocked -= UpdateUIString;
        GameEvents.OnMissionCompleted -= UpdateUIInt;
    }

    private void NavigateToMission(MissionConfig m)
    {
        if (UIManager.Instance == null) return;

        switch (m.objective)
        {
            case "Coin":
                UIManager.Instance.ShowHomePanel();
                break;
            case "Fame":
                UIManager.Instance.ShowHomePanel();
                break;
            case "Diamond":
                UIManager.Instance.ShowHomePanel();
                break;
            case "TeacherLevel":
                UIManager.Instance.ShowTeacherListPanel();
                break;
            case "TeacherUnlocked":
                UIManager.Instance.ShowTeacherListPanel();
                break;
            case "Subject":
                UIManager.Instance.ShowSubjectsListPanel();
                break;
            case "SubjectUnlocked":
                UIManager.Instance.ShowResearchUI();
                break;
        }
        UIManager.Instance.CloseMissionPanel();
    }

    private float GetCurrentValue(string objective)
    {
        if (MissionManager.Instance == null) return 0;

        switch (objective)
        {
            case "Coin": return MissionManager.Instance.coinMade;
            case "Fame": return MissionManager.Instance.fame;
            case "TeacherLevel": return MissionManager.Instance.maxLVTeacher;
            case "Subject": return MissionManager.Instance.maxLVSubject;
            case "TeacherUnlocked": return MissionManager.Instance.amountTeacher;
            case "SubjectUnlocked": return MissionManager.Instance.amountSubject;
            default: return 0;
        }
    }

    public void OnCloseButton()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (UIManager.Instance != null) UIManager.Instance.CloseMissionPanel();
    }
}