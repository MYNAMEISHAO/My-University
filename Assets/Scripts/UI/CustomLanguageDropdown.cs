using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomLanguageDropdown : MonoBehaviour
{
    [Header("=== Tham chiếu UI ===")]
    [SerializeField] private GameObject template;           // Template (phần sổ xuống)
    [SerializeField] private TMP_Text mainLabel;            // "Tiếng Việt" trên cùng
    [SerializeField] private GameObject itemViet;           // ItemTiengViet
    [SerializeField] private GameObject itemAnh;            // ItemTiengAnh

    [Header("=== Thành phần con ===")]
    [SerializeField] private TMP_Text vietText;
    [SerializeField] private Image vietBackground;
    [SerializeField] private TMP_Text anhText;
    [SerializeField] private Image anhBackground;

    private bool isOpen = false;

    void Start()
    {
        // Ẩn template lúc đầu
        template.SetActive(false);

        // Gán sự kiện bấm vào mainLabel
        if (mainLabel != null)
        {
            var trigger = mainLabel.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var entry = new UnityEngine.EventSystems.EventTrigger.Entry
            {
                eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) => { OnMainClick(); });
            trigger.triggers.Add(entry);
        }

        // Gán sự kiện cho từng item
        AddClickToItem(itemViet, 0); // 0 = Tiếng Việt
        AddClickToItem(itemAnh, 1);  // 1 = Tiếng Anh

        // Khởi tạo: chọn mặc định Tiếng Việt
        SelectLanguage(0);
    }

    void OnMainClick()
    {
        isOpen = !isOpen;
        template.SetActive(isOpen);
    }

    void AddClickToItem(GameObject item, int langIndex)
    {
        if (item == null) return;

        var trigger = item.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        var entry = new UnityEngine.EventSystems.EventTrigger.Entry
        {
            eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick
        };
        entry.callback.AddListener((data) => { SelectLanguage(langIndex); });
        trigger.triggers.Add(entry);
    }

    void SelectLanguage(int index)
    {
        // Ẩn template sau khi chọn
        template.SetActive(false);
        isOpen = false;

        // Cập nhật Label chính
        string selectedText = index == 0 ? "Tiếng Việt" : "Tiếng Anh";
        mainLabel.text = selectedText;

        // Cập nhật trạng thái 2 item
        SetItemState(vietText, vietBackground, index == 0);
        SetItemState(anhText, anhBackground, index == 1);

        // TODO: Đổi ngôn ngữ game ở đây
        // ChangeLanguage(index == 0 ? "vi" : "en");
    }

    void SetItemState(TMP_Text text, Image bg, bool isSelected)
    {
        if (text != null)
            text.fontStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;

        if (bg != null)
            bg.enabled = isSelected; // Chỉ hiện nền khi chọn
    }

    // Gọi từ bên ngoài nếu cần
    public void OpenDropdown()
    {
        isOpen = true;
        template.SetActive(true);
    }
}