using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUI : MonoBehaviour
{
    // Tham chiếu đến Text Component (giả sử dùng TextMeshPro)
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private Image IconImage;
    [SerializeField] private Sprite TrainingBookImage;
    [SerializeField] private Sprite InkImage;
    // Tham chiếu đến Image Component của Panel

    private const float DisplayDuration = 2.0f; // Thời gian hiển thị
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Awake()
    {
        IconImage = transform.GetChild(1).GetComponent<Image>();
        notificationText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void InitializeAndShow(int amount, string unitName)
    {
        // 1. Thiết lập nội dung Text
        string sign = (amount >= 0) ? "+" : "";
        notificationText.text = $"{sign}{amount}";

        if(unitName=="trainingbook")
        {
            IconImage.sprite = TrainingBookImage;
        }
        else if(unitName=="ink")
        {
            IconImage.sprite = InkImage;
        }
        // 2. Bắt đầu Coroutine để quản lý vòng đời
        StartCoroutine(WaitCoroutine());
    }

    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(DisplayDuration);
        GameObject.Destroy(this.gameObject);
    }
}
