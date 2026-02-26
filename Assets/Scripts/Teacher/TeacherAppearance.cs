using System.Collections;
using UnityEngine;

public class TeacherAppearance : MonoBehaviour
{
    [Header("Teacher Placement Settings")]
    [SerializeField] private Transform teacherDeskSpot;

    public void SetDeskPosition(Transform spot)
    {
        teacherDeskSpot = spot;
        SnapToPosition();
    }
    private void OnEnable() 
    {
        if (teacherDeskSpot != null)
        {
            SnapToPosition();
        }
    }

    private void SnapToPosition()
    {
        if (teacherDeskSpot != null)
        {
            transform.position = teacherDeskSpot.position;
            transform.rotation = Quaternion.identity;
        }
    }
    //private void CheckRoomAndPosition()
    //{
    //    ClassRoomController roomCtrl = GetComponentInParent<ClassRoomController>();

    //    if (roomCtrl != null)
    //    {
    //        if (!roomCtrl.IsUnlocked)
    //        {
    //            gameObject.SetActive(false);
    //            return; 
    //        }
    //    }

    //    SetFixedPosition();
    //}

    //private void SetFixedPosition()
    //{
    //    if (teacherDeskSpot != null)
    //    {
    //        transform.position = teacherDeskSpot.position;
    //        Debug.Log($"{gameObject.name} dung tren buc giang: {teacherDeskSpot.name}", this);
    //    }
    //    else
    //    {
    //        Debug.LogError($"TeacherAppearance: Chua gan vi tri 'teacherDeskSpot' cho {gameObject.name}!", this);
    //    }
    //}

}
