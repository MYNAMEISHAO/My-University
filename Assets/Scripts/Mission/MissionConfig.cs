using UnityEngine;

[CreateAssetMenu(fileName = "MissionConfig", menuName = "Scriptable Objects/MissionConfig")]
public class MissionConfig : ScriptableObject
{
    public int id;
    public string missionName;
    public string description;

    public string objective;
    public int objectiveAmount;
    public string reward;
    public int rewardAmount;

}
