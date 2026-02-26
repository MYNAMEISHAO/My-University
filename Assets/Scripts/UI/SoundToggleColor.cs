using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundToggleColor : MonoBehaviour
{
    [Header("Màu sắc cho BG (thanh dài)")]
    [SerializeField] private Color onColor  = new Color(0.3f, 1f, 0.3f, 1f); // XANH khi BẬT
    [SerializeField] private Color offColor = new Color(0.6f, 0.4f, 0.2f, 1f); // NÂU khi TẮT

    [Header("Tham chiếu")]
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image bgImage; // BG (thanh dài) – Image component

    private void Awake()
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        if (bgImage == null) bgImage = transform.Find("BG").GetComponent<Image>();
    }

    private void OnEnable()
    {
        toggle.onValueChanged.AddListener(UpdateBGColor);
        UpdateBGColor(toggle.isOn); // Cập nhật ngay
    }

    private void OnDisable()
    {
        toggle.onValueChanged.RemoveListener(UpdateBGColor);
    }

    private void UpdateBGColor(bool isOn)
    {
        if (bgImage != null)
        {
            bgImage.color = isOn ? onColor : offColor;
        }
    }
}