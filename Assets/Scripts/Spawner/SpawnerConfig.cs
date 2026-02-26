using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerConfig", menuName = "Configs/SpawnerConfig")]
public class SpawnerConfig : ScriptableObject
{
    public string spawnerID;
    public GameObject npcPrefab;
    public int maxLevel = 10;
    public UpgradeConfig upgradeConfig;
}
