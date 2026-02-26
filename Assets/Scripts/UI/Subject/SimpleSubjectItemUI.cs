using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleSubjectItemUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button clickButton; // Button de bam vao item (co the chinh la Button tren prefab)

    private SubjectConfig config;
    private SubjectData data;
    private SimpleSubjectsPanel parentPanel;

    // Chi can config + panel, khong can SubjectData/SaveDataJson
    public void Setup(SubjectConfig cfg, SubjectData subjectData, SimpleSubjectsPanel panel)
    {
        config = cfg;
        data = subjectData;
        parentPanel = panel;

        if (config != null)
        {
            if (iconImage != null) iconImage.sprite = config.Image;
            if (nameText != null) nameText.text = config.Name;
        }

        Button btn = clickButton;
        if (btn == null)
        {
            // Neu chua gan rieng, thu lay Button tren chinh GameObject nay
            btn = GetComponent<Button>();
        }

        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (config == null || parentPanel == null) return;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("success");
        }
        parentPanel.OnSubjectClicked(config, data);
    }
}

