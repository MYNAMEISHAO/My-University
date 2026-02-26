using UnityEngine;

public class SeatController : MonoBehaviour
{
    [SerializeField] public string direction;
    float timeRequired;
    private ClassRoomController classRoomController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        classRoomController = GetComponentInParent<ClassRoomController>();

        if (classRoomController == null)
        {
            Debug.LogError($"SeatController tren {gameObject.name}: Khong tim thay ClassRoomController tren doi tuong cha nao!", this);
        }
    }
    private void Start()
    {
        SetTime();
        
    }

    void Update()
    {
        if (timeRequired > 0)
        {
            timeRequired -= Time.deltaTime;
            if (timeRequired <= 0)
            {
                SetTime();

            }

        }
    }
    public void SetTime()
    {
        if (classRoomController != null)
        {
            timeRequired = classRoomController.timeRequired;
        }
        else
        {
            timeRequired = 0f;
        }
    }

    
}
