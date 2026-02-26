using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToggleButtonGroup : MonoBehaviour, IPointerClickHandler
{
    [Header("Ảnh")]
    public Sprite normalSprite;
    public Sprite pressedSprite;

    [Header("Kích thước")]
    public Vector2 normalScale = new Vector2(1f, 1f);
    public Vector2 pressedScale = new Vector2(1.2f, 1.2f);

    [Header("Nút còn lại")]
    public ToggleButtonGroup otherButton; // Kéo nút kia vào đây

    private Image buttonImage;
    private RectTransform rectTransform;
    private bool isPressed = false;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        ResetToNormal();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPressed) return; // Đã nhấn rồi → không làm gì

        // Bấm nút này
        SetPressed();

        // Thu nhỏ nút kia
        if (otherButton != null)
        {
            otherButton.SetNormal();
        }
    }

    public void SetPressed()
    {
        isPressed = true;
        buttonImage.sprite = pressedSprite;
        rectTransform.localScale = pressedScale;
    }

    public void SetNormal()
    {
        isPressed = false;
        buttonImage.sprite = normalSprite;
        rectTransform.localScale = normalScale;
    }

    public void ResetToNormal()
    {
        SetNormal();
    }
}