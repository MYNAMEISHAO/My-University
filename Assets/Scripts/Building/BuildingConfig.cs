using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "NewBuildingConfig", menuName = "Configs/Config Building")]

public class BuildingConfig : ScriptableObject
{
    [Header("INFO")]
    [SerializeField] private string buildingID;
    [SerializeField] protected string buildingName;
    [SerializeField] private string description;

    [SerializeField] private int condition;

    public string BuildingID => buildingID;
    public string BuildingName => buildingName;
    public string Description => description;
    public int Condition => condition;
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(buildingID))
        {
            string namePrefix = (this.name.Length > 8) ? this.name.Substring(0, 8) : this.name;
            buildingID = namePrefix.ToUpper() + "_BLD_" + Guid.NewGuid().ToString().Substring(0, 8);
        }
    }

}
