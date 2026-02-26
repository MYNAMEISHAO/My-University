using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeClassroomUI : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject subjectSlotPrefab;
    [SerializeField] private GameObject seatPrefab;

    [Header("Containers")]
    [SerializeField] private Transform subjectSlotContainer;
    [SerializeField] private Transform seatListTransform;

    [Header("UI Elements - General")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private Image levelProgressBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("UI Elements - Detail Panel")]
    [SerializeField] private Image subjectDetailImage;
    [SerializeField] private TextMeshProUGUI subjectNameText;
    [SerializeField] private TextMeshProUGUI subjectDescriptionText;
    [SerializeField] private TextMeshProUGUI subjectIncomeText;
    [SerializeField] private TextMeshProUGUI subjectLevelText;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private TextMeshProUGUI roomIncomeText;

    [Header("Buttons")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button addSeatButton;

    // Data
    private RoomConfig currentRoomConfig;
    private RoomData currentRoomData;
    private List<SubjectData> currentSubjectDataList;
    private List<SubjectConfig> currentSubjectConfigList;
    private int currentSubjectIndex = 0;


    private void OnEnable()
    {
        // Đăng ký sự kiện
        GameEvents.OnCoinChanged += UpdateActionButtonsState;
        GameEvents.OnRoomLevelChanged += HandleRoomUpdate;
        GameEvents.OnRoomCapacityChanged += HandleRoomUpdate;
    }

    private void OnDisable()
    {
        // Hủy đăng ký sự kiện
        GameEvents.OnCoinChanged -= UpdateActionButtonsState;
        GameEvents.OnRoomLevelChanged -= HandleRoomUpdate;
        GameEvents.OnRoomCapacityChanged -= HandleRoomUpdate;

        // RESET: Xóa sạch mọi Object cũ để lần sau mở lên là "trang giấy trắng"
        ClearAllDynamicUI();
    }

    public void DisplayRoom(RoomConfig config, RoomData data)
    {
        // 1. Khởi tạo dữ liệu cơ bản
        currentRoomConfig = config;
        currentRoomData = data;
        currentSubjectIndex = 0;

        // 2. Dọn dẹp UI cũ (phòng hờ)
        ClearAllDynamicUI();

        // 3. Hiển thị tên phòng
        if (roomNameText) roomNameText.text = config.RoomID;

        // 4. Lấy dữ liệu môn học từ Manager
        currentSubjectDataList = SubjectManager.Instance?.GetAllSubjectOfRoom(config.RoomID) ?? new List<SubjectData>();
        for(int i = 0; i < currentSubjectDataList.Count; i++)
        {
            var subData = currentSubjectDataList[i];
            var subConfig = SubjectManager.Instance?.GetConfig(subData.SubjectConfigID);
            if (subConfig != null)
            {
                if (currentSubjectConfigList == null) currentSubjectConfigList = new List<SubjectConfig>();
                currentSubjectConfigList.Add(subConfig);
            }
        }

        // 5. Khởi tạo danh sách môn học từ Prefab
        BuildSubjectSlots();

        // 6. Khởi tạo danh sách ghế từ Prefab
        BuildSeatLayout();

        // 7. Cập nhật các thông tin khác
        RefreshRoomUI();
    }

    private void BuildSubjectSlots()
    {
        for (int i = 0; i < currentRoomConfig.MaxSubject; i++)
        {
            GameObject go = Instantiate(subjectSlotPrefab, subjectSlotContainer);
            if (i < currentSubjectConfigList.Count)
            {
                SubjectSlotButton slotScript = go.GetComponent<SubjectSlotButton>();

                if (slotScript != null)
                {
                    // Truyền dữ liệu vào từng slot
                    slotScript.Setup(i, this, currentSubjectConfigList[i]);

                    // Cập nhật trạng thái hiển thị (khung chọn)
                    slotScript.SetHighlight(i == currentSubjectIndex);
                }
            }
            else
            {
                go.transform.GetChild(0).gameObject.SetActive(false); // Ẩn icon nếu không có môn học
                go.transform.GetChild(1).gameObject.SetActive(false); // Ẩn khung chọn nếu không có môn học
            }
        }
    }

    private void BuildSeatLayout()
    {
        int maxSeat = currentRoomConfig.MaxSeat;
        int currentSeatCount = currentRoomData.SeatCount;

        // Cấu hình Grid dựa trên MaxSeat
        GridLayoutGroup grid = seatListTransform.GetComponent<GridLayoutGroup>();
        ConfigureSeatGrid(grid, maxSeat);

        // Tạo đủ số ghế tối đa
        for (int i = 0; i < maxSeat; i++)
        {
            GameObject seat = Instantiate(seatPrefab, seatListTransform);
            // Ghế nào đã mua thì hiện ảnh con, chưa mua thì ẩn ảnh con (giữ lại slot trống)
            if (seat.transform.childCount > 0)
            {
                seat.transform.GetChild(0).gameObject.SetActive(i < currentSeatCount);
            }
        }
    }

    public void OnSubjectSlotClicked(int index)
    {
        currentSubjectIndex = index;

        // Cập nhật Highlight cho các slot
        for (int i = 0; i < subjectSlotContainer.childCount; i++)
        {
            var slot = subjectSlotContainer.GetChild(i).GetComponent<SubjectSlotButton>();
            if (slot != null) slot.SetHighlight(i == currentSubjectIndex);
        }

        DisplaySubjectDetail();
    }

    private void DisplaySubjectDetail()
    {
        if (currentSubjectConfigList == null || currentSubjectIndex >= currentSubjectConfigList.Count) return;

        SubjectConfig config = currentSubjectConfigList[currentSubjectIndex];
        SubjectData data = currentSubjectDataList.Count > currentSubjectIndex ? currentSubjectDataList[currentSubjectIndex] : null;

        // Gán thông tin chi tiết
        if (subjectDetailImage)
        {
            subjectDetailImage.sprite = config.Image;
            subjectDetailImage.preserveAspect = true;
        }
        if (subjectNameText) subjectNameText.text = config.Name;
        if (subjectDescriptionText) subjectDescriptionText.text = config.Description;

        if (data != null)
        {
            if (subjectLevelText)
            {
                if (data.Status == false)
                {
                    subjectLevelText.gameObject.SetActive(false);
                    lockIcon.SetActive(true);
                }
                else
                {
                    subjectLevelText.text = $"LV {data.currentLevel}";
                    lockIcon.SetActive(false);
                }
            }
            if (subjectIncomeText) subjectIncomeText.text = $"Income: {Mathf.Round(config.GetIncomeAtLevel(data.currentLevel))}";

            lockIcon?.SetActive(!data.Status);
            subjectLevelText?.gameObject.SetActive(data.Status);
        }
    }

    private void RefreshRoomUI()
    {
        // Cập nhật Progress Bar
        if (levelProgressBar) levelProgressBar.fillAmount = (float)currentRoomData.currentLevel / currentRoomConfig.MaxLevel;
        if (levelText) levelText.text = $"Level {currentRoomData.currentLevel}";
        if (roomIncomeText) roomIncomeText.text = $"Học phí: {Mathf.Round(RoomManager.Instance.CalculateRoomIncome(currentRoomConfig.RoomID))}";

        // Cập nhật trạng thái nút bấm và giá tiền
        UpdateActionButtonsState(PlayerManager.Instance != null ? PlayerManager.Instance.GetCoin() : 0);

        // Cập nhật chi tiết môn học đang chọn
        DisplaySubjectDetail();
    }

    private void UpdateActionButtonsState(int playerCoins)
    {
        if (currentRoomConfig == null) return;

        // Upgrade Button
        int upgradeCost = currentRoomConfig.GetUpgradeCost(currentRoomData.currentLevel);
        bool canUpgrade = playerCoins >= upgradeCost;
        UpdateBtnUI(upgradeButton, canUpgrade, upgradeCost);

        // Add Seat Button
        int seatCost = currentRoomConfig.GetCapacityCost(currentRoomData.SeatCount);
        bool canAddSeat = playerCoins >= seatCost && currentRoomData.SeatCount < currentRoomConfig.MaxSeat;
        UpdateBtnUI(addSeatButton, canAddSeat, seatCost);
    }

    private void UpdateBtnUI(Button btn, bool interactable, int cost)
    {
        if (btn == null) return;
        btn.interactable = interactable;
        var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (txt) txt.text = cost.ToString();

        // Bật/Tắt hiệu ứng mờ nếu bạn có (GetChild 0/1 như code cũ)
        if (btn.transform.childCount >= 2)
        {
            btn.transform.GetChild(0).gameObject.SetActive(interactable); // Active state
            btn.transform.GetChild(1).gameObject.SetActive(!interactable); // Deactive state
        }
    }

    private void ClearAllDynamicUI()
    {
        foreach (Transform child in subjectSlotContainer) Destroy(child.gameObject);
        foreach (Transform child in seatListTransform) Destroy(child.gameObject);
        currentSubjectConfigList?.Clear();
    }

    private void HandleRoomUpdate(string roomID, int newValue)
    {
        if (currentRoomConfig != null && roomID == currentRoomConfig.RoomID)
        {
            // Thay vì build lại từ đầu (tốn hiệu năng), chỉ cần refresh UI
            // Nhưng nếu là tăng SeatCount thì cần Update ghế
            int currentSeatCount = currentRoomData.SeatCount;
            for (int i = 0; i < seatListTransform.childCount; i++)
            {
                Transform seat = seatListTransform.GetChild(i);
                if (seat.childCount > 0) seat.GetChild(0).gameObject.SetActive(i < currentSeatCount);
            }
            RefreshRoomUI();
        }
    }

    private void ConfigureSeatGrid(GridLayoutGroup grid, int maxSeat)
    {
        if (grid == null) return;
        switch (maxSeat)
        {
            case 9: 
                grid.constraintCount = 3; 
                grid.cellSize = new Vector2(64, 64); 
                grid.spacing = new Vector2(-40, -44);
                break;
            case 12: 
                grid.constraintCount = 4; 
                grid.cellSize = new Vector2(56, 56); 
                grid.spacing = new Vector2(-37, -40);
                break;
            case 16: 
                grid.constraintCount = 4;
                grid.cellSize = new Vector2(56, 56); 
                grid.spacing = new Vector2(-37, -40);
                break;
            default: grid.constraintCount = 3; break;
        }
    }

    public void OnUpgradeButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        int upgradeCost = currentRoomConfig.GetUpgradeCost(currentRoomData.currentLevel);

        if (PlayerManager.Instance.SpendCoin(upgradeCost))
            RoomManager.Instance.TryUpgradeRoom(currentRoomConfig.RoomID, currentRoomConfig);
    }
    public void OnAddSeatButtonClicked()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        int seatCost = currentRoomConfig.GetCapacityCost(currentRoomData.SeatCount);
            RoomManager.Instance.TryIncreaseCapacity(currentRoomConfig.RoomID, currentRoomConfig);
    }
    public void QuitRoomPanel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        UIManager.Instance?.CloseRoomPanel();
    }
}