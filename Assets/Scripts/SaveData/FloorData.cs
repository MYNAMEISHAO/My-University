using UnityEngine;
[System.Serializable]
public class FloorData
{
    public string floorConfigID;
    public bool isUnlocked;
    public int currentLevel;

    public FloorData(string floorConfigID, bool isUnlocked, int currentLevel)
    {
        this.floorConfigID = floorConfigID;
        this.isUnlocked = isUnlocked;
        this.currentLevel = currentLevel;
    }
}
