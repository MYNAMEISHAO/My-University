using UnityEngine;
using static FlowManager;

public class EntityManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static EntityManager Instance;
    //[SerializeField] private List<GameObject> list;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        

    }

   
}
