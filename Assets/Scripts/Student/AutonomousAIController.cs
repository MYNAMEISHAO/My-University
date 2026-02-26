﻿using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System;

public enum AIState { MovingOutside, MovingInside, PerformingTask, MovingToHallway, Idle, Leaving}

[System.Serializable]
public class FloorMapping
{
    public FloorConfig floorConfig;
    public List<Transform> roomPoints;
}

[System.Serializable]
public class BuildingMapping
{
    public BuildingConfig config;
    public Transform outsideDoorLocation;
    public Transform hallwayLocation;
    public List<FloorMapping> floors;
    public string BuildingName => config != null ? config.BuildingName : "Unknown Building";
}

[RequireComponent(typeof(IAstarAI))]
public class AutonomousAIController : VersionedMonoBehaviour
{
    [Header("1. Lộ trình chính")]
    public List<BuildingMapping> majorDestinations;

    [Header("2. Cài đặt hành vi")]
    public float taskDuration = 2f;

    [Header("3. Tham chiếu")]
    public SpriteRenderer npcSprite;



    private AIBase aiBase;
    private IAstarAI aiPath;
    public AIState CurrentState { get; private set; }
    private int majorDestinationIndex = 0;
    private int currentFloorIndex = 0;
    private int currentRoomIndex = 0;

