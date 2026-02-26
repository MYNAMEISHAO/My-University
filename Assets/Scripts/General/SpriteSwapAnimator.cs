using UnityEngine;

public class SpriteSwapAnimator : MonoBehaviour
{
    public Sprite[] newSprites;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] originalSprites;
    public bool isPaused = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Debug.Log("Original Sprites Count: " + originalSprites.Length);
    }

    public void SetNewSprites(Sprite[] replacementSprites)
    {
        newSprites = replacementSprites;
    }

    private void LateUpdate()
    {
        if (isPaused) return;
        if (newSprites == null || originalSprites == null || newSprites.Length == 0 || originalSprites.Length == 0) return;
        
        Sprite current = spriteRenderer.sprite;
        int index = System.Array.IndexOf(originalSprites, current);

        if (index >= 0 && index < newSprites.Length)
        {
            spriteRenderer.sprite = newSprites[index];
        }
    }
}
