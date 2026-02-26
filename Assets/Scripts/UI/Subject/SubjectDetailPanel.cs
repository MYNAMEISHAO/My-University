using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Panel hiển thị chi tiết một môn học và cho phép nâng cấp / mở khóa bằng INK.
/// Đã chỉnh sửa để tương tác với SubjectManager (UpgradeSubject / UnlockSubject)
/// và lắng nghe GameEvents để tự động refresh UI.
/// </summary>
public class SubjectDetailPanel : MonoBehaviour
{
    [Header("Thông tin cơ bản")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI currentInk;

    [Header("Thông tin nâng cấp")]
    [SerializeField] private TextMeshProUGUI levelText;        // Level hiện tại
    [SerializeField] private TextMeshProUGUI incomeText;       // Thu nhập: hiện tại -> kế tiếp

    // Giữ tên biến cũ để không phải cập nhật Inspector, nhưng giờ hiển thị chi phí Ink
    [SerializeField] private TextMeshProUGUI coinCostText;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button upgradeByCoinButton; // thực chất là upgrade bằng Ink

    private SubjectConfig currentConfig;
    private SubjectData currentData;

    // Item id ink hiện đang dùng trong project (cập nhật nếu khác)
    private const string InkItemId = "Shop_Item_4";

    private void OnEnable()
    {
        GameEvents.OnSubjectLevelChanged += HandleSubjectLevelChanged;
        GameEvents.OnSubjectUnlocked += HandleSubjectUnlocked;
        GameEvents.OnItemChanged += HandleInkChanged;
    }


    private void OnDisable()
    {
        GameEvents.OnSubjectLevelChanged -= HandleSubjectLevelChanged;
        GameEvents.OnSubjectUnlocked -= HandleSubjectUnlocked;
        GameEvents.OnItemChanged -= HandleInkChanged;
    }

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
        }

        if (upgradeByCoinButton != null)
        {
            upgradeByCoinButton.onClick.RemoveAllListeners();
            upgradeByCoinButton.onClick.AddListener(OnUpgradeByInkClicked);
        }
    }

    public void ShowDetail(SubjectConfig config, SubjectData data)
    {
        if (config == null) return;

        currentConfig = config;
        currentData = data ?? (SubjectManager.Instance != null ? SubjectManager.Instance.GetSubject(config.ID) : null);

        if (nameText != null) nameText.text = config.Name;
        if (descriptionText != null) descriptionText.text = config.Description;
        if (iconImage != null) iconImage.sprite = config.Image;

        RefreshUpgradeUI();
        gameObject.SetActive(true);
    }

    private void RefreshUpgradeUI()
    {
        if (currentConfig == null)
        {
            if (levelText != null) levelText.text = "Lv. -";
            if (incomeText != null) incomeText.text = string.Empty;
            if (coinCostText != null) coinCostText.text = "-";
            if (upgradeByCoinButton != null) upgradeByCoinButton.interactable = false;
            return;
        }

        // Lấy runtime data nếu cần
        if (currentData == null && SubjectManager.Instance != null)
            currentData = SubjectManager.Instance.GetSubject(currentConfig.ID);

        bool isUnlocked = currentData != null && currentData.Status;
        int currentLevel = (currentData != null && currentData.currentLevel > 0) ? currentData.currentLevel : 1;
        int nextLevel = currentLevel + 1;

        float incomeNow = currentConfig.GetIncomeAtLevel(currentLevel);
        float incomeNext = currentConfig.GetIncomeAtLevel(nextLevel);

        int unlockCost = Mathf.CeilToInt(currentConfig.GetPriceAtLevel(1));
        int nextUpgradeCost = Mathf.CeilToInt(currentConfig.GetPriceAtLevel(nextLevel));

        if (levelText != null)
            levelText.text = isUnlocked ? "LV. " + currentLevel.ToString() : "LV. 0" ;

        if (incomeText != null)
            incomeText.text = isUnlocked ? $"Thu nhập: {incomeNow:0} -> {incomeNext:0}" : $"Thu nhập: - (chưa mở)";

        if (coinCostText != null)
            coinCostText.text = unlockCost.ToString();

        // Kiểm tra số Ink của người chơi (InventoryManager có thể là null)
        int playerInk = InventoryManager.Instance != null ? InventoryManager.Instance.GetAmount(InkItemId) : 0;

        bool interactable = false;
        int required = isUnlocked ? nextUpgradeCost : unlockCost;
        if (required > 0 && InventoryManager.Instance != null)
            interactable = playerInk >= required;

        if (upgradeByCoinButton != null)
        {
            upgradeByCoinButton.interactable = interactable;
            var txt = upgradeByCoinButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = isUnlocked ? required.ToString() + " Ink" : "Đang khóa)";
        }

        if (currentInk != null)
            currentInk.text = "Hiện sở hữu: " + InventoryManager.Instance.GetAmount(InkItemId).ToString();
    }

    // Khi người chơi bấm nút: gọi SubjectManager để xử lý (manager sẽ cập nhật data và phát event)
    public void OnUpgradeByInkClicked()
    {
        if (currentConfig == null || SubjectManager.Instance == null) return;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        // Lấy data tươi
        currentData = SubjectManager.Instance.GetSubject(currentConfig.ID);
        bool isUnlocked = currentData != null && currentData.Status;

        // Trước khi gọi manager, optional: kiểm tra đủ Ink ở UI level để disable nút tránh race
        int required = isUnlocked ? Mathf.CeilToInt(currentConfig.GetPriceAtLevel((currentData?.currentLevel ?? 1) + 1))
                                  : Mathf.CeilToInt(currentConfig.GetPriceAtLevel(1));

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager chưa sẵn sàng.");
            return;
        }

        if (InventoryManager.Instance.GetAmount(InkItemId) < required)
        {
            Debug.Log("Không đủ Ink để thực hiện thao tác.");
            RefreshUpgradeUI();
            return;
        }
        // NOTE: SubjectManager hiện chỉ thay đổi dữ liệu và phát event.
        // Nếu bạn muốn tiêu Ink ở central place, thực hiện InventoryManager.ConsumeItem trước khi gọi manager.
        // Ở đây tiêu ink ngay tại panel để đảm bảo atomicity UI->consume->manager.

        if (isUnlocked)
        {
            SubjectManager.Instance.UpgradeSubject(currentConfig.ID);
            if (!InventoryManager.Instance.ConsumeItem(InkItemId, required))
            {
                Debug.LogWarning("Tiêu Ink thất bại.");
                RefreshUpgradeUI();
                return;
            }
        }

        // Sau thao tác, refresh dữ liệu và UI
        currentData = SubjectManager.Instance.GetSubject(currentConfig.ID);
        RefreshUpgradeUI();
        // Có thể chơi animation / âm thanh ở đây
    }

    public void ClosePanel()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        UIManager.Instance.CloseSubjectDetailPanel();
    }

    // Event handlers
    private void HandleSubjectLevelChanged(string subjectID, int newLevel)
    {
        if (currentConfig == null) return;
        if (subjectID == currentConfig.ID)
        {
            currentData = SubjectManager.Instance.GetSubject(subjectID);
            RefreshUpgradeUI();
        }
    }

    private void HandleSubjectUnlocked(string subjectID)
    {
        if (currentConfig == null) return;
        if (subjectID == currentConfig.ID)
        {
            currentData = SubjectManager.Instance.GetSubject(subjectID);
            RefreshUpgradeUI();
        }
    }
    private void HandleInkChanged(string arg1, int arg2)
    {
        if (arg1 == InkItemId)
            RefreshUpgradeUI();
    }
}