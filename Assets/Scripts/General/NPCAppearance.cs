using UnityEngine;
using System.Collections.Generic;
using System;

//[ExecuteInEditMode]
public class NPCAppearance : MonoBehaviour
{
    [System.Serializable]
    public class SpriteSheet
    {
        public string name;
        public Sprite[] sprites;
    }
    [Header("Appearance Settings")]
    [SerializeField] private List<SpriteSheet> possibleAppearances;
    public List<SpriteSheet> PossibleAppearances => possibleAppearances;


    [Header("Editor Placement Settings")]
    [SerializeField] private List<Rect> spawnAreas; 
    //[SerializeField] private Rect spawnArea = new Rect(-10, -5, 20, 10);

    [Tooltip("Radius to check the collision")]
    [SerializeField] private float checkRadius = 1.0f;

    [Tooltip("The maximum number of empty search")]
    [SerializeField] private int maxAttempts = 50;

    private bool hasBeenPositioned = false;

    private static HashSet<string> usedAppearances = new HashSet<string>();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetUsedAppearances()
    {
        usedAppearances.Clear();
    }

    protected virtual void Awake()
    {
        if (Application.isPlaying)
        {
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks + gameObject.GetInstanceID());
            RandomizePosition();
            RandomizeAppearance();
        }
        else if (!hasBeenPositioned)
        {
            RandomizePosition();
            hasBeenPositioned = true;
        }
    }
    protected virtual void RandomizePosition()
    {
        if (spawnAreas == null || spawnAreas.Count == 0)
        {
            Debug.LogError($"Chưa kéo 'Spawn Area Colliders' (danh sách) vào {gameObject.name}!", this);
            return;
        }

        int randomAreaIndex = UnityEngine.Random.Range(0, spawnAreas.Count);
        Rect chosenSpawnArea = spawnAreas[randomAreaIndex]; 
        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = UnityEngine.Random.Range(chosenSpawnArea.xMin, chosenSpawnArea.xMax);
            float randomY = UnityEngine.Random.Range(chosenSpawnArea.yMin, chosenSpawnArea.yMax);

            Vector2 potentialPosition = new Vector2(randomX, randomY);

            Collider2D overlap = Physics2D.OverlapCircle(potentialPosition, checkRadius);
            if (overlap == null)
            {
                transform.position = potentialPosition;
                Debug.Log($"Placed {gameObject.name} at {potentialPosition} successfully.", this);
                return;
            }
        }
        Debug.LogWarning($"Could not find a free spot for {gameObject.name} after {maxAttempts} attempts.", this);
    }
    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;

        Gizmos.color = Color.green;
        foreach (Rect area in spawnAreas)
        {
            Gizmos.DrawWireCube(area.center, area.size);
        }
    }

    private void RandomizeAppearance()
    {
        if (possibleAppearances == null || possibleAppearances.Count == 0)
        {
            Debug.LogError("Danh sach 'Possible Appearances' dang trong!", this);
            return;
        }

        //SpriteSheet randomSheet = possibleAppearances[UnityEngine.Random.Range(0, possibleAppearances.Count)];

        //SpriteSwapAnimator swapper = GetComponent<SpriteSwapAnimator>();
        //if (swapper != null)
        //{
        //    swapper.SetNewSprites(randomSheet.sprites);
        //}

        if (usedAppearances.Count >= possibleAppearances.Count)
            usedAppearances.Clear();

        List<SpriteSheet> unused = new List<SpriteSheet>();
        foreach (var sheet in possibleAppearances)
        {
            if (!usedAppearances.Contains(sheet.name))
            { 
                unused.Add(sheet);
            }
        }
        SpriteSheet chosenSheet;
        if (unused.Count > 0)
        {
            chosenSheet = unused[UnityEngine.Random.Range(0, unused.Count)];
        }
        else
        {
            chosenSheet = possibleAppearances[UnityEngine.Random.Range(0, possibleAppearances.Count)];
        }
        usedAppearances.Add(chosenSheet.name);
        SpriteSwapAnimator swapper = GetComponent<SpriteSwapAnimator>();
        if (swapper != null)
        {
            swapper.SetNewSprites(chosenSheet.sprites);
        }
    }
}
