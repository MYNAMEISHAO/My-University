using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentClassroomHandle : MonoBehaviour
{
    public ClassRoomController classRoom;
    public Queue<GameObject> waitingQueue = new Queue<GameObject>();

    [Header("Settings")]
    public float wanderRadius = 3f;
    public float retryDelay = 2f;

    public void OnStudentArrive(GameObject student)
    {
        if (classRoom == null) return;
        int emptySeatIndex = classRoom.CheckEmptySeat();

        // classRoom.IsUnlocked và HasTeacher lấy từ Properties của ClassRoomController 
        // (ClassRoomController nên lấy data từ RoomManager/RoomDataManager)
        bool canEnter = (emptySeatIndex != -1) && classRoom.IsUnlocked;

        if (canEnter)
        {
            classRoom.usedSeat.Add(emptySeatIndex);
            Transform seatPos = classRoom.seatPos[emptySeatIndex];
            StartCoroutine(MoveAndSnapToTarget(student, seatPos, emptySeatIndex));
        }
        else
        {
            HandleQueueing(student);
        }
    }

    private void HandleQueueing(GameObject student)
    {
        int maxQueueSize = classRoom.waitingPos.Count;
        if (waitingQueue.Count < maxQueueSize)
        {
            waitingQueue.Enqueue(student);
            int slotIndex = waitingQueue.Count - 1;
            Vector3 targetPos = classRoom.waitingPos[slotIndex].position;
            StartCoroutine(GoToQueueAndStand(student, targetPos));
        }
        else
        {
            StartCoroutine(WanderAndRetry(student));
        }
    }

    private IEnumerator WanderAndRetry(GameObject student)
    {
        Vector3 randomPoint = (Vector3)UnityEngine.Random.insideUnitCircle * wanderRadius;
        Vector3 wanderTarget = transform.position + randomPoint;

        MoveStudentTo(student, wanderTarget);

        var aiPath = student.GetComponent<IAstarAI>();
        if (aiPath != null)
        {
            yield return null; 
            yield return new WaitUntil(() => aiPath.reachedEndOfPath && !aiPath.pathPending);
        }
        yield return new WaitForSeconds(retryDelay);
        yield return StartCoroutine(ReturnToDoorAndRetry(student));
    }
    private IEnumerator ReturnToDoorAndRetry(GameObject student)
    {
        Vector3 doorPos = transform.position;

        MoveStudentTo(student, doorPos);

        var aiPath = student.GetComponent<IAstarAI>();
        if (aiPath != null)
        {
            yield return null;
            yield return new WaitUntil(() => aiPath.reachedEndOfPath && !aiPath.pathPending);
        }

        OnStudentArrive(student);
    }
    public void OnSeatFreed(int seatIndex)
    {
        classRoom.usedSeat.Remove(seatIndex);

        if (waitingQueue.Count > 0)
        {
            GameObject nextStudent = waitingQueue.Dequeue();
            classRoom.usedSeat.Add(seatIndex);
            Transform seatPos = classRoom.seatPos[seatIndex];

            StartCoroutine(MoveAndSnapToTarget(nextStudent, seatPos, seatIndex));
            ReassignWaitingPositions();
        }
    }

    private void ReassignWaitingPositions()
    {
        if (waitingQueue.Count == 0) return;

        int index = 0;
        foreach (GameObject student in waitingQueue)
        {
            if (student == null) continue;

            if (index < classRoom.waitingPos.Count)
            {
                Vector3 targetPos = classRoom.waitingPos[index].position;
                MoveStudentTo(student, targetPos);
            }
            index++;
        }
    }


    private IEnumerator StudentLearningProcess(GameObject student, int seatIndex)
    {
        yield return new WaitForSeconds(classRoom.timeRequired);

        int earnedCoin = 0; 
        if (RoomManager.Instance != null && classRoom != null)
        {
            earnedCoin =
                RoomManager.Instance.CalculateRoomIncome(classRoom.RoomID);
            if (!classRoom.HasTeacher)
            {
                earnedCoin = Mathf.RoundToInt(earnedCoin * 0.5f);
            }

            PlayerManager.Instance.AddCoin(earnedCoin);

            var coinEffect = student.GetComponent<CoinParticleSystem>();
            if (coinEffect != null)
                coinEffect.OnFinishStudying(earnedCoin);
        }

        Debug.Log(
            $"[INCOME] Student {student.name} finished → +" +
            $"{earnedCoin} coins in room {classRoom.RoomID}"
        );


        classRoom.OnStudentLeave(seatIndex);
        OnSeatFreed(seatIndex);

        NotifyAICompletion(student);
    }

    private IEnumerator MoveAndSnapToTarget(GameObject student, Transform seatTransform, int seatIndex)
    {
        var aiBase = student.GetComponent<AIBase>();
        var aiPath = student.GetComponent<IAstarAI>();
        var spriteSwapper = student.GetComponent<SpriteSwapAnimator>();
        var animator = student.GetComponent<Animator>();

        if (aiBase == null || aiPath == null) yield break;

        PrepareForMovement(student);

        aiPath.destination = seatTransform.position;
        aiBase.SearchPath();
        aiPath.canMove = true;

        yield return null; // Chờ 1 frame để path tính toán
        yield return new WaitUntil(() => aiPath.reachedEndOfPath && !aiPath.pathPending);

        aiPath.canMove = false;
        spriteSwapper.isPaused = true;
        animator.enabled = false;

        student.transform.position = new Vector3(seatTransform.position.x, seatTransform.position.y, student.transform.position.z);

        SeatController seatController = seatTransform.GetComponent<SeatController>();
        if (seatController != null)
        {
            SpriteRenderer npcSprite = student.GetComponentInChildren<SpriteRenderer>();
            SetSpriteByDirection(npcSprite, seatController.direction, spriteSwapper.newSprites);
        }

        if (classRoom != null) classRoom.OnStudentSit(seatIndex);
        StartCoroutine(StudentLearningProcess(student, seatIndex));
    }

    private IEnumerator GoToQueueAndStand(GameObject student, Vector3 targetPos)
    {
        var aiPath = student.GetComponent<IAstarAI>();
        var aiBase = student.GetComponent<AIBase>();
        var spriteSwapper = student.GetComponent<SpriteSwapAnimator>();
        var animator = student.GetComponent<Animator>();

        if (aiBase != null && aiPath != null)
        {
            aiPath.canMove = true;
            aiPath.destination = targetPos;
            aiBase.SearchPath();

            yield return null;
            yield return new WaitUntil(() => aiPath.reachedEndOfPath && !aiPath.pathPending);

            aiPath.canMove = false;
            if (aiBase is AIBase aiScript) aiScript.isStopped = true;

            Rigidbody2D rb = student.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            student.transform.position = new Vector3(targetPos.x, targetPos.y, student.transform.position.z);

            if (spriteSwapper != null && animator != null)
            {
                spriteSwapper.isPaused = true;
                animator.enabled = false;
                SpriteRenderer npcSprite = student.GetComponentInChildren<SpriteRenderer>();
                SetSpriteByDirection(npcSprite, classRoom.queueDirection, spriteSwapper.newSprites);
            }
        }
    }

    private void NotifyAICompletion(GameObject student)
    {
        if (student == null) return;

        var aiController = student.GetComponent<AutonomousAIController>();
        var spriteSwapper = student.GetComponent<SpriteSwapAnimator>();
        var animator = student.GetComponent<Animator>();

        if (spriteSwapper != null) spriteSwapper.isPaused = false;
        if (animator != null) animator.enabled = true;
        var aiPath = student.GetComponent<IAstarAI>();
        var aiBase = student.GetComponent<AIBase>();

        if (aiPath != null)
        {
            aiPath.canMove = true;
        }

        if (aiBase != null)
        {
            aiBase.isStopped = false;
        }

        if (aiController != null)
            aiController.NotifyTaskFinished();
        else
            Debug.LogError($"Student {student.name} missing AutonomousAIController!");
    }

    private void MoveStudentTo(GameObject student, Vector3 finalPosition)
    {
        var aiPath = student.GetComponent<IAstarAI>();
        var aiBase = student.GetComponent<AIBase>();

        if (aiBase != null && aiPath != null)
        {
            aiPath.destination = finalPosition;
            aiBase.SearchPath();
            aiPath.canMove = true;
        }
    }

    private void PrepareForMovement(GameObject student)
    {
        var aiBase = student.GetComponent<AIBase>();
        var spriteSwapper = student.GetComponent<SpriteSwapAnimator>();
        var animator = student.GetComponent<Animator>();

        if (aiBase != null) aiBase.isStopped = false;
        if (animator != null) animator.enabled = true;
        if (spriteSwapper != null) spriteSwapper.isPaused = false;
    }

    private void SetSpriteByDirection(SpriteRenderer npcSprite, string direction, Sprite[] sprites)
    {
        if (npcSprite == null || sprites == null || sprites.Length == 0) return;
        int index = 1; 
        if (!string.IsNullOrEmpty(direction))
        {
            switch (direction.ToLower())
            {
                case "left": index = 4; break;
                case "right": index = 7; break;
                case "down": index = 1; break;
                case "up": index = 10; break;
            }
        }
        if (index < sprites.Length) npcSprite.sprite = sprites[index];
    }
    public void ProcessQueue()
    {
        if (waitingQueue.Count == 0) return;

        Debug.Log("Cửa đã mở/Có GV! Đang kiểm tra hàng chờ...");

        while (waitingQueue.Count > 0)
        {
            int seatIndex = classRoom.CheckEmptySeat();
            bool canEnter = (seatIndex != -1) && classRoom.IsUnlocked && classRoom.HasTeacher;

            if (!canEnter)
            {
                Debug.Log("Vẫn chưa đủ điều kiện vào (Thiếu GV hoặc Hết chỗ).");
                break;
            }

            GameObject student = waitingQueue.Dequeue();

            if (student != null)
            {
                classRoom.usedSeat.Add(seatIndex);

                Transform seatPos = classRoom.seatPos[seatIndex];
                StartCoroutine(MoveAndSnapToTarget(student, seatPos, seatIndex));
            }
        }
        ReassignWaitingPositions();
    }
}