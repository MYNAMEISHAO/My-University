using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDetailPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI describeText;
    public Image characterImage;
    public Button closeButton;

    [Header("Attribute Section")]
    public TextMeshProUGUI attributeText;

    [Header("Modules")]
    public UpgradePanelUI upgradePanel;

    private TeacherConfig _currentConfig;

    private void Start()
    {
        closeButton?.onClick.AddListener(ClosePanel);
        gameObject.SetActive(false);
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

    private void OnTeacherLevelChanged(string teacherID, int newLevel)
    {
        RefreshIfCurrentTeacher(teacherID);
    }

    private void OnTeacherUnlocked(string teacherID)
    {
        RefreshIfCurrentTeacher(teacherID);
    }

    private void RefreshIfCurrentTeacher(string teacherID)
    {
        if (_currentConfig == null) return;
        if (_currentConfig.TeacherID != teacherID) return;

        RefreshUI();
    }

    public void ShowDetail(TeacherConfig config)
    {
        if (config == null) return;

        _currentConfig = config;

        nameText?.SetText(config.TeacherName);
        describeText?.SetText(config.TeacherDescription);
        if (characterImage) characterImage.sprite = config.TeacherSprite;

        RefreshUI();
        upgradePanel?.SetupUpgradeInfo(config);

        gameObject.SetActive(true);
    }

    private void RefreshUI()
    {
        var data = TeacherManager.Instance.GetTeacher(_currentConfig.TeacherID);
        bool isUnlocked = data?.teacherStatus ?? false;

        attributeText.gameObject.SetActive(isUnlocked);

        if (isUnlocked)
        {
            int level = data.teacherLevel;
            float earning = _currentConfig.GetUpgradeValue(level);
            attributeText.text = $"Thu nhập thêm:\n<color=yellow>+{earning:F0} xu</color>";
        }

        upgradePanel?.RefreshUI();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
