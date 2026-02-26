using UnityEngine;
using UnityEngine.UI;

public class SubjectSlotButton : MonoBehaviour
{
    public int SubjectIndex { get; private set; }
    private UpgradeClassroomUI uiController;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject highlightEffect;

    public void Setup(int index, UpgradeClassroomUI controller, SubjectConfig config)
    {
        SubjectIndex = index;
        uiController = controller;

        if (iconImage != null && config != null)
        {
            iconImage.sprite = config.Image;
            iconImage.preserveAspect = true;
            // Đảm bảo icon không chặn click chuột
            iconImage.raycastTarget = false;
        }

        Button btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(HandleClick);
    }

    public void SetHighlight(bool isActive)
    {
        if (highlightEffect != null) highlightEffect.SetActive(isActive);
    }

    private void HandleClick()
    {
        uiController?.OnSubjectSlotClicked(SubjectIndex);
    }
}