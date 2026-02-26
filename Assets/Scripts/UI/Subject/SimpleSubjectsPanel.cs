using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleSubjectsPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform content;            // Content của ScrollView
    [SerializeField] private GameObject subjectItemPrefab;     // Prefab UI 1 môn
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;         // Panel MainMenu

    [Header("Layout")]
    [SerializeField] private int columns = 2;                  // Mỗi hàng 2 ô

    private readonly List<SimpleSubjectItemUI> runtimeItems = new List<SimpleSubjectItemUI>();
    private bool isBuilt = false;

    private void Awake()
    {
        SetupGridConstraint();
    }

    private void OnEnable()
    {
        if (!Application.isPlaying) return;

        // Rebuild list khi bật panel nếu chưa build
        if (!isBuilt)
        {
            BuildList();
            isBuilt = true;
        }
        else
        {
            // Nếu đã build trước đó, refresh để chắc chắn dữ liệu mới nhất
            RefreshList();
        }

        // Lắng nghe event thay đổi subject để cập nhật UI
        GameEvents.OnSubjectUnlocked += OnSubjectChanged_Rebuild;
        GameEvents.OnSubjectLevelChanged += OnSubjectChanged_Rebuild;
    }

    private void OnDisable()
    {
        GameEvents.OnSubjectUnlocked -= OnSubjectChanged_Rebuild;
        GameEvents.OnSubjectLevelChanged -= OnSubjectChanged_Rebuild;
    }

    private void SetupGridConstraint()
    {
        if (content == null) return;
        var grid = content.GetComponent<GridLayoutGroup>();
        if (grid != null)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = Mathf.Max(1, columns);
        }
    }

    // Public API: build list từ SubjectManager
    public void BuildList()
    {
        ClearAll();

        if (subjectItemPrefab == null || content == null)
        {
            Debug.LogError("SimpleSubjectsPanel: subjectItemPrefab hoặc content chưa được gán", this);
            return;
        }

        if (SubjectManager.Instance == null)
        {
            Debug.LogWarning("SimpleSubjectsPanel: SubjectManager.Instance null, không thể build danh sách.");
            return;
        }

        var allSubjectConfigs = SubjectManager.Instance.GetAllSubjectConfigs();
        var allSubjectData = SubjectManager.Instance.GetAllSubjects();

        if (allSubjectConfigs == null || allSubjectConfigs.Count == 0)
        {
            // Không có config để hiển thị
            return;
        }

        foreach (var cfg in allSubjectConfigs)
        {
            if (cfg == null) continue;

            // Tìm SubjectData tương ứng với config
            SubjectData subjectData = null;
            if (allSubjectData != null)
            {
                subjectData = allSubjectData.Find(d => d != null && d.SubjectConfigID == cfg.ID);
            }

            // Tạo Prefab
            GameObject itemGO = Instantiate(subjectItemPrefab, content);
            var itemUI = itemGO.GetComponent<SimpleSubjectItemUI>();

            // Nếu Prefab chưa có script thì tự add (phòng hờ)
            if (itemUI == null) itemUI = itemGO.AddComponent<SimpleSubjectItemUI>();

            // Setup hiển thị và đăng ký callback
            itemUI.Setup(cfg, subjectData, this);
            runtimeItems.Add(itemUI);
        }
    }

    // Rebuild hoặc refresh (hiện tại ta rebuild toàn bộ để đơn giản)
    public void RefreshList()
    {
        BuildList();
    }

    // Event handler rebuild khi có thay đổi Subject
    private void OnSubjectChanged_Rebuild(string arg1, int arg2)
    {
        RefreshList();
    }

    private void OnSubjectChanged_Rebuild(string subjectID)
    {
        RefreshList();
    }

    // Called from item UI when click
    public void OnSubjectClicked(SubjectConfig config, SubjectData data)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        if (config == null) return;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSubjectDetailPanel(config, data);
            return;
        }
        else
        {
            Debug.LogWarning("SimpleSubjectsPanel: Chưa gán DetailPanel và UIManager.Instance null!", this);
        }
    }

    public void CloseToMainMenu()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("click1");
        UIManager.Instance.CloseSubjectsListPanel();
    }

    private void ClearAll()
    {
        foreach (var ui in runtimeItems)
        {
            if (ui != null) Destroy(ui.gameObject);
        }
        runtimeItems.Clear();
    }
}