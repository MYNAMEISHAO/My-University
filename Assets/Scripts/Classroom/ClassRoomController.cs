using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClassRoomController : MonoBehaviour
{
    [Header("DATA CONFIG")]
    [SerializeField] private RoomConfig config;

    private RoomData roomData => RoomManager.Instance.GetRoom(config.RoomID);


    [Header("Teacher Setup")]
    public Transform teacherSpawnPoint;
    private GameObject currentTeacherObject;

    [Header("Seats & Tables")]
    public List<Transform> seatPos = new List<Transform>();
    public List<Transform> TablePos = new List<Transform>();
    public List<GameObject> bookList = new List<GameObject>();
    private List<int> sittingList = new List<int>();
    public List<int> usedSeat = new List<int>();

    [Header("Waiting Points")]
    public List<Transform> waitingPos = new List<Transform>();
    private List<bool> waitingSpotStates = new List<bool>();
    public string queueDirection = "";

    [Header("Visuals")]
    [SerializeField] private GameObject unlockSignObj;
    [SerializeField] private ParticleSystem unlockEffect;

    public float timeRequired;

    public int CurrentLevel => roomData != null ? roomData.currentLevel : 0;
    public int CurrentCapacity => roomData != null ? roomData.SeatCount : 0;
    public bool IsUnlocked => roomData != null && roomData.isUnlocked;
    public bool HasTeacher => roomData != null && !string.IsNullOrEmpty(roomData.currentTeacherID);
    public string RoomID => config != null ? config.RoomID : "";

    private void Start()
    {
        if (config == null) return;

        InitializeSeats();
        UpdateRoomVisuals();
        TrySpawnTeacher();
    }

    private void OnEnable()
    {
        GameEvents.OnRoomUnlocked += HandleRoomUnlocked;
        GameEvents.OnRoomLevelChanged += HandleRoomLevelChanged;
        GameEvents.OnRoomCapacityChanged += HandleRoomCapacityChanged;
        GameEvents.OnRoomTeacherChanged += HandleRoomTeacherChanged;
        GameEvents.OnTeacherUnlocked += HandleTeacherUnlocked;
        GameEvents.OnCoinChanged +=  HandleCoinChanged;
        GameEvents.OnFameChanged +=  HandleFameChanged;
    }



    private void OnDisable()
    {
        GameEvents.OnRoomUnlocked -= HandleRoomUnlocked;
        GameEvents.OnRoomLevelChanged -= HandleRoomLevelChanged;
        GameEvents.OnRoomCapacityChanged -= HandleRoomCapacityChanged;
        GameEvents.OnRoomTeacherChanged -= HandleRoomTeacherChanged;
        GameEvents.OnTeacherUnlocked -= HandleTeacherUnlocked;
        GameEvents.OnCoinChanged -=  HandleCoinChanged;
        GameEvents.OnFameChanged -=  HandleFameChanged;
    }
    private void HandleRoomUnlocked(string roomID)
    {
        if (roomID != config.RoomID) return;

        UpdateRoomVisuals();
        TrySpawnTeacher();
    }

    private void HandleTeacherUnlocked(string teacherID)
    {
        if (roomData == null) return;
        if (roomData.currentTeacherID == teacherID && IsUnlocked)
        {
            TrySpawnTeacher();
        }

    }
    private void HandleRoomTeacherChanged(string roomID, string teacherID)
    {
        if (roomID != config.RoomID) return;
        if (!IsUnlocked) return;

        if (currentTeacherObject != null)
        {
            Destroy(currentTeacherObject);
            currentTeacherObject = null;
        }

        TrySpawnTeacher();
    }

    private void HandleRoomLevelChanged(string roomID, int newLevel)
    {
        if (roomID != config.RoomID) return;
        UpdateRoomVisuals();
    }

    private void HandleRoomCapacityChanged(string roomID, int newCapacity)
    {
        if (roomID != config.RoomID) return;
        UpdateRoomVisuals();
    }

    private void HandleCoinChanged(int obj)
    {
        if(!IsUnlocked)
            UpdateRoomVisuals();
    }

    private void HandleFameChanged(int obj)
    {
        if(!IsUnlocked)
            UpdateRoomVisuals();
    }

    private void UpdateRoomVisuals()
    {
        if (roomData == null) return;

        Transform lockUIRoot = transform.GetChild(transform.childCount - 1);

        // Room locked
        if (!IsUnlocked)
        {
            lockUIRoot.gameObject.SetActive(true);

            bool canUnlockByFame = PlayerManager.Instance.GetFame() >= config.UnlockCondition;
            var textComp = lockUIRoot.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            var coinObj = lockUIRoot.GetChild(1).gameObject;
            var fameObj = lockUIRoot.GetChild(2).gameObject;

            textComp.text = (canUnlockByFame ? config.UnlockCost : config.UnlockCondition).ToString();
            coinObj.SetActive(canUnlockByFame);
            fameObj.SetActive(!canUnlockByFame);

            if (currentTeacherObject != null) Destroy(currentTeacherObject);
            foreach (var t in TablePos) if (t) t.gameObject.SetActive(false);
            return;
        }

        // Room unlocked
        lockUIRoot.gameObject.SetActive(false);
        if (unlockSignObj != null) unlockSignObj.SetActive(false);

        int actualCapacity = Mathf.Max(CurrentCapacity, 1);

        for (int i = 0; i < TablePos.Count; i++)
        {
            if (TablePos[i] == null) continue;

            bool isTableUnlocked = i < actualCapacity;
            TablePos[i].gameObject.SetActive(isTableUnlocked);

            if (i < bookList.Count && bookList[i] != null)
            {
                bool showBook = isTableUnlocked && sittingList.Contains(i);
                bookList[i].SetActive(showBook);
            }
        }
        TrySpawnTeacher();
    }
    private void SpawnTeacher()
    {
        if (!IsUnlocked) return;
        string tID = roomData.currentTeacherID;

        if (string.IsNullOrEmpty(tID))
            return;

        if (!TeacherManager.Instance.IsUnlocked(tID))
        {
            if (currentTeacherObject != null)
                Destroy(currentTeacherObject);
            return;
        }

        if (currentTeacherObject != null)
        {
            var teacherCtrl = currentTeacherObject.GetComponent<TeacherController>();
            if (teacherCtrl != null)
                teacherCtrl.Setup(tID);

            currentTeacherObject.SetActive(true);
            return;
        }


        TeacherConfig tConfig = TeacherManager.Instance.GetTeacherConfig(tID);
        if (tConfig == null || tConfig.prefabModel == null)
            return;

        if (teacherSpawnPoint == null)
            return;

        currentTeacherObject = Instantiate(
            tConfig.prefabModel,
            teacherSpawnPoint.position,
            Quaternion.identity,
            this.transform
        );

        var ctrl = currentTeacherObject.GetComponent<TeacherController>();
        if (ctrl != null)
            ctrl.Setup(tID);

        var appearance = currentTeacherObject.GetComponent<TeacherAppearance>();
        if (appearance != null)
            appearance.SetDeskPosition(teacherSpawnPoint);
    }

    private void InitializeSeats()
    {
        seatPos.Clear();
        TablePos.Clear();
        bookList.Clear();
        waitingPos.Clear();
        sittingList.Clear();
        usedSeat.Clear();
        waitingSpotStates.Clear();

        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name.StartsWith("Ghe")) seatPos.Add(child);
            else if (child.name.StartsWith("DiemCho"))
            {
                waitingPos.Add(child);
                waitingSpotStates.Add(false);
            }
        }

        Transform tableContainer = transform.Find("Ban") ?? transform.GetChild(0);
        if (tableContainer != null)
        {
            foreach (Transform t in tableContainer)
            {
                if (t.name.StartsWith("Ban"))
                {
                    TablePos.Add(t);
                    Transform book = t.GetChild(2);
                    bookList.Add(book != null ? book.gameObject : null);
                    if (book != null) book.gameObject.SetActive(false);
                }
            }
        }
    }


    public void UnlockRoom()
    {
        if (IsUnlocked || roomData == null) return;
        int cost = config.UnlockCost;
        if (PlayerManager.Instance.SpendCoin(cost))
        {
            RoomManager.Instance.UnlockRoom(config.RoomID);
            UpdateRoomVisuals();
            Debug.Log($"Mo khoa phong '{config.RoomName}' thanh cong!");
            if (unlockEffect != null)
            {
                ParticleSystem newEffect = Instantiate(unlockEffect, transform.position, Quaternion.identity);
                newEffect.transform.SetParent(transform);
                newEffect.Play();
                float duration = newEffect.main.duration + newEffect.main.startLifetime.constantMax;
                Destroy(newEffect.gameObject, duration);
            }
            var handle = GetComponent<StudentClassroomHandle>();
            if (handle != null) handle.ProcessQueue();
        }
        else
        {
            Debug.Log("Khong du tien de mo khoa phong!");
        }
    }

    public bool AssignTeacher(string teacherID)
    {
        if (!IsUnlocked || roomData == null) return false;

        bool result = RoomManager.Instance.AssignTeacher(config.RoomID, teacherID);
        UpdateRoomVisuals();
        var handle = GetComponent<StudentClassroomHandle>();
        if (handle != null) handle.ProcessQueue();
        return result;
    }

    public int BookSpot()
    {
        for (int i = 0; i < waitingSpotStates.Count; i++)
        {
            if (!waitingSpotStates[i])
            {
                waitingSpotStates[i] = true;
                return i;
            }
        }
        return -1;
    }

    public void ReleaseSpot(int index)
    {
        if (index >= 0 && index < waitingSpotStates.Count)
            waitingSpotStates[index] = false;
    }

    public int CheckEmptySeat()
    {
        int limit = Mathf.Min(CurrentCapacity, seatPos.Count);
        for (int i = 0; i < limit; i++)
        {
            if (!usedSeat.Contains(i)) return i;
        }
        return -1;
    }

    public void OnStudentSit(int seatIndex)
    {
        if (!sittingList.Contains(seatIndex)) sittingList.Add(seatIndex);
        if (!usedSeat.Contains(seatIndex)) usedSeat.Add(seatIndex);
        UpdateRoomVisuals();
    }

    public void OnStudentLeave(int seatIndex)
    {
        sittingList.Remove(seatIndex);
        usedSeat.Remove(seatIndex);
        UpdateRoomVisuals();
    }

    private void TrySpawnTeacher()
    {
        if (!IsUnlocked) return;
        if (roomData == null) return;

        string tID = roomData.currentTeacherID;
        if (string.IsNullOrEmpty(tID)) return;
        if (!TeacherManager.Instance.IsUnlocked(tID)) return;

        SpawnTeacher();
    }


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return; // Thoát hàm, không thực hiện logic phía dưới
        }
        if (config == null || roomData == null) return;

        if (!IsUnlocked) {
            if(PlayerManager.Instance.GetFame() >= config.UnlockCondition) UnlockRoom();
        }
        else UIManager.Instance.ShowRoomPanel(config, roomData);
    }
}