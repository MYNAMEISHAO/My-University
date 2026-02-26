using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRoomConfig", menuName = "Configs/Config Room")]
public class RoomConfig : ScriptableObject
{
    [Header("INFO")]
    [SerializeField] private string roomID;
    [SerializeField] protected string roomName;
    [TextArea(3, 5)][SerializeField] private string description;
    [SerializeField] private int maxLevel;
    [SerializeField] private int maxSeat;
    [SerializeField] private int maxSubject;

    [Header("Unlock Rules")]
    [SerializeField] private int unlockCost;      
    [SerializeField] private int unlockCondition;

    [Header("Upgrade Logic (Stats per Level)")]
    [SerializeField] private UpgradeConfig upgradeCurve;
    [SerializeField] private UpgradeConfig capacityCurve;

    [Header("Default Setup")]
    public string DefaultTeacherID;
    public string RoomID => roomID;
    public string RoomName => roomName;
    public string Description => description;
    public int UnlockCost => unlockCost;
    public int UnlockCondition => unlockCondition;
    public int MaxLevel => maxLevel;
    public int MaxSeat => maxSeat;
    public int MaxSubject => maxSubject;


    public int GetUpgradeCost(int level) => Mathf.CeilToInt(upgradeCurve.costCurve.Evaluate(level));
    public int GetIncome(int level) => Mathf.CeilToInt(upgradeCurve.valueCurve.Evaluate(level));
    public int GetCapacityCost(int level) => Mathf.CeilToInt(capacityCurve.costCurve.Evaluate(level));
    public int GetCapacity(int level) => Mathf.CeilToInt(capacityCurve.valueCurve.Evaluate(level));
    //public int GetUpgradeCost(int level) => (int)upgradeCurve.costCurve.Evaluate(level);
    //public int GetIncome(int level) => (int)upgradeCurve.valueCurve.Evaluate(level);
    //public int GetCapacityCost(int level) => (int)capacityCurve.costCurve.Evaluate(level);
    //public int GetCapacity(int level) => (int)capacityCurve.valueCurve.Evaluate(level);
    
}