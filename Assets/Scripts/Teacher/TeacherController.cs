using System.Collections;
using UnityEngine;
public class TeacherController : MonoBehaviour
{
    private string myTeacherID;

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask seatLayer;

    public TeacherConfig Config => !string.IsNullOrEmpty(myTeacherID) ? TeacherManager.Instance.GetTeacherConfig(myTeacherID) : null;
    public bool IsUnlocked => !string.IsNullOrEmpty(myTeacherID) && TeacherManager.Instance.IsUnlocked(myTeacherID);
    public int CurrentLevel => TeacherManager.Instance.GetLevel(myTeacherID);

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(string teacherID)
    {
        myTeacherID = teacherID;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(true);
        StartCoroutine(WaitAndRefresh());
    }
    private IEnumerator WaitAndRefresh()
    {
        yield return new WaitForSeconds(0.2f);
        RefreshVisuals();
    }

    private void OnEnable()
    {
        GameEvents.OnTeacherUnlocked += OnTeacherChanged;
        GameEvents.OnTeacherLevelChanged += OnTeacherLevelChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnTeacherUnlocked -= OnTeacherChanged;
        GameEvents.OnTeacherLevelChanged -= OnTeacherLevelChanged;
    }

    private void OnTeacherChanged(string teacherID)
    {
        if (teacherID == myTeacherID) RefreshVisuals();
    }

    private void OnTeacherLevelChanged(string teacherID, int level)
    {
        if (teacherID == myTeacherID) RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        UpdateVisuals();
        UpdateActiveState();
    }
    private void UpdateActiveState()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = IsUnlocked ? Color.white : new Color(0f, 0f, 0f, 0.5f); ;
    }

    public void UpdateVisuals()
    {
        if (Config == null || spriteRenderer == null || Config.AllDirectionSprites == null || Config.AllDirectionSprites.Length < 4)
        {
            Debug.LogError($"Teacher: {gameObject.name} thieu Config hoac Sprite Array chua du 4 phan tu.");
            return;
        }

        string direction = "";
        SeatController foundSeatController = null;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 5.5f, seatLayer);
        foreach (var hitCollider in hitColliders)
        {
            foundSeatController = hitCollider.GetComponent<SeatController>();
            if (foundSeatController != null)
            {
                direction = foundSeatController.direction;
                break;
            }
        }

        Sprite newSprite = Config.AllDirectionSprites[0];
        switch (direction)
        {
            case "left": newSprite = Config.AllDirectionSprites[0]; break;
            case "right": newSprite = Config.AllDirectionSprites[1]; break;
            case "up": newSprite = Config.AllDirectionSprites[2]; break;
            case "down": newSprite = Config.AllDirectionSprites[3]; break;
        }
        spriteRenderer.sprite = newSprite;
    }

    private void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        if (!IsUnlocked)
        {
            Debug.Log("Giao vien nay chua duoc mo khoa!");
            return;
        }


        if (UIManager.Instance != null)
            UIManager.Instance.ShowDetailTeacherPanel(Config);
    }
}