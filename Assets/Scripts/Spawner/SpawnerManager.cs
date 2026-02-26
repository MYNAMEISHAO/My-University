using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-95)]
public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance { get; private set; }
    [Header("1. Data & Configs")]
    public SpawnerData data;             // Chứa currentLevel
    public SpawnerConfig spawnerConfig;  // Chứa Prefab
    public UpgradeConfig upgradeConfig;  // Config chứa Curve tính toán

    [Header("2. Scene References")]
    public List<BuildingMapping> schoolMapData;
    public Transform schoolExitPoint;

    [Header("3. Spawn Logic (Rect)")]
    public List<Rect> spawnAreas;
    public LayerMask obstacleLayer;
    public float checkRadius = 0.5f;
    public int maxAttempts = 10;

    [Header("4. Limits")]
    public int maxStudentCount = 20;

    private List<AutonomousAIController> activeStudents = new List<AutonomousAIController>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (data == null) data = GenerateDefaultSpawnData();
    }

    void Start()
    {
        var existingStudents = FindObjectsOfType<AutonomousAIController>();
        foreach (var student in existingStudents)
        {
            student.Initialize(schoolMapData, schoolExitPoint);
        }
    }
    public void Initialize(SpawnerData savedData)
    {
        this.data = savedData;
    }

    public static SpawnerData GenerateDefaultSpawnData()
    {
        return new SpawnerData
        {
            spawnerID = "Main_Student_Spawner",
            currentLevel = 1,
            spawnTimer = 0f
        };
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (spawnAreas != null)
        {
            foreach (Rect area in spawnAreas)
                Gizmos.DrawWireCube(new Vector3(area.center.x, area.center.y, 0), new Vector3(area.width, area.height, 1));
        }
    }

    void Update()
    {
        if (spawnerConfig == null || data == null || upgradeConfig == null) return;

        if (activeStudents.Count >= maxStudentCount) return;

        int spawnRatePerMinute = (int)Mathf.Floor(upgradeConfig.valueCurve.Evaluate(data.currentLevel));
        float interval = spawnRatePerMinute > 0 ? 60f / spawnRatePerMinute : float.MaxValue;

        data.spawnTimer += Time.deltaTime;
        if (data.spawnTimer >= interval)
        {
            data.spawnTimer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        if (spawnerConfig.npcPrefab == null) return;

        Vector3 spawnPos = GetRandomPositionInRect();

        GameObject npcObj = Instantiate(spawnerConfig.npcPrefab, spawnPos, Quaternion.identity);

        var aiController = npcObj.GetComponent<AutonomousAIController>();
        if (aiController != null)
        {
            RegisterStudent(aiController);
            aiController.Initialize(schoolMapData, schoolExitPoint);
        }
    }

    public void RegisterStudent(AutonomousAIController student)
    {
        if (!activeStudents.Contains(student))
        {
            activeStudents.Add(student);
        }
    }

    public void UnregisterStudent(AutonomousAIController student)
    {
        if (activeStudents.Contains(student))
        {
            activeStudents.Remove(student);
        }
    }
    Vector3 GetRandomPositionInRect()
    {
        if (spawnAreas == null || spawnAreas.Count == 0) return transform.position;

        int randomAreaIndex = UnityEngine.Random.Range(0, spawnAreas.Count);
        Rect chosenArea = spawnAreas[randomAreaIndex];

        for (int i = 0; i < maxAttempts; i++)
        {
            float rx = UnityEngine.Random.Range(chosenArea.xMin, chosenArea.xMax);
            float ry = UnityEngine.Random.Range(chosenArea.yMin, chosenArea.yMax);
            Vector2 pos = new Vector2(rx, ry);

            if (Physics2D.OverlapCircle(pos, checkRadius, obstacleLayer) == null)
            {
                return pos;
            }
        }

        return chosenArea.center;
    }

    public SpawnerData GetSpawnerData()
    {
        return data;
    }

    public SpawnerConfig GetSpawnerConfig()
    {
        return spawnerConfig;
    }

    public void UpgradeSpawnerData()
    {
        if(data.currentLevel < spawnerConfig.maxLevel)
        {
            data.currentLevel += 1;
            SaveManager.Instance.SaveGame();
            GameEvents.OnSpawnerLevelChanged?.Invoke(data.currentLevel);
        }
    }
}