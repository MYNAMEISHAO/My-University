using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloorConfig", menuName = "Configs/FloorConfig")]
public class FloorConfig : ScriptableObject
{
    [Header("Floor Info")]
    [SerializeField] private string floorID;
    [SerializeField] private string floorName;
    [SerializeField] private int condition;

    [Header("Rooms on this Floor")]
    [SerializeField] private List<RoomConfig> roomsOnFloor;

    public string BuildingID;
    public string FloorID => floorID;
    public string FloorName => floorName;
    public int Condition => condition;

}
