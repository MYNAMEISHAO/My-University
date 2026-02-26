using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewSubjectConfig", menuName = "Configs/Config Subject")]
public class SubjectConfig : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string subjectName;
    [SerializeField] private string id;         
    [SerializeField] private string description;
    [SerializeField] private string roomID;


    [Header("Visuals")]
    [SerializeField] private Sprite image;

    [Header("Economy Logic")]
    [SerializeField] private UpgradeConfig upgradeCurve;

    public string Name => subjectName;
    public string ID => id;
    public string Description => description;
    public Sprite Image => image;
    public string RoomID => roomID;


    public float GetPriceAtLevel(int level)
    {
        return upgradeCurve.costCurve.Evaluate(level);
    }

    public float GetIncomeAtLevel(int level)
    {
        return upgradeCurve.valueCurve.Evaluate(level);
    }
}