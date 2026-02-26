using System.Net;
using UnityEngine;

[DefaultExecutionOrder(-95)] // Chạy sau SaveManager nhưng trước BuildingController
public class BuildingController : MonoBehaviour
{
    [Header("DATA CONFIG")]
    [SerializeField] private BuildingConfig config;

    private BuildingData runtimeData;

    private SaveManager saveManager;
    public int currentFloors => runtimeData != null ? runtimeData.currentLevel : 0;
    public bool IsBuilt => runtimeData != null ? runtimeData.isUnlocked : false;

    private void Start()
    {
        if (config == null) return;
        runtimeData = BuildingManager.Instance.GetBuilding(config.BuildingID);
    }
    public void ConstructBuilding()
    {
        if (IsBuilt) return;
        bool success = BuildingManager.Instance.TryConstruct(config);
        if (success)
        {
            Debug.Log($"Toa nha dc mo khoa: {config.BuildingName}");
        }
    }
    
}
