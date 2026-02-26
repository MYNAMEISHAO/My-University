using System.Collections.Generic;
using UnityEngine;
using static NPCAppearance;

public class UniqueSprieassigner : MonoBehaviour
{
    private NPCAppearance npcAppearance;
    private static HashSet<NPCAppearance.SpriteSheet> usedSpriteSheets = new HashSet<NPCAppearance.SpriteSheet>();

    void Awake()
    {
        npcAppearance = GetComponent<NPCAppearance>();
        var allSheets = npcAppearance.PossibleAppearances;
        foreach (var sheet in allSheets)
        {
            if (!usedSpriteSheets.Contains(sheet))
            {
                usedSpriteSheets.Add(sheet);
            }
        }
    }

}
