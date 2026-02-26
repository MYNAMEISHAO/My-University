using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI describeText;
    public TextMeshProUGUI levelText;
    public Image imageCharacter;
    public Button selectButton;

    private TeacherData _currentData;
    private TeacherConfig _currentConfig;
    private CharacterListUI characterListUI;

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
        if (_currentConfig != null && _currentConfig.TeacherID == teacherID)
        {
            UpdateUI(_currentConfig);
        }
    }

    private void OnTeacherUnlocked(string teacherID)
    {
        if (_currentConfig != null && _currentConfig.TeacherID == teacherID)
        {
            UpdateUI(_currentConfig);
        }
    }


    public void Setup(TeacherConfig config, CharacterListUI listUI)
    {
        _currentConfig = config;
        characterListUI = listUI;

        UpdateUI(config);

        if (selectButton == null) selectButton = GetComponent<Button>();
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSelect);
    }

    private void UpdateUI(TeacherConfig config)
    {
        if (config == null) return;

        var data = TeacherManager.Instance.GetTeacher(config.TeacherID);

        if (nameText != null) nameText.text = config.TeacherName;
        if (describeText != null) describeText.text = config.TeacherDescription;
        if (imageCharacter != null) imageCharacter.sprite = config.TeacherSprite;

        if (levelText != null)
        {
            if (data != null && data.teacherStatus)
            {
                levelText.text = "Lv. " + data.teacherLevel;
            }
            else
            {
                levelText.text = "LOCKED";
            }
        }
    }

    private void OnSelect()
    {
        if (characterListUI != null)
        {
            characterListUI.OnTeacherSelected(_currentConfig);
        }
    }
}