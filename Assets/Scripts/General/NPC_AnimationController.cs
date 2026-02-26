using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AIPath), typeof(Animator))]
public class NPC_AnimationController : MonoBehaviour
{
    private AIPath aiPath;
    private Animator animator;
    private Vector3 initialScale;

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        initialScale = transform.localScale;
    }

    void Update()
    {
        Vector2 moveDir = aiPath.desiredVelocity;

        // Gui thong tin cho Animator biet co dang di hay khong
        animator.SetBool("isWalking", moveDir.sqrMagnitude > 0.01f);

        if (moveDir.sqrMagnitude < 0.01f)
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
            return;
        }

        // Logic quyet dinh uu tien huong nao
        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
        {
            // Neu di chuyen chu yeu theo chieu ngang
            animator.SetFloat("MoveX", Mathf.Sign(moveDir.x));
            animator.SetFloat("MoveY", 0);
        }
        else
        {
            // Neu di chuyen chu yeu theo chieu doc
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", Mathf.Sign(moveDir.y));
        }
    }
}