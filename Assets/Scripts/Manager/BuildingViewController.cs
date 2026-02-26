using UnityEngine;

public class BuildingViewController : MonoBehaviour
{
    [Header("Map Objects")]
    public GameObject ngoaiToaHoc;
    public GameObject trongToaHoc;

    // Không cần biến Camera hay Vị trí Camera nữa

    private bool isInside = false;

    // Hàm này sẽ được gọi bởi script Click (DoorTrigger)
    public void ToggleView(bool enterBuilding)
    {
        isInside = enterBuilding;
        if (isInside)
        {
            // Vào trong: Ẩn map ngoài, hiện map trong
            SetVisibility(ngoaiToaHoc, false);
            SetVisibility(trongToaHoc, true);
        }
        else
        {
            // Ra ngoài: Hiện map ngoài, ẩn map trong
            SetVisibility(ngoaiToaHoc, true);
            SetVisibility(trongToaHoc, false);
        }
    }

    // Hàm đệ quy để Bật/Tắt tất cả SpriteRenderer
    private void SetVisibility(GameObject parent, bool isVisible)
    {
        if (parent == null) return;

        // 1. Tắt/Bật chính nó
        SpriteRenderer sr = parent.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = isVisible;
        }

        // (Tùy chọn) Bạn có thể muốn tắt/bật cả Collider
        Collider2D col = parent.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = isVisible;
        }

        // 2. Tắt/Bật tất cả con của nó
        foreach (Transform child in parent.transform)
        {
            SetVisibility(child.gameObject, isVisible); // Lặp lại cho từng object con
        }
    }

    // Khởi tạo ban đầu
    void Start()
    {
        // Mặc định, cho hiện bên ngoài và ẩn bên trong
        ToggleView(false);
    }
}