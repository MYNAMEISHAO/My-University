using UnityEngine;

public class ItemSlotButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int ItemIndex; // Chỉ số của item trong danh sách item
    private PointShopPanelUI uiController;
    void Start()
    {
        uiController = GetComponentInParent<PointShopPanelUI>();

    }

    // Update is called once per frame

    public void OnItemSlotClick()
    {
               // ⭐ Gọi hàm trong UI chính để hiển thị chi tiết
        if(uiController != null)
        {
            uiController.OnItemClick(this);
        }
    }
}
