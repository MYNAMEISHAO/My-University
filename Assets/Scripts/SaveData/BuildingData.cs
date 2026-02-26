using System;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingData
{
    public string buildingConfigID;
    public bool isUnlocked;
    public int currentLevel;

    public BuildingData() { }

    public BuildingData(string id, bool unlocked, int level)
    {
        this.buildingConfigID = id;
        this.isUnlocked = unlocked;
        this.currentLevel = level;
    }
}

