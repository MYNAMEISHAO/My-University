using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    private List<BuildingData> buildings;
    private Dictionary<string, BuildingData> dict = new Dictionary<string, BuildingData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Initialize(List<BuildingData> data)
    {
        Debug.Log("--- [BuildingManager] START INITIALIZE ---");

        buildings = data ?? new List<BuildingData>();

        dict.Clear();
        foreach (var b in buildings)
        {
            string validID = !string.IsNullOrEmpty(b.buildingConfigID) ? b.buildingConfigID : "";
            if (!string.IsNullOrEmpty(validID))
            {
                dict[validID] = b;
            }
        }

        CheckForNewConfigs();

        Debug.Log($"--- [BuildingManager] DONE. Tổng số Building: {buildings.Count}");
    }

    public static List<BuildingData> GenerateDefaultBuildingData()
    {
        List<BuildingData> defaultBuildings = new List<BuildingData>();
        var configs = Resources.LoadAll<BuildingConfig>("Buildings");
        foreach (var config in configs)
        {
            string finalID = config.BuildingID;
            if (string.IsNullOrEmpty(finalID))
            {
                finalID = config.name;
            }
            var newData = new BuildingData(
                finalID,
                false,
                0
            );
            defaultBuildings.Add(newData);
        }
        //Debug.Log($"[BuildingManager] Tạo dữ liệu building mặc định, tổng số building: {defaultBuildings.Count}");
        return defaultBuildings;
    }

    private void CheckForNewConfigs()
    {
        var configs = Resources.LoadAll<BuildingConfig>("Buildings");
        bool hasNewData = false;

        foreach (var config in configs)
        {
            string finalID = config.BuildingID;
            if (string.IsNullOrEmpty(finalID))
            {
                finalID = config.name;
            }

            if (!dict.ContainsKey(finalID))
            {
                var newData = new BuildingData(finalID, false, 0);

                buildings.Add(newData);
                dict[finalID] = newData;
                hasNewData = true;

            }
        }

        if (hasNewData)
        {
            SaveManager.Instance.SaveGame();
        }
    }

    public List<BuildingConfig> GetAllBuildingConfigs()
    {
        var configs = Resources.LoadAll<BuildingConfig>("Buildings");
        List<BuildingConfig> listBuildings = new List<BuildingConfig>(configs);

        return listBuildings;
    }


    public BuildingData GetBuilding(string id)
    {
        dict.TryGetValue(id, out var data);
        return data;
    }

    public bool TryConstruct(BuildingConfig config)
    {
        string targetID = !string.IsNullOrEmpty(config.BuildingID) ? config.BuildingID : config.name;

        var data = GetBuilding(targetID);

        if (data == null || data.isUnlocked) return false;

        int cost = config.Condition * 10;

        if (!PlayerManager.Instance.SpendCoin(cost))
        {
            Debug.Log("Không đủ tiền xây nhà!");
            return false;
        }

        data.isUnlocked = true;
        data.currentLevel = 1;

        SaveManager.Instance.SaveGame();
        return true;
    }
}