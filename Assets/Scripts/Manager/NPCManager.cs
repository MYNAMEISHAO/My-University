using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;


public class NPCManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static NPCManager instance;

    private List<GameObject> hiddenNPCs = new List<GameObject>();
    private Transform entranceTransform;

    [Header("Reappearing Settings")]
    [SerializeField] private float minReappearTime = 5f;
    [SerializeField] private float maxReappearTime = 15f;
    private float reappearTimer;

    [Header("Appearance Settings")]
    [SerializeField] private List<NPCAppearance.SpriteSheet> allPossibleAppearances;

    private List<NPCAppearance.SpriteSheet> usedAppearances = new List<NPCAppearance.SpriteSheet>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ResetReappearTimer();
    }

    private void Update()
    {
        if (hiddenNPCs.Count > 0)
        {
            reappearTimer -= Time.deltaTime;
            if (reappearTimer <= 0)
            {
                ReappearRandomNPC();
                ResetReappearTimer();
            }
        }
    }

    public NPCAppearance.SpriteSheet GetUniqueAppearance()
    {
        var availableAppearances = allPossibleAppearances.Except(usedAppearances).ToList();

        NPCAppearance.SpriteSheet chosenAppearance;

        if (availableAppearances.Count > 0)
        {
            chosenAppearance = availableAppearances[UnityEngine.Random.Range(0, availableAppearances.Count)];
        }
        else
        {
            Debug.LogWarning("All unique appearances are in use. Allowing duplicates.");
            chosenAppearance = allPossibleAppearances[UnityEngine.Random.Range(0, allPossibleAppearances.Count)];
        }
        usedAppearances.Add(chosenAppearance);
        return chosenAppearance;
    }

    public void ReleaseAppearance(NPCAppearance.SpriteSheet appearanceToRealease)
    {
        if (appearanceToRealease != null && usedAppearances.Contains(appearanceToRealease))
        {
            usedAppearances.Remove(appearanceToRealease);
        }
    }

    public void HideNPC(GameObject npc, Transform entrance)
    {
        if (!hiddenNPCs.Contains(npc))
        {
            npc.SetActive(false);
            hiddenNPCs.Add(npc);
            this.entranceTransform = entrance; 
        }
    }

    private void ReappearRandomNPC()
    {
        if (hiddenNPCs.Count == 0 || entranceTransform == null) return;

        int randomIndex = UnityEngine.Random.Range(0, hiddenNPCs.Count);
        GameObject npcToReappear = hiddenNPCs[randomIndex];
        hiddenNPCs.RemoveAt(randomIndex);

        npcToReappear.transform.position = entranceTransform.position;
        npcToReappear.SetActive(true);
    }

    private void ResetReappearTimer()
    {
        reappearTimer = UnityEngine.Random.Range(minReappearTime, maxReappearTime);
    }
}
