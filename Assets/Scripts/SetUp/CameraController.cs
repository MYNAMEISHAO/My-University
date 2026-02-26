using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float panSpeed = 0.5f; // Tăng lên một chút để dễ cảm nhận
    [SerializeField] private float dragThreshold = 5f;

    [Header("MOVEMENT LIMITS")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private bool isDragging = false;
    private Vector2 startDragPosition;
    private bool canDragCamera = false;

    private void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!IsPointerOverUI())
            {
                canDragCamera = true;
                startDragPosition = Mouse.current.position.ReadValue();
            }
            else
            {
                canDragCamera = false;
            }
            isDragging = false;
        }

        if (Mouse.current.leftButton.isPressed && canDragCamera)
        {
            Vector2 currentPosition = Mouse.current.position.ReadValue();

            if (!isDragging)
            {
                if (Vector2.Distance(startDragPosition, currentPosition) > dragThreshold)
                {
                    isDragging = true;
                }
            }

            if (isDragging)
            {
                Vector2 delta = Mouse.current.delta.ReadValue();

                // Tính toán vị trí mới
                Vector3 panDirection = new Vector3(-delta.x, -delta.y, 0) * panSpeed;
                Vector3 targetPosition = transform.position + panDirection;

                // Giới hạn vị trí
                targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

                transform.position = targetPosition;
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
            canDragCamera = false;
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }
}