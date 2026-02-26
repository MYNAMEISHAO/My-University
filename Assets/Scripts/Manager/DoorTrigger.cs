using UnityEngine;
using UnityEngine.EventSystems;

public class DoorTrigger : MonoBehaviour, IPointerDownHandler
{
    [Tooltip("Kéo GameManager (chứa script BuildingViewController) vào đây")]
    public BuildingViewController viewController;

    [Tooltip("Click cửa này là để ĐI VÀO (True) hay ĐI RA (False)?")]
    public bool isEntranceDoor; // Đặt là True cho cửa vào, False cho cửa ra

    public void OnPointerDown(PointerEventData eventData)
    {
        if (viewController == null)
        {
            Debug.LogError("Chưa kéo BuildingViewController vào!");
            return;
        }

        // Gọi hàm ToggleView trong GameManager
        viewController.ToggleView(isEntranceDoor);
    }
}