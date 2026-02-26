using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePanelUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costLabelText;

    [Header("Buttons")]
    public Button actionButton;
    public TextMeshProUGUI actionButtonLabel;

    private TeacherConfig _currentConfig;

    private void Start()
    {
        actionButton?.onClick.AddListener(OnActionClick);
    }

    private void OnEnable()
    {
        GameEvents.OnTeacherLevelChanged += OnTeacherLevelChanged;
        GameEvents.OnTeacherUnlocked += OnTeacherUnlocked;
    }

    private void OnDisable()
    {
        GameEvents.OnTeacherLevelChanged -= OnTeacherLevelChanged;
        GameEvents.OnTeacherUnlocked -= OnTeacherUnlocked;
    }

    private void OnTeacherLevelChanged(string teacherID, int level)
    {
        if (_currentConfig != null && _currentConfig.TeacherID == teacherID)
            RefreshUI();
    }

    private void OnTeacherUnlocked(string teacherID)
    {
        if (_currentConfig != null && _currentConfig.TeacherID == teacherID)
            RefreshUI();
    }

    public void SetupUpgradeInfo(TeacherConfig config)
    {
        _currentConfig = config;
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_currentConfig == null) return;

        var data = TeacherManager.Instance.GetTeacher(_currentConfig.TeacherID);
        bool isUnlocked = data?.teacherStatus ?? false;
        int currentLevel = data?.teacherLevel ?? 0;

        if (!isUnlocked)
        {
            levelText.text = "LOCKED";
            actionButtonLabel.text = "MỞ KHÓA";

            int requiredCoin, requiredFame;
            TeacherManager.Instance.GetUnlockRequirements(_currentConfig.TeacherID, out requiredCoin, out requiredFame);

            string costStr = "";
            if (requiredFame > 0)
                costStr += $"{requiredFame} Fame ";

            if (requiredCoin > 0)
            {
                if (costStr.Length > 0) costStr += "+ ";
                costStr += $"{requiredCoin} Coin";
            }

            costLabelText.text =
                $"Chi phí mở khóa:\n<color=red>{costStr}</color>";

            actionButton.interactable =
                PlayerManager.Instance.GetFame() >= requiredFame &&
                PlayerManager.Instance.GetCoin() >= requiredCoin;
        }
        else
        {
            levelText.text = $"LV.{currentLevel}";

            var step = TeacherManager.Instance.GetNextLevelRequirements(_currentConfig.TeacherID);

            if (step == null || step.requiredItems.Count == 0)
            {
                actionButtonLabel.text = "MAX";
                costLabelText.text = "Đã đạt cấp tối đa";
                actionButton.interactable = false;
                return;
            }

            actionButtonLabel.text = "NÂNG CẤP";

            bool enoughAll = true;
            string costStr = "Yêu cầu:\n";

            foreach (var req in step.requiredItems)
            {
                int current = InventoryManager.Instance.GetAmount(req.itemID);
                if (current < req.amount) enoughAll = false;

                string itemName = InventoryManager.Instance.GetItemName(req.itemID);
                costStr += $"{req.amount} {itemName} (Có: {current})\n";
            }

            costLabelText.text = costStr.TrimEnd();
            actionButton.interactable = enoughAll;
        }
    }

    private void OnActionClick()
    {
        if (_currentConfig == null) return;

        var data = TeacherManager.Instance.GetTeacher(_currentConfig.TeacherID);
        if (data == null) return;

        if (!data.teacherStatus)
        {
            int requiredCoin, requiredFame;
            TeacherManager.Instance.GetUnlockRequirements(_currentConfig.TeacherID, out requiredCoin,out requiredFame);
            if (PlayerManager.Instance.GetCoin() >= requiredCoin && PlayerManager.Instance.GetFame() >= requiredFame)
            {
                PlayerManager.Instance.SpendCoin(requiredCoin);

                TeacherManager.Instance.Unlock(_currentConfig.TeacherID);
            }
            else
            {
                Debug.Log("Khong du tai nguyen");
            }
        }
        else
        {
            var step = TeacherManager.Instance.GetNextLevelRequirements(_currentConfig.TeacherID);

            if (step != null && step.requiredItems != null)
            {
                bool canUpgrade = true;
                foreach (var item in step.requiredItems)
                {
                    if (InventoryManager.Instance.GetAmount(item.itemID) < item.amount)
                    {
                        canUpgrade = false; break;
                    }
                }
                if (canUpgrade)
                {
                    foreach (var item in step.requiredItems)
                    {
                        InventoryManager.Instance.ConsumeItem(item.itemID, item.amount);
                    }
                    TeacherManager.Instance.Upgrade(_currentConfig.TeacherID);
                }
                else
                {
                    Debug.Log("Thieu vat pham can de nang cap");
                }
            }
        }
    }
}
