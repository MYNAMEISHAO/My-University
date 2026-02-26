using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

[System.Serializable]
public class ItemRequirement
{
    public string itemID;
    public int amount;
}

[System.Serializable]
public class UpgradeStep
{
    public int targetLevel;
    public List<ItemRequirement> requiredItems;
}

[CreateAssetMenu(fileName = "NewTeacherConfig", menuName = "Configs/Config Teacher")]
public class TeacherConfig : ScriptableObject
{
    [Header("INFO")]
    [SerializeField] protected string teacherID;
    [SerializeField] protected string teacherName;
    [SerializeField] protected string Description;
    //[SerializeField] protected string UnlockCondition;

    [Header("Visuals")]
    public GameObject prefabModel;
    public Sprite TeacherSprite;
    [SerializeField] private Sprite[] allDirectionSprites = new Sprite[4];


    [Header("Upgrade Logic")]
    [SerializeField] protected UpgradeConfig upgradeStats;

    [Header("Upgrade Steps")]
    public List<UpgradeStep> upgradeLevels;

    [Header("Initial State")]
    [SerializeField] private int initialLevel = 1;
    [SerializeField] private bool isInitiallyUnlocked = false;

    [Header("Unlock Settings")]
    [SerializeField] private int unlockFame;
    [SerializeField] private int unlockCost;

    [Header("Income Settings")]
    [SerializeField] private int baseIncome;
    public string TeacherID => teacherID;

    public string TeacherName => teacherName;
    public string TeacherDescription => Description;
    public int InitialLevel => initialLevel;
    public bool IsInitiallyUnlocked => isInitiallyUnlocked;
    public int UnlockFame => unlockFame;
    public int UnlockCost => unlockCost;
    public int BaseIncome => baseIncome;
    public UpgradeConfig UpgradeStats => upgradeStats;
    public Sprite[] AllDirectionSprites => allDirectionSprites;

    public int GetUpgradeCost(int level)
    {
        if (upgradeStats != null) return (int)upgradeStats.costCurve.Evaluate(level);
        return 0;
    }

    public float GetUpgradeValue(int level)
    {
        float curveVal = 0;
        if (upgradeStats != null)
        {
            curveVal = upgradeStats.valueCurve.Evaluate(level);
        }

        // In ra xem thằng nào đang bằng 0
        Debug.Log($"[CHECK TIỀN] GV {teacherID}: Base({baseIncome}) x Curve({curveVal}) = {baseIncome * curveVal}");
        Debug.Log($"[Check File] Tên: '{this.name}' (ID: {this.GetInstanceID()}) | Base: {baseIncome} | Ra: {curveVal}");

        if (upgradeStats != null) return baseIncome * upgradeStats.valueCurve.Evaluate(level);
        return 0;
    }

    public UpgradeStep GetRequirementForNextLevel(int currentLevel)
    {
        int nextLevel = currentLevel + 1;
        return upgradeLevels.Find(step => step.targetLevel == nextLevel);
    }
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(TeacherID))
        {
            string[] guids = AssetDatabase.FindAssets("t:TeacherConfig");
            List<int> existingIds = new List<int>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TeacherConfig config = AssetDatabase.LoadAssetAtPath<TeacherConfig>(path);
                if (config != null && config != this && !string.IsNullOrEmpty(config.teacherID)){
                    if (int.TryParse(config.teacherID, out int idNum))
                    {
                        existingIds.Add(idNum);
                    }
                }
            }

            int nextId = 1;
            if (existingIds.Count > 0)
            {
                nextId = existingIds.Max() + 1;
            }
            teacherID = nextId.ToString();
            EditorUtility.SetDirty(this);
        }
#endif
    }

}