    private Transform exitPoint;
    private bool arrivalHandled = false;
    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        aiBase = GetComponent<AIBase>();
        aiPath = GetComponent<IAstarAI>();
    }

    private void OnEnable()
    {
        if (isInitialized && aiPath != null && aiPath.destination != Vector3.positiveInfinity)
        {
            aiPath.canMove = true;
            aiBase.isStopped = false;
            aiBase.SearchPath(); 
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (aiPath != null) aiPath.canMove = false;
    }

    private void OnDestroy()
    {
        if (SpawnerManager.Instance != null)
        {
            SpawnerManager.Instance.UnregisterStudent(this);
        }
    }
    public void Initialize(List<BuildingMapping> mapData, Transform exitTransform)
    {
        if (isInitialized) return;
        this.majorDestinations = mapData; 
        this.exitPoint = exitTransform;
        if (majorDestinations != null && majorDestinations.Count > 0)
        {
            isInitialized = true;
            majorDestinationIndex = 0;
            currentFloorIndex = 0;
            currentRoomIndex = 0;
            PrepareNextMajorJourney(true);
        }
        else
        {
            Debug.LogWarning("NPC Init với Map Data rỗng!");
            GoHome(); 
        }
    }
    private void Update()
    {
        if (aiPath == null || aiPath.pathPending) return;

        if (aiPath.reachedEndOfPath && !arrivalHandled)
        {
            arrivalHandled = true;
            StartCoroutine(ProcessArrival());
        }
    }
    private void SetDestination(Vector3 pos)
    {
        arrivalHandled = false;
        aiPath.destination = pos;
        aiBase.SearchPath();
        aiPath.canMove = true;
    }

    private IEnumerator ProcessArrival()
    {
        aiPath.canMove = false;

        if (CurrentState == AIState.Leaving)
        {
            Debug.Log("Đã về đến cổng. Tạm biệt!");
            yield return StartCoroutine(FadeAndDestroy());
            yield break;
        }
        // TH1: Đến hành lang (Kết thúc)
        if (CurrentState == AIState.MovingToHallway)
        {
            CheckNextSchedule();
            yield break;
        }

        // TH2: Đến cửa tòa nhà
        if (CurrentState == AIState.MovingOutside)
        {
            CurrentState = AIState.MovingInside;
            currentFloorIndex = 0;
            currentRoomIndex = 0;

            var building = majorDestinations[majorDestinationIndex];
            if (building.floors != null && building.floors.Count > 0 && building.floors[0].roomPoints.Count > 0)
            {
                var firstRoom = building.floors[currentFloorIndex].roomPoints[currentRoomIndex];
                SetDestination(GetPositionWithOffset(firstRoom.position));
            }
            else
            {
                GoToHallway(); 
            }
            yield break;
        }

        // TH3: Đến cửa phòng học
        if (CurrentState == AIState.MovingInside)
        {
            CurrentState = AIState.PerformingTask;

            var building = majorDestinations[majorDestinationIndex];
            Transform door = building.floors[currentFloorIndex].roomPoints[currentRoomIndex];

            StudentClassroomHandle handle =
                door.GetComponent<StudentClassroomHandle>() ??
                door.GetComponentInParent<StudentClassroomHandle>();

            if (handle != null)
            {
                Debug.Log($"Đến lớp {door.name}, xin ghế.");
                handle.OnStudentArrive(gameObject);
            }
            else
            {
                Debug.Log($"Điểm {door.name} không phải lớp, bỏ qua.");
                yield return new WaitForSeconds(taskDuration);
                StartCoroutine(ProcessNextRoom());
            }
        }
    }

    private void CheckNextSchedule()
    {
        if (majorDestinationIndex >= majorDestinations.Count-1)
        {
            if (exitPoint == null)
            {
                Debug.LogWarning("Không tìm thấy cổng ra (ExitPoint)! Xóa NPC luôn.");
                Destroy(gameObject);
                return;
            }

            Debug.Log("Đã học xong tất cả các môn. Đi về cổng!");
            CurrentState = AIState.Leaving;

            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 1.0f;
            SetDestination(exitPoint.position + (Vector3)randomOffset);
        }
        else
        {
            PrepareNextMajorJourney(false);
        }
    }

    private void GoHome()
    {
        if (exitPoint == null)
        {
            Destroy(gameObject);
            return;
        }
        CurrentState = AIState.Leaving;
        SetDestination(GetPositionWithOffset(exitPoint.position));
    }
    private IEnumerator FadeAndDestroy()
    {
        if (npcSprite != null)
        {
            float duration = 1f;
            float timer = 0;
            Color startColor = npcSprite.color;
            while(timer < duration)
            {
                timer += Time.deltaTime;
                float a = Mathf.Lerp(1f, 0f, timer / duration);
                npcSprite.color = new Color(startColor.r, startColor.g, startColor.b, a);
                yield return null;
            }
        }
        Destroy(gameObject);
    }

    private IEnumerator ProcessNextRoom()
    {
        yield return null;

        if (MoveToNextRoom())
        {
            CurrentState = AIState.MovingInside;
        }
        else
        {
            GoHome();
        }
    }

    private bool MoveToNextRoom()
    {
        var building = majorDestinations[majorDestinationIndex];

        currentRoomIndex++;
        if (currentRoomIndex >= building.floors[currentFloorIndex].roomPoints.Count)
        {
            currentFloorIndex++;
            currentRoomIndex = 0;
        }

        if (currentFloorIndex >= building.floors.Count)
            return false;

        Transform nextRoom = building.floors[currentFloorIndex].roomPoints[currentRoomIndex];

        if (nextRoom == null) return MoveToNextRoom();

        StudentClassroomHandle handle =
            nextRoom.GetComponent<StudentClassroomHandle>() ??
            nextRoom.GetComponentInParent<StudentClassroomHandle>();

        if (handle != null && handle.classRoom != null)
        {
            if (!handle.classRoom.IsUnlocked)
            {
                return false;
            }
        }
        SetDestination(nextRoom.position);
        return true;
    }
    public void PrepareNextMajorJourney(bool isFirstJourney = false)
    {
        if (!isFirstJourney)  majorDestinationIndex++;

        if (majorDestinationIndex < majorDestinations.Count)
        {
            var building = majorDestinations[majorDestinationIndex];
            if (building.outsideDoorLocation != null)
            {
                ShowNPC(true);
                CurrentState = AIState.MovingOutside;
                SetDestination(GetPositionWithOffset(building.outsideDoorLocation.position));
            }
        }
        else
        {
            GoHome();
        }
    }

    private Vector3 GetPositionWithOffset(Vector3 originalPos)
    {
        Vector2 offset = UnityEngine.Random.insideUnitCircle * 0.5f;
        return originalPos + new Vector3(offset.x, offset.y, 0);
    }

    public void NotifyTaskFinished()
    {
        StartCoroutine(ProcessNextRoom());
    }
    private void GoToHallway()
    {
        var building = majorDestinations[majorDestinationIndex];

        if (building.hallwayLocation == null)
        {
            Debug.LogWarning($"Tòa {building.BuildingName} chưa gán hành lang!");
            CurrentState = AIState.Idle;
            return;
        }

        Debug.Log($"Hết lớp trong {building.BuildingName}, ra hành lang.");
        CurrentState = AIState.MovingToHallway;
        SetDestination(building.hallwayLocation.position);
    }

    void ShowNPC(bool isVisible)
    {
        if (npcSprite != null) npcSprite.enabled = isVisible;
    }
}