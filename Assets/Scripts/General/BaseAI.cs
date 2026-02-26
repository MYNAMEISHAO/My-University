using UnityEngine;
using Rigidbody2D = UnityEngine.Rigidbody2D;

public abstract class BaseAI : MonoBehaviour
{
    [SerializeField] protected NPCStats NPCStats = new NPCStats();

    protected Animator animator;
    protected UnityEngine.Rigidbody2D rb;


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    protected virtual void Start()
    {
        StartAI();
    }

    protected abstract void StartAI();

    protected virtual void Update()
    {
        //Logic di chuyen chung
    }

    protected void UpdateAnimation(Vector2 direction)
    {
        bool isMoving = direction.magnitude > NPCStats.StoppingDistance;

        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            animator.SetFloat("MoveX", direction.x);
            animator.SetFloat("MoveY", direction.y);
        }
    }

    protected void MoveCharacter(Vector2 direction)
    {
        #pragma warning disable CS0618
        rb.velocity = direction * NPCStats.MoveSpeed;
        #pragma warning restore CS0618

        //((UnityEngine.Rigidbody2D)rb).velocity = direction * NPCStats.MoveSpeed;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collison)
    {

    }
}
